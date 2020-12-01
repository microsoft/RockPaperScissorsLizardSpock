using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RPSLS.Game.Server.Config;
using RPSLS.Game.Shared.Config;
using System.Threading.Tasks;

namespace RPSLS.Game.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly IOptions<ClientSettings> _clientSettings;

        public SettingsController(IOptions<ClientSettings> clientSettings)
        {
            _clientSettings = clientSettings;
        }

        public async Task<ClientSettings> GetAsync() => await Task.FromResult(_clientSettings.Value);
    }
}
