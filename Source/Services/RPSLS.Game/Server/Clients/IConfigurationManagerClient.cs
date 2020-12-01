using RPSLS.Game.Shared.Config;

namespace RPSLS.Game.Server.Clients
{
    public interface IConfigurationManagerClient
    {
        GameSettingsDto GetSettings();
    }
}
