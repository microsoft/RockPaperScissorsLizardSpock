using Microsoft.Extensions.Options;
using RPSLS.Game.Multiplayer.Config;
using System;
using System.Linq;

namespace RPSLS.Game.Multiplayer.Services
{
    public class TokenService : ITokenService
    {
        private readonly TokenSettings _settings;
        private readonly Random _randGenerator;

        public TokenService(IOptions<TokenSettings> options)
        {
            _settings = options.Value;
            _randGenerator = new Random();
        }

        public string GenerateToken()
        {
            return new string(Enumerable.Repeat(_settings.ValidCharacters, _settings.Length).Select(s => s[_randGenerator.Next(s.Length)]).ToArray());
        }
    }
}
