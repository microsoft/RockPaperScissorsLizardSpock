using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Http;

namespace RPSLS.Game.Server.GrpcInterceptors
{
    public class AzdsMetadataInterceptor : Interceptor
    {
        private const string AzdsRouteAsKey = "azds-route-as";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AzdsMetadataInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            AddAzdsHeadersToMetadata(ref context);

            return base.AsyncServerStreamingCall(request, context, continuation);
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            AddAzdsHeadersToMetadata(ref context);
            return base.AsyncUnaryCall(request, context, continuation);
        }

        private void AddAzdsHeadersToMetadata<TRequest, TResponse>(ref ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class
        {
            var headers = context.Options.Headers;

            if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey(AzdsRouteAsKey))
            {
                var routeAs = _httpContextAccessor.HttpContext.Request.Headers[AzdsRouteAsKey].ToString();
                headers.Add(AzdsRouteAsKey, routeAs);
            }
        }
    }
}
