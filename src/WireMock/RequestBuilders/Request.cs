using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

[module:
    SuppressMessage("StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Justification = "Reviewed. Suppression is OK here, as it conflicts with internal naming rules.")]
[module:
    SuppressMessage("StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
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
namespace WireMock.RequestBuilders
{
    /// <summary>
    /// The requests.
    /// </summary>
    public class Request : CompositeRequestSpec, IVerbRequestBuilder, IHeadersRequestBuilder, IParamsRequestBuilder
    {
        /// <summary>
        /// The _request specs.
        /// </summary>
        private readonly IList<ISpecifyRequests> _requestSpecs;

        /// <summary>
        /// Initializes a new instance of the <see cref="Request"/> class.
        /// </summary>
        /// <param name="requestSpecs">
        /// The request specs.
        /// </param>
        private Request(IList<ISpecifyRequests> requestSpecs) : base(requestSpecs)
        {
            _requestSpecs = requestSpecs;
        }

        /// <summary>
        /// The with url.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// The <see cref="IVerbRequestBuilder"/>.
        /// </returns>
        public static IVerbRequestBuilder WithUrl(string url)
        {
            var specs = new List<ISpecifyRequests>();
            var requests = new Request(specs);
            specs.Add(new RequestUrlSpec(url));
            return requests;
        }

        /// <summary>
        /// The with path.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="IVerbRequestBuilder"/>.
        /// </returns>
        public static IVerbRequestBuilder WithPath(string path)
        {
            var specs = new List<ISpecifyRequests>();
            var requests = new Request(specs);
            specs.Add(new RequestPathSpec(path));
            return requests;
        }

        /// <summary>
        /// The using get.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersRequestBuilder"/>.
        /// </returns>
        public IHeadersRequestBuilder UsingGet()
        {
            _requestSpecs.Add(new RequestVerbSpec("get"));
            return this;
        }

        /// <summary>
        /// The using post.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersRequestBuilder"/>.
        /// </returns>
        public IHeadersRequestBuilder UsingPost()
        {
            _requestSpecs.Add(new RequestVerbSpec("post"));
            return this;
        }

        /// <summary>
        /// The using put.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersRequestBuilder"/>.
        /// </returns>
        public IHeadersRequestBuilder UsingPut()
        {
            _requestSpecs.Add(new RequestVerbSpec("put"));
            return this;
        }

        /// <summary>
        /// The using head.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersRequestBuilder"/>.
        /// </returns>
        public IHeadersRequestBuilder UsingHead()
        {
            _requestSpecs.Add(new RequestVerbSpec("head"));
            return this;
        }

        /// <summary>
        /// The using any verb.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersRequestBuilder"/>.
        /// </returns>
        public IHeadersRequestBuilder UsingAnyVerb()
        {
            return this;
        }

        /// <summary>
        /// The using verb.
        /// </summary>
        /// <param name="verb">
        /// The verb.
        /// </param>
        /// <returns>
        /// The <see cref="IHeadersRequestBuilder"/>.
        /// </returns>
        public IHeadersRequestBuilder UsingVerb(string verb)
        {
            _requestSpecs.Add(new RequestVerbSpec(verb));
            return this;
        }

        /// <summary>
        /// The with body.
        /// </summary>
        /// <param name="body">
        /// The body.
        /// </param>
        /// <returns>
        /// The <see cref="ISpecifyRequests"/>.
        /// </returns>
        public ISpecifyRequests WithBody(string body)
        {
            _requestSpecs.Add(new RequestBodySpec(body));
            return this;
        }

        /// <summary>
        /// The with parameters.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// The <see cref="ISpecifyRequests"/>.
        /// </returns>
        public ISpecifyRequests WithParam(string key, params string[] values)
        {
            _requestSpecs.Add(new RequestParamSpec(key, values.ToList()));
            return this;
        }

        /// <summary>
        /// The with header.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="ignoreCase">ignore Case</param>
        /// <returns>
        /// The <see cref="IHeadersRequestBuilder"/>.
        /// </returns>
        public IHeadersRequestBuilder WithHeader(string name, string value, bool ignoreCase = true)
        {
            _requestSpecs.Add(new RequestHeaderSpec(name, value, ignoreCase));
            return this;
        }
    }
}
