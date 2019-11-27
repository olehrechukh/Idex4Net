using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Idex4Net.Domain.CoreModels;
using Idex4Net.Domain.ExchangeModels;
using Idex4Net.Extensions;
using Newtonsoft.Json;

namespace Idex4Net
{
    public class IdexExecutor
    {
        private const string RestHost = "https://api.idex.market";
        private readonly ApiCredentials _apiCredentials;

        public IdexExecutor(ApiCredentials apiCredentials)
        {
            _apiCredentials = apiCredentials;
        }

        public async Task<Result<TOut>> Request<TOut>(string url, CancellationToken cancellationToken,
            object parameters = null, bool isSigned = false)
        {
            var content = await InnerRequest(url, cancellationToken, isSigned, parameters);
            try
            {
                if (IsRestError(content, out var error))
                {
                    return Result<TOut>.Failed(error);
                }

                var result = JsonConvert.DeserializeObject<TOut>(content);

                return Result<TOut>.Success(result);
            }
            catch (Exception)
            {
                var parsingError = new ExchangeError(ExchangeErrorType.ParsingError, content);
                return Result<TOut>.Failed(parsingError);
            }
        }

        private async Task<string> InnerRequest(string url, CancellationToken cancellationToken, bool isSigned,
            object parameters)
        {
            if (parameters == null)
            {
                parameters = new Dictionary<string, object>();
            }

            using var client = new HttpClient();

            if (isSigned)
            {
                AddVrsParameters(parameters);
                client.DefaultRequestHeaders.Add("API-KEY", _apiCredentials.ApiKey);
            }

            var response = await client.PostAsJsonAsync($"{RestHost}{url}", parameters, cancellationToken);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public void AddVrsParameters(object parameters)
        {
            if (!(parameters is IDictionary<string, object> dictionaryParameters))
            {
                return;
            }

            if (!dictionaryParameters.Any())
            {
                return;
            }

            var paramsBytes = new List<byte[]>();
            // In the python-idex there are 2 types described.
            // "address" which means a string
            // "uint256" which means an integer
            foreach (var p in dictionaryParameters.Values)
            {
                if (p is long pLong)
                {
                    // For simplicity, I restricted it to using long (not int).
                    // Integer is converted to bigendian bytes, then left padded to 32 bytes using zeros, then hexlified.
                    var bytes = BitConverter.GetBytes(pLong);
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(bytes);
                    var padding = new byte[32 - bytes.Length];
                    var padded = ConcatenateBytes(padding, bytes);
                    paramsBytes.Add(padded);
                }
                else if (p is BigInteger pBigInt)
                {
                    var bytes = pBigInt.ToByteArray();
                    Array.Reverse(bytes);
                    var padding = new byte[32 - bytes.Length];
                    var padded = ConcatenateBytes(padding, bytes);
                    paramsBytes.Add(padded);
                }
                else if (p is string pString)
                {
                    // String is expected to be an address in the form 0x...
                    // The 0x is truncated, then converted to bytes (not unhexlified).
                    var bytes = pString.Substring(2).FromHexString();
                    paramsBytes.Add(bytes);
                }
                else
                {
                    // Field "expires" used to be an int (not long) causing this error. Do not use plain integer literals.
                    throw new Exception("Only integers and strings are supposed to be used.");
                }
            }

            var paramsConcatenated = ConcatenateBytes(paramsBytes.ToArray());
            var hash1 = Sha3(paramsConcatenated);
            var salted = ConcatenateBytes(Encoding.ASCII.GetBytes("\u0019Ethereum Signed Message:\n32"), hash1);
            var hash2 = Sha3(salted);

            var privateKey = new Nethereum.Signer.EthECKey(_apiCredentials.ApiSecret);
            var sign = privateKey.SignAndCalculateV(hash2);

            dictionaryParameters["v"] = sign.V[0];
            dictionaryParameters["r"] = HoshoEthUtil.HexExtensions.ToHexString(sign.R, true);
            dictionaryParameters["s"] = HoshoEthUtil.HexExtensions.ToHexString(sign.S, true);

            FixBigIntegerHeaders(dictionaryParameters);
        }

        private static byte[] ConcatenateBytes(params byte[][] data)
        {
            var totalLen = data.Sum(x => x.Length);
            var total = new byte[totalLen];

            var offset = 0;
            foreach (var segment in data)
            {
                Buffer.BlockCopy(segment, 0, total, offset, segment.Length);
                offset += segment.Length;
            }

            return total;
        }

        private static void FixBigIntegerHeaders(IDictionary<string, object> param)
        {
            // Collections cannot be modified and enumerated at same time. A copy is required.
            var param2 = new Dictionary<string, object>(param);
            foreach (var (key, value) in param2.Where(item => item.Value is BigInteger))
            {
                param[key] = value.ToString();
            }
        }

        private static byte[] Sha3(byte[] data)
        {
            // Correct, 256 digest size is implicit apparently.
            // https://www.nuget.org/packages/Nethereum.Signer/
            return new Nethereum.Util.Sha3Keccack().CalculateHash(data);
        }

        private static bool IsRestError(string content, out ExchangeError error)
        {
            {
                var isRestError = IsRestErrorCore(content, out var errorMessage);

                if (!isRestError)
                {
                    error = default;
                    return false;
                }

                var errorType = GetErrorType(errorMessage);

                error = errorType == ExchangeErrorType.PendingError
                    ? ExchangeError.Pending(GetPendingTime(errorMessage), errorMessage)
                    : new ExchangeError(errorType, errorMessage);
            }

            return true;
        }


        private static bool IsRestErrorCore(string content, out string errorMessage)
        {
            if (content.Contains("\"error\":"))
            {
                var message = JsonConvert.DeserializeObject<ErrorMessage>(content);
                errorMessage = message.Error;
                return true;
            }

            if (content.Contains("\"message\": \"Internal server error\""))
            {
                var message = JsonConvert.DeserializeObject<InternalErrorMessage>(content);
                errorMessage = message.Message;
                return true;
            }

            errorMessage = default;
            return false;
        }

        private static ExchangeErrorType GetErrorType(string message)
        {
            if (message == "Order not found" || message == "Order no longer available.")
            {
                return ExchangeErrorType.OrderNotFound;
            }

            if (message == "Please enter a valid amount.")
            {
                return ExchangeErrorType.InvalidOrder;
            }

            if (message == "You have insufficient funds to place this order." ||
                message.Contains("You have insufficient funds to match this order."))
            {
                return ExchangeErrorType.InsufficientFunds;
            }

            if (message.Contains("value of order must be at least"))
            {
                return ExchangeErrorType.InvalidOrder;
            }

            if (message.Contains("Your order is still pending "))
            {
                return ExchangeErrorType.PendingError;
            }

            if (message == "Invalid API Key" || message.Contains("Invalid address"))
            {
                return ExchangeErrorType.Authentication;
            }

            return ExchangeErrorType.Unknown;
        }

        private static TimeSpan GetPendingTime(string message)
        {
            var lastIndex = message.IndexOf("seconds", StringComparison.Ordinal);
            var firstIndex = message.IndexOf("after", StringComparison.Ordinal) + "after".Length;

            var seconds = message.Substring(firstIndex, lastIndex - firstIndex);

            if (int.TryParse(seconds, out var value))
            {
                return TimeSpan.FromSeconds(value);
            }

            throw new ArgumentException($"Cannot parse time span. {message}");
        }
    }
}