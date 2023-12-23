#if PROTOBUF
using System;
using System.Linq;
using ProtoBufJsonConverter;
using ProtoBufJsonConverter.Models;
using Stef.Validation;

namespace WireMock.Matchers;

/// <summary>
/// GrpcMatcher ProtoBuf Matcher
/// </summary>
/// <inheritdoc cref="IObjectMatcher"/>
public class ProtoBufMatcher : IBytesMatcher
{
    /// <inheritdoc />
    public string Name => nameof(ProtoBufMatcher);

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <summary>
    /// The MatchOperator
    /// </summary>
    public MatchOperator MatchOperator { get; }


    private readonly string _protoDefinition;
    private readonly string _method;
    private readonly IStringMatcher[] _matchers;

    private static readonly IConverter ProtoBufToJsonConverter = new Converter();

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphQLMatcher"/> class.
    /// </summary>
    /// <param name="protoDefinition">The proto definition as a string.</param>
    /// <param name="method">The method which is called on service. Format is {package-name}.{service-name}-{method-name}</param>
    /// <param name="matchBehaviour">The match behaviour. (default = "AcceptOnMatch")</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use. (default = "Or")</param>
    /// <param name="matchers">The matchers to use to match the ProtoBuf as JSON string.</param>
    public ProtoBufMatcher(
        string protoDefinition,
        string method,
        MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch,
        MatchOperator matchOperator = MatchOperator.Or,
        params IStringMatcher[] matchers
    )
    {
        _protoDefinition = Guard.NotNullOrWhiteSpace(protoDefinition);
        _method = Guard.NotNullOrWhiteSpace(method);
        _matchers = Guard.NotNull(matchers);
        MatchBehaviour = matchBehaviour;
        MatchOperator = matchOperator;
    }

    /// <inheritdoc />
    public MatchResult IsMatch(byte[]? input)
    {
        var result = new MatchResult();

        if (input != null)
        {
            var request = new ConvertToJsonRequest(_protoDefinition, _method, input);

            try
            {
                var json = ProtoBufToJsonConverter.ConvertToJson(request);

                // If no matchers are defined, just check if the ConvertToJson is fine.
                if (_matchers.Length == 0)
                {
                    result = new MatchResult(MatchScores.Perfect);
                }
                else
                {
                    var results = _matchers.Select(m => m.IsMatch(json)).ToArray();

                    result = MatchResult.From(results, MatchOperator);
                }
            }
            catch (Exception e)
            {
                result = new MatchResult(MatchScores.Mismatch, e);
            }
        }

        return MatchBehaviourHelper.Convert(MatchBehaviour, result);
    }
}
#endif