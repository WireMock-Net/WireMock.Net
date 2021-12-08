using System;
using NFluent;
using WireMock.RegularExpressions;
using Xunit;

namespace WireMock.Net.Tests.RegularExpressions
{
    public class RegexGuidTests
    {
        /// <summary>
        /// Input guid used for testing
        /// </summary>
        public Guid InputGuid { get; } = Guid.NewGuid();

        [Fact]
        public void RegexGuid_GuidB_Pattern()
        {
            var guidbUpper = @".*\GUIDB.*";
            var guidbLower = @".*\guidb.*";

            var inputLower = InputGuid.ToString("B");
            var inputUpper = InputGuid.ToString("B").ToUpper();
            var regexLower = new RegexGuid(guidbLower);
            var regexUpper = new RegexGuid(guidbUpper);

            Check.That(regexLower.IsMatch(inputLower)).Equals(true);
            Check.That(regexLower.IsMatch(inputUpper)).Equals(false);
            Check.That(regexUpper.IsMatch(inputUpper)).Equals(true);
            Check.That(regexUpper.IsMatch(inputLower)).Equals(false);
        }

        [Fact]
        public void RegexGuid_GuidD_Pattern()
        {
            var guiddUpper = @".*\GUIDD.*";
            var guiddLower = @".*\guidd.*";

            var inputLower = InputGuid.ToString("D");
            var inputUpper = InputGuid.ToString("D").ToUpper();
            var regexLower = new RegexGuid(guiddLower);
            var regexUpper = new RegexGuid(guiddUpper);

            Check.That(regexLower.IsMatch(inputLower)).Equals(true);
            Check.That(regexLower.IsMatch(inputUpper)).Equals(false);
            Check.That(regexUpper.IsMatch(inputUpper)).Equals(true);
            Check.That(regexUpper.IsMatch(inputLower)).Equals(false);
        }

        [Fact]
        public void RegexGuid_GuidN_Pattern()
        {
            var guidnUpper = @".*\GUIDN.*";
            var guidnLower = @".*\guidn.*";

            var inputLower = InputGuid.ToString("N");
            var inputUpper = InputGuid.ToString("N").ToUpper();
            var regexLower = new RegexGuid(guidnLower);
            var regexUpper = new RegexGuid(guidnUpper);

            Check.That(regexLower.IsMatch(inputLower)).Equals(true);
            Check.That(regexLower.IsMatch(inputUpper)).Equals(false);
            Check.That(regexUpper.IsMatch(inputUpper)).Equals(true);
            Check.That(regexUpper.IsMatch(inputLower)).Equals(false);
        }

        [Fact]
        public void RegexGuid_GuidP_Pattern()
        {
            var guidpUpper = @".*\GUIDP.*";
            var guidpLower = @".*\guidp.*";

            var inputLower = InputGuid.ToString("P");
            var inputUpper = InputGuid.ToString("P").ToUpper();
            var regexLower = new RegexGuid(guidpLower);
            var regexUpper = new RegexGuid(guidpUpper);

            Check.That(regexLower.IsMatch(inputLower)).Equals(true);
            Check.That(regexLower.IsMatch(inputUpper)).Equals(false);
            Check.That(regexUpper.IsMatch(inputUpper)).Equals(true);
            Check.That(regexUpper.IsMatch(inputLower)).Equals(false);
        }

        [Fact]
        public void RegexGuid_GuidX_Pattern()
        {
            var guidxUpper = @".*\GUIDX.*";
            var guidxLower = @".*\guidx.*";

            var inputLower = InputGuid.ToString("X");
            var inputUpper = InputGuid.ToString("X").ToUpper().Replace("X", "x");
            var regexLower = new RegexGuid(guidxLower);
            var regexUpper = new RegexGuid(guidxUpper);

            Check.That(regexLower.IsMatch(inputLower)).Equals(true);
            Check.That(regexLower.IsMatch(inputUpper)).Equals(false);
            Check.That(regexUpper.IsMatch(inputUpper)).Equals(true);
            Check.That(regexUpper.IsMatch(inputLower)).Equals(false);
        }

    }
}
