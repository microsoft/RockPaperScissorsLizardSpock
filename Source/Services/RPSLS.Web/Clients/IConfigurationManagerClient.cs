using RPSLS.Web.Models;

namespace RPSLS.Web.Clients
{
    public interface IConfigurationManagerClient
    {
        GameSettingsDto GetSettings();
    }
}
