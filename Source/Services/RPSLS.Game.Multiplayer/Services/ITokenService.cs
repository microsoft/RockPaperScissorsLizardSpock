using RPSLS.Game.Multiplayer.Models;
using System.Threading.Tasks;

namespace RPSLS.Game.Multiplayer.Services
{
    public interface ITokenService
    {
        string GenerateToken();
    }
}
