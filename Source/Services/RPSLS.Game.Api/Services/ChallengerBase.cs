using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace RPSLS.Game.Api.Services
{
    public abstract class ChallengerBase
    {
        private readonly Uri _url;
        private readonly IHttpClientFactory _httpClientFactory;
        public ChallengerBase(string url, IHttpClientFactory httpClientFactory)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url), $"Url of challenger can't be null");
            }
            _url = new Uri(url);
            _httpClientFactory = httpClientFactory;
        }

        public async Task<PickDto> Pick(IDictionary<string, string> context, bool isTwitterUser, string userName)
        {
            var client = _httpClientFactory.CreateClient("Challenger");
            client.BaseAddress = _url;
            PropagateContext(client, context);
            var response = await client.GetAsync((isTwitterUser) ? $"/pick?username={userName}" : "/pick");
            var result = await response.Content.ReadAsStringAsync();
            var jsonOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<PickDto>(result, jsonOptions);
        }

        private void PropagateContext(HttpClient client, IDictionary<string, string> context)
        {
            foreach (var item in context)
            {
                client.DefaultRequestHeaders.Add(item.Key, item.Value);
            }
        }
    }
}
