#if MIMEKIT
using System;
using System.Collections.Generic;
using FluentAssertions;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Models;
using WireMock.Types;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.RequestMatchers;

public class RequestMessageMultiPartMatcherTests
{
    private const string TestMultiPart = @"--=-5XgmpXt0XOfzdtcgNJc2ZQ==
Content-Type: text/plain; charset=utf-8

This is some plain text
--=-5XgmpXt0XOfzdtcgNJc2ZQ==
Content-Type: text/json; charset=utf-8

{
        ""Key"": ""Value""
    }
--=-5XgmpXt0XOfzdtcgNJc2ZQ==
Content-Type: image/png; name=image.png
Content-Disposition: attachment; filename=image.png
Content-Transfer-Encoding: base64

iVBORw0KGgoAAAANSUhEUgAAAAIAAAACAgMAAAAP2OW3AAAADFBMVEX/tID/vpH/pWX/sHidUyjl
AAAADElEQVR4XmMQYNgAAADkAMHebX3mAAAAAElFTkSuQmCC

--=-5XgmpXt0XOfzdtcgNJc2ZQ==-- 
";

    [Fact]
    public void RequestMessageBodyMatcher_GetMatchingScore_BodyAsMultiPart()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsString = TestMultiPart,
            DetectedBodyType = BodyType.MultiPart
        };

        var textPlainContentTypeMatcher = new ContentTypeMatcher("text/plain");
        var textPlainContentMatcher = new ExactMatcher("This is some plain text");
        var textPlainMatcher = new MimePartMatcher(MatchBehaviour.AcceptOnMatch, textPlainContentTypeMatcher, null, null, textPlainContentMatcher);

        var partTextJsonContentTypeMatcher = new ContentTypeMatcher("text/json");
        var partTextJsonContentMatcher = new JsonMatcher(new { Key = "Value" }, true);
        var partTextMatcher = new MimePartMatcher(MatchBehaviour.AcceptOnMatch, partTextJsonContentTypeMatcher, null, null, partTextJsonContentMatcher);

        var imagePngContentTypeMatcher = new ContentTypeMatcher("image/png");
        var imagePngContentDispositionMatcher = new ExactMatcher("attachment; filename=\"image.png\"");
        var imagePngContentTransferEncodingMatcher = new ExactMatcher("base64");
        var imagePngContentMatcher = new ExactObjectMatcher(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAIAAAACAgMAAAAP2OW3AAAADFBMVEX/tID/vpH/pWX/sHidUyjlAAAADElEQVR4XmMQYNgAAADkAMHebX3mAAAAAElFTkSuQmCC"));
        var imagePngMatcher = new MimePartMatcher(MatchBehaviour.AcceptOnMatch, imagePngContentTypeMatcher, imagePngContentDispositionMatcher, imagePngContentTransferEncodingMatcher, imagePngContentMatcher);

        var matchers = new IMatcher[]
        {
            textPlainMatcher,
            partTextMatcher,
            imagePngMatcher
        };

        var headers = new Dictionary<string, string[]>
        {
            { "Content-Type", new[] { @"multipart/mixed; boundary=""=-5XgmpXt0XOfzdtcgNJc2ZQ==""" } }
        };
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", body, headers);

        var matcher = new RequestMessageMultiPartMatcher(matchers);

        // Act
        var result = new RequestMatchResult();
        var score = matcher.GetMatchingScore(requestMessage, result);

        // Assert
        score.Should().Be(MatchScores.Perfect);
    }
}
#endif