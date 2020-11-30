using RPSLS.Game.Client.Models;

namespace RPSLS.Game.Client.Clients
{
    public interface IConfigurationManagerClient
    {
        GameSettingsDto GetSettings();
    }
}
