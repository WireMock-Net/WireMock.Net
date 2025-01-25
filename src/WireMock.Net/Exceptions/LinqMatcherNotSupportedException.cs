// Copyright © WireMock.Net

namespace WireMock.Exceptions;

/// <summary>
/// LinqMatcherException
/// </summary>
/// <seealso cref="WireMockException" />
public class LinqMatcherNotSupportedException : WireMockException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LinqMatcherNotSupportedException"/> class.
    /// </summary>
    public LinqMatcherNotSupportedException() : base("It's not allowed to use the 'LinqMatcher' because WireMockServerSettings.AllowDynamicLinq is not set to 'true'.")
    {
    }
}