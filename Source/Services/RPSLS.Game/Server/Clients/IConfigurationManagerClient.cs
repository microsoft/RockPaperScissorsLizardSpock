
using RPSLS.Game.Shared;

namespace RPSLS.Game.Server.Clients
{
    public interface IConfigurationManagerClient
    {
        GameSettingsDto GetSettings();
    }
}
