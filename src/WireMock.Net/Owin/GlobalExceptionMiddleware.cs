using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
#if !NETSTANDARD
using Microsoft.Owin;
#else
using Microsoft.AspNetCore.Http;
#endif

namespace WireMock.Owin
{
#if !NETSTANDARD
    internal class GlobalExceptionMiddleware : OwinMiddleware
#else
    internal class GlobalExceptionMiddleware
#endif
    {
        private readonly WireMockMiddlewareOptions _options;

#if !NETSTANDARD
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

#if NETSTANDARD
        public RequestDelegate Next { get; }
#endif

        private readonly OwinResponseMapper _responseMapper = new OwinResponseMapper();

#if !NETSTANDARD
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
                await _responseMapper.MapAsync(new ResponseMessage { StatusCode = 500, Body = JsonConvert.SerializeObject(ex) }, ctx.Response);
            }
        }
    }
}
