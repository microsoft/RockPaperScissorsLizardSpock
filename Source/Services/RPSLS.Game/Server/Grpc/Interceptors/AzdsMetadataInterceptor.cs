using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Http;
using System;

namespace RPSLS.Game.Server.Grpc.Interceptors
{
    public class AzdsMetadataInterceptor : Interceptor
    {
        private const string PropagationHeaderKey = "azds-route-as";
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

            if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey(PropagationHeaderKey))
            {
                var headers = context.Options.Headers;

                if (headers == null)
                {
                    headers = new Metadata();
                    var options = context.Options.WithHeaders(headers);
                    context = new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, options);
                }

                var routeAs = _httpContextAccessor.HttpContext.Request.Headers[PropagationHeaderKey].ToString();
                headers.Add(PropagationHeaderKey, routeAs);

            }
        }
    }
}
