using System;
using WireMock.Models;

namespace WireMock.Matchers;

/// <summary>
/// IProtoBufMatcher
/// </summary>
public interface IProtoBufMatcher : IDecodeBytesMatcher, IBytesMatcher
{
    /// <summary>
    /// The Func to define the proto definition as text or id.
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
}