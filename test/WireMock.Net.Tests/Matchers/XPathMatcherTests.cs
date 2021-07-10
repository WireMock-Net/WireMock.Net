using NFluent;
using WireMock.Matchers;
using Xunit;

namespace WireMock.Net.Tests.Matchers
{
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
            string[] patterns = matcher.GetPatterns();

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
            double result = matcher.IsMatch(xml);

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
            var matcher = new XPathMatcher(MatchBehaviour.RejectOnMatch, false, "/todo-list[count(todo-item) = 1]");

            // Act
            double result = matcher.IsMatch(xml);

            // Assert
            Check.That(result).IsEqualTo(0.0);
        }

        [Fact]
        public void XPathMatcher_Match_Soap()
        {
            // Assign
            string xml = @"
<?xml version='1.0' standalone='no'?>
<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ns=""http://www.Test.nl/XMLHeader/10"" xmlns:req=""http://www.Test.nl/Betalen/COU/Services/RdplDbTknLystByOvkLyst/8/Req"">
   <soapenv:Header>
      <ns:TestHeader>
         <ns:HeaderVersion>10</ns:HeaderVersion>
         <ns:MessageId>MsgId10</ns:MessageId>
         <ns:ServiceRequestorDomain>Betalen</ns:ServiceRequestorDomain>
         <ns:ServiceRequestorId>CRM</ns:ServiceRequestorId>
         <ns:ServiceProviderDomain>COU</ns:ServiceProviderDomain>
         <ns:ServiceId>RdplDbTknLystByOvkLyst</ns:ServiceId>
         <ns:ServiceVersion>8</ns:ServiceVersion>
         <ns:FaultIndication>N</ns:FaultIndication>
         <ns:MessageTimestamp>?</ns:MessageTimestamp>
      </ns:TestHeader>
   </soapenv:Header>
   <soapenv:Body>
      <req:RdplDbTknLystByOvkLyst_REQ>
         <req:AanleveraarCode>CRM</req:AanleveraarCode>
         <!--Optional:-->
         <req:AanleveraarDetail>CRMi</req:AanleveraarDetail>
         <req:BerichtId>BerId</req:BerichtId>
         <req:BerichtType>RdplDbTknLystByOvkLyst</req:BerichtType>
         <!--Optional:-->
         <req:OpgenomenBedragenGewenstIndicatie>N</req:OpgenomenBedragenGewenstIndicatie>
         <req:TokenIdLijst>
            <!--1 to 10 repetitions:-->
            <req:TokenId>0000083256</req:TokenId>
            <req:TokenId>0000083259</req:TokenId>
         </req:TokenIdLijst>
      </req:RdplDbTknLystByOvkLyst_REQ>
   </soapenv:Body>
</soapenv:Envelope>";
            var matcher = new XPathMatcher("//*[local-name()='TokenIdLijst']");

            // Act
            var result = matcher.IsMatch(xml);

            // Assert
            Check.That(result).IsEqualTo(0.0);
        }
    }
}