using PlayFab.MultiplayerModels;

namespace RPSLS.Game.Multiplayer.Builders
{
    public class GetMatchmakingTicketRequestBuilder : PlayFabRequestCommonBuilder<GetMatchmakingTicketRequestBuilder, GetMatchmakingTicketRequest>
    {
        public GetMatchmakingTicketRequestBuilder WithQueue(string queueName)
        {
            _product.QueueName = queueName;
            return this;
        }

        public GetMatchmakingTicketRequestBuilder WithTicketId(string ticketId)
        {
            _product.TicketId = ticketId;
            return this;
        }
    }
}
