using RPSLS.Game.Server.Config;

namespace RPSLS.Game.Server.Clients
{
    public interface IConfigurationManagerClient
    {
        GameSettingsDto GetSettings();
    }
}
