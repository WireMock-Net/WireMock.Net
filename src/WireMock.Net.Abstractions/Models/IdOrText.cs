// Copyright © WireMock.Net

namespace WireMock.Models;

/// <summary>
/// A structure defining an (optional) Id and a Text.
/// </summary>
public readonly struct IdOrText
{
    /// <summary>
    /// The Id [optional].
    /// </summary>
    public string? Id { get; }

    /// <summary>
    /// The Text.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// When Id is defined, return the Id, else the Text.
    /// </summary>
    public string Value => Id ?? Text;

    /// <summary>
    /// Create a IdOrText
    /// </summary>
    /// <param name="id">The Id [optional]</param>
    /// <param name="text">The Text.</param>
    public IdOrText(string? id, string text)
    {
        Id = id;
        Text = text;
    }
}