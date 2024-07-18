// Copyright © WireMock.Net

#if MIMEKIT
using System;
using System.Collections.Generic;
using System.Linq;
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

    private static readonly MimePartMatcher PerfectTextPlainMatcher = new(
        MatchBehaviour.AcceptOnMatch,
        new ContentTypeMatcher("text/plain"),
        null,
        null,
        new ExactMatcher("This is some plain text")
    );

    private static readonly MimePartMatcher PerfectPartTextMatcher = new(
        MatchBehaviour.AcceptOnMatch,
        new ContentTypeMatcher("text/json"),
        null,
        null,
        new JsonMatcher(new { Key = "Value" }, true)
    );

    private static readonly MimePartMatcher PerfectImagePngMatcher = new(
        MatchBehaviour.AcceptOnMatch,
         new ContentTypeMatcher("image/png"),
         new ExactMatcher("attachment; filename=\"image.png\""),
         new ExactMatcher("base64"),
         new ExactObjectMatcher(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAIAAAACAgMAAAAP2OW3AAAADFBMVEX/tID/vpH/pWX/sHidUyjlAAAADElEQVR4XmMQYNgAAADkAMHebX3mAAAAAElFTkSuQmCC"))
    );

    private static readonly MimePartMatcher MismatchTextPlainMatcher = new(
        MatchBehaviour.AcceptOnMatch,
        new ContentTypeMatcher("text/plain"),
        null,
        null,
        new ExactMatcher("--INVALID TEXT--")
    );

    private static readonly MimePartMatcher MismatchPartTextMatcher = new(
        MatchBehaviour.AcceptOnMatch,
        new ContentTypeMatcher("text/json"),
        null,
        null,
        new JsonMatcher(new { Key = "InvalidValue" }, true)
    );

    private static readonly MimePartMatcher MismatchImagePngMatcher = new(
        MatchBehaviour.AcceptOnMatch,
         new ContentTypeMatcher("image/png"),
         new ExactMatcher("attachment; filename=\"invalid.png\""),
         new ExactMatcher("base64"),
         null
    );

    public static TheoryData<IMatcher?, IMatcher?, IMatcher?> PerfectMatchersData => new()
    {
       { PerfectTextPlainMatcher, null, null },
       { PerfectPartTextMatcher, null, null },
       { PerfectImagePngMatcher, null, null },
       { PerfectPartTextMatcher, PerfectTextPlainMatcher, null },
       { PerfectPartTextMatcher, PerfectImagePngMatcher, PerfectTextPlainMatcher },
    };

    public static TheoryData<IMatcher?, IMatcher?, IMatcher?> MismatchMatchersData => new()
    {
       { MismatchTextPlainMatcher, null, null },
       { MismatchPartTextMatcher, null, null },
       { MismatchImagePngMatcher, null, null },
       { MismatchPartTextMatcher, MismatchTextPlainMatcher, null },
       { MismatchPartTextMatcher, MismatchImagePngMatcher, MismatchTextPlainMatcher },
    };

    public static TheoryData<IMatcher?, IMatcher?, IMatcher?> MixedMatchersData => new()
    {
       { MismatchPartTextMatcher, PerfectTextPlainMatcher, null },
       { PerfectPartTextMatcher, MismatchImagePngMatcher, null },
    };

    [Theory]
    [MemberData(nameof(PerfectMatchersData))]
    public void RequestMessageBodyMatcher_GetMatchingScore_BodyAsMultiPart_Perfect(IMatcher? matcher1, IMatcher? matcher2, IMatcher? matcher3)
    {
        // Assign
        var score = GetScore(matcher1, matcher2, matcher3);

        // Assert
        score.Should().Be(MatchScores.Perfect);
    }

    [Theory]
    [MemberData(nameof(MismatchMatchersData))]
    public void RequestMessageBodyMatcher_GetMatchingScore_BodyAsMultiPart_Mismatch(IMatcher? matcher1, IMatcher? matcher2, IMatcher? matcher3)
    {
        // Assign
        var score = GetScore(matcher1, matcher2, matcher3);

        // Assert
        score.Should().Be(MatchScores.Mismatch);
    }

    [Theory]
    [MemberData(nameof(MixedMatchersData))]
    public void RequestMessageBodyMatcher_GetMatchingScore_BodyAsMultiPart_Mixed_With_And(IMatcher? matcher1, IMatcher? matcher2, IMatcher? matcher3)
    {
        // Assign
        var score = GetScore(matcher1, matcher2, matcher3);

        // Assert
        score.Should().Be(MatchScores.Mismatch);
    }

    [Theory]
    [MemberData(nameof(MixedMatchersData))]
    public void RequestMessageBodyMatcher_GetMatchingScore_BodyAsMultiPart_Mixed_With_Or(IMatcher? matcher1, IMatcher? matcher2, IMatcher? matcher3)
    {
        // Assign
        var score = GetScore(matcher1, matcher2, matcher3, MatchOperator.Or);

        // Assert
        score.Should().Be(MatchScores.Perfect);
    }

    private static double GetScore(IMatcher? matcher1, IMatcher? matcher2, IMatcher? matcher3, MatchOperator matchOperator = MatchOperator.And)
    {
        // Assign
        var body = new BodyData
        {
            BodyAsString = TestMultiPart,
            DetectedBodyType = BodyType.MultiPart
        };

        var headers = new Dictionary<string, string[]>
        {
            { "Content-Type", new[] { @"multipart/mixed; boundary=""=-5XgmpXt0XOfzdtcgNJc2ZQ==""" } }
        };
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", body, headers);
        var matchers = new IMatcher?[] { matcher1, matcher2, matcher3 }
        .Where(m => m is not null)
        .ToArray();

        var matcher = new RequestMessageMultiPartMatcher(MatchBehaviour.AcceptOnMatch, matchOperator, matchers!);

        // Act
        var result = new RequestMatchResult();
        return matcher.GetMatchingScore(requestMessage, result);
    }
}
#endif