using System.Threading.Tasks;
#if !USE_ASPNETCORE
using IResponse = Microsoft.Owin.IOwinResponse;
#else
using IResponse = Microsoft.AspNetCore.Http.HttpResponse;
#endif

namespace WireMock.Owin.Mappers
{
    /// <summary>
    /// IOwinResponseMapper
    /// </summary>
    internal interface IOwinResponseMapper
    {
        /// <summary>
        /// Map ResponseMessage to IResponse.
        /// </summary>
        /// <param name="responseMessage">The ResponseMessage</param>
        /// <param name="response">The OwinResponse/HttpResponse</param>
        /// <param name="options">The WireMockMiddlewareOptions</param>
        Task MapAsync(ResponseMessage responseMessage, IResponse response, IWireMockMiddlewareOptions options);
    }
}