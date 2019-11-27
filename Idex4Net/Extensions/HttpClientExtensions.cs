using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Idex4Net.Domain;
using Newtonsoft.Json;

namespace Idex4Net.Extensions
{
    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> PostAsJsonAsync(this HttpClient httpClient, string url,
            object jsonValue, CancellationToken cancellationToken)
        {
            var content = new StringContent(JsonConvert.SerializeObject(jsonValue), Encoding.UTF8, "application/json");
            return httpClient.PostAsync(url, content, cancellationToken);
        }
    }
}