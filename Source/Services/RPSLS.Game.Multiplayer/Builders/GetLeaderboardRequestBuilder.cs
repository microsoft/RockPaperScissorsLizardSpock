using PlayFab.ServerModels;

namespace RPSLS.Game.Multiplayer.Builders
{
    public class GetLeaderboardRequestBuilder : PlayFabRequestCommonBuilder<GetLeaderboardRequestBuilder, GetLeaderboardRequest>
    {
        public GetLeaderboardRequestBuilder WithStats(string statisticsName)
        {
            _product.StatisticName = statisticsName;
            return this;
        }

        public GetLeaderboardRequestBuilder WithLimits(int start, int count)
        {
            _product.StartPosition = start;
            _product.MaxResultsCount = count;
            return this;
        }
    }
}
