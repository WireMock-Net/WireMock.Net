// Copyright © WireMock.Net

#if PROTOBUF
using System;
using System.Threading;
using System.Threading.Tasks;
using ProtoBufJsonConverter;
using ProtoBufJsonConverter.Models;
using Stef.Validation;
using WireMock.Models;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// Grpc ProtoBuf Matcher
/// </summary>
/// <inheritdoc cref="IProtoBufMatcher"/>
public class ProtoBufMatcher : IProtoBufMatcher
{
    /// <inheritdoc />
    public string Name => nameof(ProtoBufMatcher);

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <summary>
    /// The Func to define The proto definition as text.
    /// </summary>
    public Func<IdOrText> ProtoDefinition { get; }

    /// <summary>
    /// The full type of the protobuf (request/response) message object. Format is "{package-name}.{type-name}".
    /// </summary>
    public string MessageType { get; }

    /// <summary>
    /// The Matcher to use (optional).
    /// </summary>
    public IObjectMatcher? Matcher { get; }

    private static readonly Converter ProtoBufToJsonConverter = SingletonFactory<Converter>.GetInstance();

    /// <summary>
    /// Initializes a new instance of the <see cref="ProtoBufMatcher"/> class.
    /// </summary>
    /// <param name="protoDefinition">The proto definition.</param>
    /// <param name="messageType">The full type of the protobuf (request/response) message object. Format is "{package-name}.{type-name}".</param>
    /// <param name="matchBehaviour">The match behaviour. (default = "AcceptOnMatch")</param>
    /// <param name="matcher">The optional jsonMatcher to use to match the ProtoBuf as (json) object.</param>
    public ProtoBufMatcher(
        Func<IdOrText> protoDefinition,
        string messageType,
        MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch,
        IObjectMatcher? matcher = null
    )
    {
        ProtoDefinition = Guard.NotNull(protoDefinition);
        MessageType = Guard.NotNullOrWhiteSpace(messageType);
        Matcher = matcher;
        MatchBehaviour = matchBehaviour;
    }

    /// <inheritdoc />
    public async Task<MatchResult> IsMatchAsync(byte[]? input, CancellationToken cancellationToken = default)
    {
        var result = new MatchResult();

        if (input != null)
        {
            try
            {
                var instance = await DecodeAsync(input, true, cancellationToken).ConfigureAwait(false);

                result = Matcher?.IsMatch(instance) ?? new MatchResult(MatchScores.Perfect);
            }
            catch (Exception e)
            {
                result = new MatchResult(MatchScores.Mismatch, e);
            }
        }

        return MatchBehaviourHelper.Convert(MatchBehaviour, result);
    }

    /// <inheritdoc />
    public Task<object?> DecodeAsync(byte[]? input, CancellationToken cancellationToken = default)
    {
        return DecodeAsync(input, false, cancellationToken);
    }

    /// <inheritdoc />
    public string GetCSharpCodeArguments()
    {
        return "NotImplemented";
    }

    private async Task<object?> DecodeAsync(byte[]? input, bool throwException, CancellationToken cancellationToken)
    {
        if (input == null)
        {
            return null;
        }

        var request = new ConvertToObjectRequest(ProtoDefinition().Text, MessageType, input);

        try
        {
            return await ProtoBufToJsonConverter.ConvertAsync(request, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            if (throwException)
            {
                throw;
            }

            return null;
        }
    }
}
#endif