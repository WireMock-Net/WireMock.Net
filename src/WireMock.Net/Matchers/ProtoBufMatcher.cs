#if PROTOBUF
using System;
using ProtoBufJsonConverter;
using ProtoBufJsonConverter.Models;
using Stef.Validation;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// Grpc ProtoBuf Matcher
/// </summary>
/// <inheritdoc cref="IBytesMatcher"/>
public class ProtoBufMatcher : IBytesMatcher
{
    /// <inheritdoc />
    public string Name => nameof(ProtoBufMatcher);

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <summary>
    /// The proto definition.
    /// </summary>
    public string ProtoDefinition { get; }

    /// <summary>
    /// The method which is called on service. Format is "{package-name}.{service-name}-{method-name}".
    /// </summary>
    public string MessageType { get; }

    /// <summary>
    /// The JsonMatcher to use (optional).
    /// </summary>
    public IJsonMatcher? JsonMatcher { get; }

    private static readonly IConverter ProtoBufToJsonConverter = SingletonFactory<Converter>.GetInstance();

    /// <summary>
    /// Initializes a new instance of the <see cref="ProtoBufMatcher"/> class.
    /// </summary>
    /// <param name="protoDefinition">The proto definition.</param>
    /// <param name="messageType">The full type of the protobuf (request/response) message object. Format is "{package-name}.{type-name}".</param>
    /// <param name="matchBehaviour">The match behaviour. (default = "AcceptOnMatch")</param>
    /// <param name="jsonMatcher">The optional jsonMatcher to use to match the ProtoBuf as (json) object.</param>
    public ProtoBufMatcher(
        string protoDefinition,
        string messageType,
        MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch,
        IJsonMatcher? jsonMatcher = null
    )
    {
        ProtoDefinition = Guard.NotNullOrWhiteSpace(protoDefinition);
        MessageType = Guard.NotNullOrWhiteSpace(messageType);
        JsonMatcher = jsonMatcher;
        MatchBehaviour = matchBehaviour;
    }

    /// <inheritdoc />
    public MatchResult IsMatch(byte[]? input)
    {
        var result = new MatchResult();

        if (input != null)
        {
            var request = new ConvertToObjectRequest(ProtoDefinition, MessageType, input);

            try
            {
                var instance = ProtoBufToJsonConverter.Convert(request);

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