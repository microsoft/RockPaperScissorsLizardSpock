using PlayFab.MultiplayerModels;
using System.Collections.Generic;

namespace RPSLS.Game.Multiplayer.Builders
{
    public class SetMatchmakingQueueRequestBuilder : PlayFabRequestCommonBuilder<SetMatchmakingQueueRequestBuilder, SetMatchmakingQueueRequest>
    {
        public SetMatchmakingQueueRequestBuilder WithQueue(string queueName, uint playersMatch)
        {
            var queueConfig = _product.MatchmakingQueue ?? new MatchmakingQueueConfig();
            queueConfig.Name = queueName;
            queueConfig.MinMatchSize = playersMatch;
            queueConfig.MaxMatchSize = playersMatch;
            queueConfig.ServerAllocationEnabled = false;

            _product.MatchmakingQueue = queueConfig;
            return this;
        }

        public SetMatchmakingQueueRequestBuilder WithQueueStringRule(string name, string path, string defaultValue)
        {
            var queueConfig = _product.MatchmakingQueue ?? new MatchmakingQueueConfig();
            var stringRules = queueConfig.StringEqualityRules ?? new List<StringEqualityRule>();

            var rule = new StringEqualityRule()
            {
                Name = name,
                Attribute = new QueueRuleAttribute
                {
                    Path = path,
                    Source = AttributeSource.User
                },
                DefaultAttributeValue = defaultValue,
                Weight = 1
            };

            stringRules.Add(rule);
            queueConfig.StringEqualityRules = stringRules;
            _product.MatchmakingQueue = queueConfig;
            return this;
        }
    }
}
