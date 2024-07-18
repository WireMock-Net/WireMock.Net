// Copyright © WireMock.Net

using FluentAssertions.Primitives;
using WireMock.Server;

// ReSharper disable once CheckNamespace
namespace WireMock.FluentAssertions;

/// <summary>
/// Contains a number of methods to assert that the <see cref="IWireMockServer"/> is in the expected state.
/// </summary>
public class WireMockReceivedAssertions : ReferenceTypeAssertions<IWireMockServer, WireMockReceivedAssertions>
{
    /// <summary>
    /// Create a WireMockReceivedAssertions. 
    /// </summary>
    /// <param name="server">The <see cref="IWireMockServer"/>.</param>
    public WireMockReceivedAssertions(IWireMockServer server) : base(server)
    {
    }

    /// <summary>
    /// Asserts if <see cref="IWireMockServer"/> has received no calls.
    /// </summary>
    /// <returns><see cref="WireMockAssertions"/></returns>
    public WireMockAssertions HaveReceivedNoCalls()
    {
        return new WireMockAssertions(Subject, 0);
    }

    /// <summary>
    /// Asserts if <see cref="IWireMockServer"/> has received a call.
    /// </summary>
    /// <returns><see cref="WireMockAssertions"/></returns>
    public WireMockAssertions HaveReceivedACall()
    {
        return new WireMockAssertions(Subject, null);
    }

    /// <summary>
    /// Asserts if <see cref="IWireMockServer"/> has received n-calls.
    /// </summary>
    /// <param name="callsCount"></param>
    /// <returns><see cref="WireMockANumberOfCallsAssertions"/></returns>
    public WireMockANumberOfCallsAssertions HaveReceived(int callsCount)
    {
        return new WireMockANumberOfCallsAssertions(Subject, callsCount);
    }

    /// <inheritdoc />
    protected override string Identifier => "wiremockserver";
}