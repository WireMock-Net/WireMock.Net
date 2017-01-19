using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
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
namespace WireMock
{
    /// <summary>
    /// The request body spec.
    /// </summary>
    public class RequestBodySpec : ISpecifyRequests
    {
        /// <summary>
        /// The bodyRegex.
        /// </summary>
        private readonly byte[] bodyData;

        /// <summary>
        /// The bodyRegex.
        /// </summary>
        private readonly Regex bodyRegex;

        /// <summary>
        /// The body function
        /// </summary>
        private readonly Func<string, bool> bodyFunc;

        /// <summary>
        /// The body data function
        /// </summary>
        private readonly Func<byte[], bool> bodyDataFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestBodySpec"/> class.
        /// </summary>
        /// <param name="body">
        /// The body Regex pattern.
        /// </param>
        public RequestBodySpec([NotNull, RegexPattern] string body)
        {
            Check.NotNull(body, nameof(body));
            bodyRegex = new Regex(body);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestBodySpec"/> class.
        /// </summary>
        /// <param name="body">
        /// The body Regex pattern.
        /// </param>
        public RequestBodySpec([NotNull] byte[] body)
        {
            Check.NotNull(body, nameof(body));
            bodyData = body;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestBodySpec"/> class.
        /// </summary>
        /// <param name="func">
        /// The body func.
        /// </param>
        public RequestBodySpec([NotNull] Func<string, bool> func)
        {
            Check.NotNull(func, nameof(func));
            bodyFunc = func;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestBodySpec"/> class.
        /// </summary>
        /// <param name="func">
        /// The body func.
        /// </param>
        public RequestBodySpec([NotNull] Func<byte[], bool> func)
        {
            Check.NotNull(func, nameof(func));
            bodyDataFunc = func;
        }

        /// <summary>
        /// The is satisfied by.
        /// </summary>
        /// <param name="requestMessage">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsSatisfiedBy(RequestMessage requestMessage)
        {
            if (bodyRegex != null)
                return bodyRegex.IsMatch(requestMessage.BodyAsString);

            if (bodyData != null)
                return requestMessage.Body == bodyData;

            if (bodyFunc != null)
                return bodyFunc(requestMessage.BodyAsString);

            if (bodyDataFunc != null)
                return bodyDataFunc(requestMessage.Body);

            return false;
        }
    }
}