using System;

namespace RPSLS.Game.Multiplayer.Models
{
    public class Token
    {
        private readonly int _maxTokenLifeMinutes;
        private readonly DateTime _creationTimeStamp;

        public Token(string value, int maxTokenLifeMinutes)
        {
            Value = value;
            _maxTokenLifeMinutes = maxTokenLifeMinutes;
            _creationTimeStamp = DateTime.UtcNow;
        }

        public string Value { get; private set; }

        public bool IsExpired
        {
            get
            {
                var tokenLife = DateTime.UtcNow.Subtract(_creationTimeStamp);
                return tokenLife.Minutes > _maxTokenLifeMinutes;
            }
        }
    }
}
