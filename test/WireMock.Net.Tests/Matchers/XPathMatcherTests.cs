// Copyright © WireMock.Net

using NFluent;
using WireMock.Admin.Mappings;
using WireMock.Matchers;
using Xunit;

namespace WireMock.Net.Tests.Matchers;

public class XPathMatcherTests
{
    [Fact]
    public void XPathMatcher_GetName()
    {
        // Assign
        var matcher = new XPathMatcher("X");

        // Act
        string name = matcher.Name;

        // Assert
        Check.That(name).Equals("XPathMatcher");
    }

    [Fact]
    public void XPathMatcher_GetPatterns()
    {
        // Assign
        var matcher = new XPathMatcher("X");

        // Act
        var patterns = matcher.GetPatterns();

        // Assert
        Check.That(patterns).ContainsExactly("X");
    }

    [Fact]
    public void XPathMatcher_IsMatch_AcceptOnMatch()
    {
        // Assign
        string xml = @"
                    <todo-list>
                        <todo-item id='a1'>abc</todo-item>
                    </todo-list>";
        var matcher = new XPathMatcher("/todo-list[count(todo-item) = 1]");

        // Act
        double result = matcher.IsMatch(xml).Score;

        // Assert
        Check.That(result).IsEqualTo(1.0);
    }

    [Fact]
    public void XPathMatcher_IsMatch_WithNamespaces_AcceptOnMatch()
    {
        // Assign
        string input =
            @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
                  <s:Body>
                      <QueryRequest xmlns=""urn://MyWcfService"">
                          <MaxResults i:nil=""true"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance""/>
                          <Restriction i:nil=""true"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance""/>
                          <SearchMode i:nil=""true"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance""/>
                      </QueryRequest>
                  </s:Body>
              </s:Envelope>";
        var xmlNamespaces = new[]
        {
            new XmlNamespace { Prefix = "s", Uri = "http://schemas.xmlsoap.org/soap/envelope/" },
            new XmlNamespace { Prefix = "i", Uri = "http://www.w3.org/2001/XMLSchema-instance" }
        };
        var matcher = new XPathMatcher(
            MatchBehaviour.AcceptOnMatch,
            MatchOperator.Or,
            xmlNamespaces,
            "/s:Envelope/s:Body/*[local-name()='QueryRequest' and namespace-uri()='urn://MyWcfService']");
        
        // Act
        double result = matcher.IsMatch(input).Score;

        // Assert
        Check.That(result).IsEqualTo(1.0);
    }

    [Fact]
    public void XPathMatcher_IsMatch_WithNamespaces_OneSelfDefined_AcceptOnMatch()
    {
        // Assign
        string input =
            @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
                  <s:Body>
                      <QueryRequest xmlns=""urn://MyWcfService"">
                          <MaxResults i:nil=""true"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance""/>
                          <Restriction i:nil=""true"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance""/>
                          <SearchMode i:nil=""true"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance""/>
                      </QueryRequest>
                  </s:Body>
              </s:Envelope>";
        var xmlNamespaces = new[]
        {
            new XmlNamespace { Prefix = "s", Uri = "http://schemas.xmlsoap.org/soap/envelope/" },
            new XmlNamespace { Prefix = "i", Uri = "http://www.w3.org/2001/XMLSchema-instance" },
            new XmlNamespace { Prefix = "q", Uri = "urn://MyWcfService" }
        };
        var matcher = new XPathMatcher(
            MatchBehaviour.AcceptOnMatch,
            MatchOperator.Or,
            xmlNamespaces,
            "/s:Envelope/s:Body/q:QueryRequest");

        // Act
        double result = matcher.IsMatch(input).Score;

        // Assert
        Check.That(result).IsEqualTo(1.0);
    }

    [Fact]
    public void XPathMatcher_IsMatch_RejectOnMatch()
    {
        // Assign
        string xml = @"
                    <todo-list>
                        <todo-item id='a1'>abc</todo-item>
                    </todo-list>";
        var matcher = new XPathMatcher(MatchBehaviour.RejectOnMatch, MatchOperator.Or, null, "/todo-list[count(todo-item) = 1]");

        // Act
        double result = matcher.IsMatch(xml).Score;

        // Assert
        Check.That(result).IsEqualTo(0.0);
    }
}