using Microsoft.AspNetCore.Http;

namespace RPSLS.Game.Client.Clients
{
    public abstract class BaseClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BaseClient()
        {
            
        }

        protected Grpc.Core.Metadata GetRequestMetadata()
        {
            var metadata = new Grpc.Core.Metadata();
           
            return metadata;
        }
    }
}
