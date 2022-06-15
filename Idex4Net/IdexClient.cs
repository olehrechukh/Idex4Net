using System.Collections.Generic;
using System.Net.Http;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Idex4Net.Domain.CoreModels;
using Idex4Net.Domain.ExchangeModels;
using Idex4Net.Extensions;

namespace Idex4Net
{
    public class IdexClient
    {
        private readonly ApiCredentials _apiCredentials;
        private readonly IdexExecutor _idexExecutor;
        private readonly IdexClientCache _cache;

        public IdexClient(HttpClient httpClient, ApiCredentials apiCredentials)
        {
            _apiCredentials = apiCredentials;
            _idexExecutor = new IdexExecutor(httpClient, apiCredentials);
            _cache = new IdexClientCache();
        }

        public Task<Result<IdexContractAddress>> GetContractAddress(CancellationToken cancellationToken)
        {
            return _idexExecutor.Request<IdexContractAddress>("/returnContractAddress", cancellationToken);
        }

        public Task<Result<Dictionary<string, Currency>>> GetCurrencies(CancellationToken cancellationToken)
        {
            return _idexExecutor.Request<Dictionary<string, Currency>>("/returnCurrencies", cancellationToken);
        }

        public async Task<Result<CreatedOrder>> CreateOrder(
            Order order,
            CancellationToken cancellationToken)
        {
            var currencies = await GetCachedCurrencies(cancellationToken);
            var contractAddress = await GetCachedContractAddress(cancellationToken);
            var nonce = await GetNextCachedNonce(cancellationToken);
            var parameters = GetCreateOrderParameters(currencies, order, contractAddress, nonce);

            return await _idexExecutor.Request<CreatedOrder>("/order", cancellationToken, parameters, true);
        }

        public async Task<Result<CancelledOrder>> CancelOrder(string orderHash, CancellationToken cancellationToken)
        {
            var nonce = await GetNextCachedNonce(cancellationToken);
            var parameters = new Dictionary<string, object>
            {
                ["orderHash"] = orderHash,
                ["nonce"] = nonce
            };

            // Only the 2 fields above should be authenticated (without address), so authentication is triggered
            // manually and skipped on RestRequest call.
            _idexExecutor.AddVrsParameters(parameters);
            parameters["address"] = _apiCredentials.WalletAddress;
            return await _idexExecutor.Request<CancelledOrder>("/cancel", cancellationToken, parameters);
        }

        public async Task<Result<TradeOrder>> TradeOrder(
            IEnumerable<string> orderHashes,
            CurrencyPair currencyPair,
            OrderSide side,
            decimal amount,
            CancellationToken cancellationToken)
        {
            var currencies = await GetCachedCurrencies(cancellationToken);
            var allParameters = new List<Dictionary<string, object>>();
            foreach (var orderHash in orderHashes)
            {
                var parameters = await CreateTradeParameters(orderHash, currencyPair, side, amount, currencies,
                    cancellationToken);

                _idexExecutor.AddVrsParameters(parameters);
                allParameters.Add(parameters);
            }

            return await _idexExecutor.Request<TradeOrder>("/trade", cancellationToken, allParameters, true);
        }

        public Task<Result<TradeOrder>> TradeOrder(
            string orderHash,
            CurrencyPair currencyPair,
            OrderSide side,
            decimal amount,
            CancellationToken cancellationToken)
        {
            return TradeOrder(new[] {orderHash}, currencyPair, side, amount, cancellationToken);
        }

        private async Task<Dictionary<string, object>> CreateTradeParameters(
            string orderHash,
            CurrencyPair currencyPair,
            OrderSide side,
            decimal amount,
            IReadOnlyDictionary<string, Currency> currencies,
            CancellationToken cancellationToken)
        {
            var amountBuy = GetAmountBuy(currencyPair, side, amount, currencies);
            var nonce = await GetNextCachedNonce(cancellationToken);
            var param = new Dictionary<string, object>
            {
                ["orderHash"] = orderHash,
                ["amount"] = amountBuy,
                ["address"] = _apiCredentials.WalletAddress,
                ["nonce"] = nonce
            };
            return param;
        }

        private static BigInteger GetAmountBuy(
            CurrencyPair currencyPair,
            OrderSide side,
            decimal amount,
            IReadOnlyDictionary<string, Currency> currencies)
        {
            var currency = side == OrderSide.Buy ? currencyPair.Quote : currencyPair.Base;
            var amountBuy = IntegerRepresentation(amount, currencies[currency].DecimalsPower10);

            return amountBuy;
        }

        private Dictionary<string, object> GetCreateOrderParameters(
            IReadOnlyDictionary<string, Currency> currencies,
            Order order,
            string contractAddress,
            long nonce)
        {
            Currency currencyBuy;
            Currency currencySell;
            BigInteger amountBuy;
            BigInteger amountSell;
            if (order.Side == OrderSide.Buy)
            {
                currencyBuy = currencies[order.CurrencyPair.Base];
                currencySell = currencies[order.CurrencyPair.Quote];
                amountBuy = IntegerRepresentation(order.Amount, currencyBuy.DecimalsPower10);
                amountSell = IntegerRepresentation(order.Price * order.Amount, currencySell.DecimalsPower10);
            }
            else
            {
                currencyBuy = currencies[order.CurrencyPair.Quote];
                currencySell = currencies[order.CurrencyPair.Base];
                amountBuy = IntegerRepresentation(order.Price * order.Amount, currencySell.DecimalsPower10);
                amountSell = IntegerRepresentation(order.Amount, currencyBuy.DecimalsPower10);
            }

            var parameters = new Dictionary<string, object>
            {
                ["contractAddress"] = contractAddress,
                ["tokenBuy"] = currencyBuy.Address,
                ["amountBuy"] = amountBuy,
                ["tokenSell"] = currencySell.Address,
                ["amountSell"] = amountSell,
                ["expires"] = (long) 100000,
                ["nonce"] = nonce,
                ["address"] = _apiCredentials.WalletAddress
            };
            return parameters;
        }

        public Task<Result<Dictionary<string, Balance>>> GetBalances(CancellationToken cancellationToken)
        {
            var parameters = new Dictionary<string, object> {["address"] = _apiCredentials.WalletAddress};

            return _idexExecutor.Request<Dictionary<string, Balance>>("/returnCompleteBalances", cancellationToken,
                parameters);
        }

        private async Task<string> GetCachedContractAddress(CancellationToken cancellationToken)
        {
            var contractAddress = _cache.ContractAddress;
            if (contractAddress == null)
            {
                var address = await RequestManager.WithRetry(() => GetContractAddress(cancellationToken));
                contractAddress = address.Address;
                _cache.ContractAddress = contractAddress;
            }

            return contractAddress;
        }

        private async Task<Dictionary<string, Currency>> GetCachedCurrencies(CancellationToken cancellationToken)
        {
            var currencies = _cache.Currencies;
            if (currencies == null)
            {
                currencies = await RequestManager.WithRetry(() => GetCurrencies(cancellationToken));
                _cache.Currencies = currencies;
            }

            return currencies;
        }

        private async Task<long> GetNextCachedNonce(CancellationToken cancellationToken)
        {
            if (_cache.Nonce.HasValue)
            {
                _cache.Nonce++;
            }

            var nextNonce = await RequestManager.WithRetry(() => GetNextNonce(cancellationToken));

            _cache.Nonce = nextNonce.Nonce;
            return nextNonce.Nonce;
        }

        private Task<Result<IdexNextNonce>> GetNextNonce(CancellationToken cancellationToken)
        {
            var parameters = new Dictionary<string, object> {["address"] = _apiCredentials.WalletAddress};

            return _idexExecutor.Request<IdexNextNonce>("/returnNextNonce", cancellationToken, parameters);
        }

        private static BigInteger IntegerRepresentation(decimal amount, decimal power)
        {
            return (BigInteger) (amount * power);
        }
    }
}