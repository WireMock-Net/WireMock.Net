using System;
using System.Threading.Tasks;
using log4net;
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
        private static readonly ILog Log = LogManager.GetLogger(typeof(GlobalExceptionMiddleware));
#if !NETSTANDARD
        public GlobalExceptionMiddleware(OwinMiddleware next) : base(next) { }
#else
        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            Next = next;
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
                Log.Error("HttpStatusCode set to 500", ex);
                await _responseMapper.MapAsync(new ResponseMessage { StatusCode = 500, Body = JsonConvert.SerializeObject(ex) }, ctx.Response);
            }
        }
    }
}
