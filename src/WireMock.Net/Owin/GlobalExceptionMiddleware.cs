using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
#if !USE_ASPNETCORE
using Microsoft.Owin;
#else
using Microsoft.AspNetCore.Http;
#endif

namespace WireMock.Owin
{
#if !USE_ASPNETCORE
    internal class GlobalExceptionMiddleware : OwinMiddleware
#else
    internal class GlobalExceptionMiddleware
#endif
    {
        private readonly WireMockMiddlewareOptions _options;

#if !USE_ASPNETCORE
        public GlobalExceptionMiddleware(OwinMiddleware next, WireMockMiddlewareOptions options) : base(next)
        {
            _options = options;
        }
#else
        public GlobalExceptionMiddleware(RequestDelegate next, WireMockMiddlewareOptions options)
        {
            Next = next;
            _options = options;
        }
#endif

#if USE_ASPNETCORE
        public RequestDelegate Next { get; }
#endif

        private readonly OwinResponseMapper _responseMapper = new OwinResponseMapper();

#if !USE_ASPNETCORE
        public override async Task Invoke(IOwinContext ctx)
#else
        public async Task Invoke(HttpContext ctx)
#endif
        {
            try
            {
                await Next?.Invoke(ctx);
            }
            catch (Exception ex)
            {
                _options.Logger.Error("HttpStatusCode set to 500 {0}", ex);
                await _responseMapper.MapAsync(ResponseMessageBuilder.Create(JsonConvert.SerializeObject(ex), 500), ctx.Response);
            }
        }
    }
}
