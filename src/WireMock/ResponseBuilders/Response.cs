using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using HandlebarsDotNet;
using JetBrains.Annotations;
using WireMock.Validation;

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
namespace WireMock.ResponseBuilders
{
    /// <summary>
    /// The responses.
    /// </summary>
    public class Response : IResponseBuilder
    {
        /// <summary>
        /// The _response.
        /// </summary>
        private readonly ResponseMessage _responseMessage;

        /// <summary>
        /// The _delay.
        /// </summary>
        private TimeSpan _delay = TimeSpan.Zero;

        private bool _useTransformer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        /// <param name="responseMessage">
        /// The response.
        /// </param>
        public Response(ResponseMessage responseMessage)
        {
            _responseMessage = responseMessage;
        }

        /// <summary>
        /// The with Success status code.
        /// </summary>
        /// <returns>The <see cref="IResponseBuilder"/>.</returns>
        public static IResponseBuilder WithSuccess()
        {
            return WithStatusCode(200);
        }

        /// <summary>
        /// The with NotFound status code.
        /// </summary>
        /// <returns>The <see cref="IResponseBuilder"/>.</returns>
        public static IResponseBuilder WithNotFound()
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
        /// The <see cref="IResponseBuilder"/>.
        /// </returns>
        public static IResponseBuilder WithStatusCode(int code)
        {
            var response = new ResponseMessage { StatusCode = code };
            return new Response(response);
        }

        /// <summary>
        /// The provide response.
        /// </summary>
        /// <param name="requestMessage">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<ResponseMessage> ProvideResponse(RequestMessage requestMessage)
        {
            if (_useTransformer)
            {
                var template = new { request = requestMessage };

                // Body
                var templateBody = Handlebars.Compile(_responseMessage.Body);
                _responseMessage.Body = templateBody(template);

                // Headers
                var newHeaders = new Dictionary<string, string>();
                foreach (var header in _responseMessage.Headers)
                {
                    var templateHeaderKey = Handlebars.Compile(header.Key);
                    var templateHeaderValue = Handlebars.Compile(header.Value);

                    newHeaders.Add(templateHeaderKey(template), templateHeaderValue(template));
                }
                _responseMessage.Headers = newHeaders;
            }

            await Task.Delay(_delay);

            return _responseMessage;
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
        /// The <see cref="IResponseBuilder"/>.
        /// </returns>
        public IResponseBuilder WithHeader(string name, string value)
        {
            _responseMessage.AddHeader(name, value);
            return this;
        }

        /// <summary>
        /// The with body.
        /// </summary>
        /// <param name="body">
        /// The body.
        /// </param>
        /// <returns>
        /// The <see cref="IResponseBuilder"/>.
        /// </returns>
        public IResponseBuilder WithBody(string body)
        {
            Check.NotNull(body, nameof(body));

            _responseMessage.Body = body;
            return this;
        }

        /// <summary>
        /// The with body as base64.
        /// </summary>
        /// <param name="bodyAsbase64">The body asbase64.</param>
        /// <param name="encoding"></param>
        /// <returns>A <see cref="IResponseBuilder"/>.</returns>
        public IResponseBuilder WithBodyAsBase64(string bodyAsbase64, Encoding encoding = null)
        {
            Check.NotNull(bodyAsbase64, nameof(bodyAsbase64));

            _responseMessage.Body = (encoding ?? Encoding.UTF8).GetString(Convert.FromBase64String(bodyAsbase64));
            return this;
        }

        /// <summary>
        /// The with transformer.
        /// </summary>
        /// <returns>
        /// The <see cref="IResponseBuilder"/>.
        /// </returns>
        public IResponseBuilder WithTransformer()
        {
            _useTransformer = true;
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
        public IResponseBuilder AfterDelay(TimeSpan delay)
        {
            _delay = delay;
            return this;
        }
    }
}
