using Microsoft.Extensions.Options;
using RPSLS.DotNetPlayer.Api.Models;
using RPSLS.DotNetPlayer.Api.Settings;
using RPSLS.DotNetPlayer.API;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace RPSLS.DotNetPlayer.Api.Services
{

    public class PredictorProxyService : IPredictorProxyService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly PredictorSettings _settings;

        public PredictorProxyService(
            IHttpClientFactory httpClientFactory,
            IOptions<PredictorSettings> options)
        {
            _httpClientFactory = httpClientFactory;
            _settings = options.Value;
        }

        public async Task<Choice> GetPickPredicted(string userName)
        {
            var client = _httpClientFactory.CreateClient("Predictor");
            var predictorUrl = _settings.Predictor_Url;
            var response = await client.GetAsync($"{predictorUrl}&humanPlayerName={userName}");
            var jsonResult = await response.Content.ReadAsStringAsync();
            var jsonOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<PredictionDto>(jsonResult, jsonOptions);
            var prediction = Enum.Parse<RPSLSEnum>(result.Prediction, true);
            return new Choice(prediction);
        }
    }
}
