using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RPSLS.DotNetPlayer.Api.Services;
using RPSLS.DotNetPlayer.API.Services;
using RPSLS.DotNetPlayer.API.Settings;
using System.Threading.Tasks;

namespace RPSLS.DotNetPlayer.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PickController : ControllerBase
    {
        private readonly ILogger<PickController> _logger;
        private readonly StrategySettings _settings;
        private readonly IPickStrategyFactory _strategyFactory;
        private readonly IPredictorProxyService _predictorProxy;

        public PickController(
            ILogger<PickController> logger, 
            IOptions<StrategySettings> settings,
            IPickStrategyFactory strategyFactory,
            IPredictorProxyService predictorProxy)
        {
            _logger = logger;
            _settings = settings.Value;
            _strategyFactory = strategyFactory;
            _predictorProxy = predictorProxy;
        }

        [HttpGet]
        public async Task<Choice> Get([FromQuery(Name = "username")] string username)
        {
            if(!string.IsNullOrWhiteSpace(username))
            {
                try
                {
                    var prediction = await _predictorProxy.GetPickPredicted(username);
                    return prediction;
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }                
            }

            var envStrategy = _settings.Pick_Strategy;
            _strategyFactory.SetDefaultStrategy(envStrategy);
            var strategy = _strategyFactory.GetStrategy();
            var result = strategy.GetChoice();
            return result;
        }
    }
}
