using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

[module:
    SuppressMessage("StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Justification = "Reviewed. Suppression is OK here, as it conflicts with internal naming rules.")]
[module:
    SuppressMessage("StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Justification = "Reviewed. Suppression is OK here, as it conflicts with internal naming rules.")]
[module:
    SuppressMessage("StyleCop.CSharp.DocumentationRules",
        "SA1633:FileMustHaveHeader",
        Justification = "Reviewed. Suppression is OK here, as unknown copyright and company.")]
// ReSharper disable ArrangeThisQualifier
// ReSharper disable InconsistentNaming
namespace WireMock
{
    /// <summary>
    /// The responses.
    /// </summary>
    public class Responses : IHeadersResponseBuilder
    {
        /// <summary>
        /// The _response.
        /// </summary>
        private readonly Response _response;

        /// <summary>
        /// The _delay.
        /// </summary>
        private TimeSpan _delay = TimeSpan.Zero;

        /// <summary>
        /// Initializes a new instance of the <see cref="Responses"/> class.
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        public Responses(Response response)
        {
            _response = response;
        }

        /// <summary>
        /// The with Success status code.
        /// </summary>
        /// <returns>The <see cref="IHeadersResponseBuilder"/>.</returns>
        public static IHeadersResponseBuilder WithSuccess()
        {
            return WithStatusCode(200);
        }

        /// <summary>
        /// The with NotFound status code.
        /// </summary>
        /// <returns>The <see cref="IHeadersResponseBuilder"/>.</returns>
        public static IHeadersResponseBuilder WithNotFound()
        {
            return WithStatusCode(404);
        }

        /// <summary>
        /// The with status code.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="IHeadersResponseBuilder"/>.
        /// </returns>
        public static IHeadersResponseBuilder WithStatusCode(int code)
        {
            var response = new Response { StatusCode = code };
            return new Responses(response);
        }

        /// <summary>
        /// The provide response.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<Response> ProvideResponse(Request request)
        {
            await Task.Delay(_delay);
            return _response;
        }

        /// <summary>
        /// The with header.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="IHeadersResponseBuilder"/>.
        /// </returns>
        public IHeadersResponseBuilder WithHeader(string name, string value)
        {
            _response.AddHeader(name, value);
            return this;
        }

        /// <summary>
        /// The with body.
        /// </summary>
        /// <param name="body">
        /// The body.
        /// </param>
        /// <returns>
        /// The <see cref="IDelayResponseBuilder"/>.
        /// </returns>
        public IDelayResponseBuilder WithBody(string body)
        {
            _response.Body = body;
            return this;
        }

        /// <summary>
        /// The after delay.
        /// </summary>
        /// <param name="delay">
        /// The delay.
        /// </param>
        /// <returns>
        /// The <see cref="IProvideResponses"/>.
        /// </returns>
        public IProvideResponses AfterDelay(TimeSpan delay)
        {
            _delay = delay;
            return this;
        }
    }
}
