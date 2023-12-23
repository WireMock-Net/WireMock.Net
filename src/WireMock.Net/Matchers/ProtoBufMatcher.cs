#if PROTOBUF
using System;
using ProtoBufJsonConverter;
using ProtoBufJsonConverter.Models;
using Stef.Validation;

namespace WireMock.Matchers;

/// <summary>
/// Grpc ProtoBuf Matcher
/// </summary>
/// <inheritdoc cref="IObjectMatcher"/>
public class ProtoBufMatcher : IBytesMatcher
{
    /// <inheritdoc />
    public string Name => nameof(ProtoBufMatcher);

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <summary>
    /// The proto definition as a string.
    /// </summary>
    public string ProtoDefinition { get; }

    /// <summary>
    /// The method which is called on service. Format is "{package-name}.{service-name}-{method-name}".
    /// </summary>
    public string GrpcServiceMethod { get; }

    /// <summary>
    /// The JsonMatcher to use (optional).
    /// </summary>
    public IJsonMatcher? JsonMatcher { get; }

    private static readonly IConverter ProtoBufToJsonConverter = new Converter();

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphQLMatcher"/> class.
    /// </summary>
    /// <param name="protoDefinition">The proto definition as a string.</param>
    /// <param name="method">The method which is called on service. Format is {package-name}.{service-name}-{method-name}</param>
    /// <param name="matchBehaviour">The match behaviour. (default = "AcceptOnMatch")</param>
    /// <param name="jsonMatcher">The optional jsonMatcher to use to match the ProtoBuf as (json) object.</param>
    public ProtoBufMatcher(
        string protoDefinition,
        string method,
        MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch,
        IJsonMatcher? jsonMatcher = null
    )
    {
        ProtoDefinition = Guard.NotNullOrWhiteSpace(protoDefinition);
        GrpcServiceMethod = Guard.NotNullOrWhiteSpace(method);
        JsonMatcher = jsonMatcher;
        MatchBehaviour = matchBehaviour;
    }

    /// <inheritdoc />
    public MatchResult IsMatch(byte[]? input)
    {
        var result = new MatchResult();

        if (input != null)
        {
            var request = new ConvertToObjectRequest(ProtoDefinition, GrpcServiceMethod, input);

            try
            {
                var instance = ProtoBufToJsonConverter.ConvertToObject(request);

                result = JsonMatcher?.IsMatch(instance) ?? new MatchResult(MatchScores.Perfect);
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