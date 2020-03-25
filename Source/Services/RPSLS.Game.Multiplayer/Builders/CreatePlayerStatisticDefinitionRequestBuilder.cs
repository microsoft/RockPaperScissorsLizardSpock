using PlayFab.AdminModels;

namespace RPSLS.Game.Multiplayer.Builders
{
    public class CreatePlayerStatisticDefinitionRequestBuilder : PlayFabRequestCommonBuilder<CreatePlayerStatisticDefinitionRequestBuilder, CreatePlayerStatisticDefinitionRequest>
    {
        public CreatePlayerStatisticDefinitionRequestBuilder() : base()
        {
            _product.VersionChangeInterval = StatisticResetIntervalOption.Never;
        }

        public CreatePlayerStatisticDefinitionRequestBuilder WithAggregatedStat(string statisticsName)
        {
            _product.StatisticName = statisticsName;
            _product.AggregationMethod = StatisticAggregationMethod.Sum;
            return this;
        }
    }
}
