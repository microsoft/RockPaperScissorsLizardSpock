using RPSLS.Game.Client.Models;

namespace RPSLS.Game.Server.Clients
{
    public interface IConfigurationManagerClient
    {
        GameSettingsDto GetSettings();
    }
}
