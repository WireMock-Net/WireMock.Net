using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
#if !USE_ASPNETCORE
using Microsoft.Owin;
using IContext = Microsoft.Owin.IOwinContext;
using OwinMiddleware = Microsoft.Owin.OwinMiddleware;
using Next = Microsoft.Owin.OwinMiddleware;
#else
using OwinMiddleware = System.Object;
using IContext = Microsoft.AspNetCore.Http.HttpContext;
using Next = Microsoft.AspNetCore.Http.RequestDelegate;
#endif
using WireMock.Owin.Mappers;
using Stef.Validation;

namespace WireMock.Owin
{
    internal class GlobalExceptionMiddleware : OwinMiddleware
    {
        private readonly IWireMockMiddlewareOptions _options;
        private readonly IOwinResponseMapper _responseMapper;

#if !USE_ASPNETCORE
        public GlobalExceptionMiddleware(Next next, IWireMockMiddlewareOptions options, IOwinResponseMapper responseMapper) : base(next)
        {
            Guard.NotNull(options, nameof(options));
            Guard.NotNull(responseMapper, nameof(responseMapper));

            _options = options;
            _responseMapper = responseMapper;
        }
#else
        public GlobalExceptionMiddleware(Next next, IWireMockMiddlewareOptions options, IOwinResponseMapper responseMapper)
        {
            Guard.NotNull(options, nameof(options));
            Guard.NotNull(responseMapper, nameof(responseMapper));

            Next = next;
            _options = options;
            _responseMapper = responseMapper;
        }
#endif

#if USE_ASPNETCORE
        public Next Next { get; }
#endif

#if !USE_ASPNETCORE
        public override Task Invoke(IContext ctx)
#else
        public Task Invoke(IContext ctx)
#endif
        {
            return InvokeInternalAsync(ctx);
        }

        private async Task InvokeInternalAsync(IContext ctx)
        {
            try
            {
                if (Next != null)
                {
                    await Next.Invoke(ctx).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _options.Logger.Error("HttpStatusCode set to 500 {0}", ex);
                await _responseMapper.MapAsync(ResponseMessageBuilder.Create(JsonConvert.SerializeObject(ex), 500), ctx.Response).ConfigureAwait(false);
            }
        }
    }
}
