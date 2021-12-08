using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WireMock.RegularExpressions
{
    /// <summary>
    /// Extension to the <see cref="Regex"/> object, adding support for GUID
    /// tokens for matching on.
    /// </summary>
    public class RegexGuid : Regex
    {
        /// <summary>
        /// Token for a GUID formated with `B` format specifier with lower case
        /// values.
        /// </summary>
        public const string GuidBLowerToken = @"\guidb";

        /// <summary>
        /// Regular expression pattern associated with the expected format for
        /// `B` format specifier with lower case.
        /// </summary>
        public const string GuidBLowerRegexPattern = @"(\{[a-z0-9]{8}-([a-z0-9]{4}-){3}[a-z0-9]{12}\})";

        /// <summary>
        /// Token for a GUID formated with `B` format specifier.
        /// </summary>
        public const string GuidBToken = @"\GUIDB";

        /// <summary>
        /// Regular expression pattern associated with the expected format for
        /// `B` format specifier.
        /// </summary>
        public const string GuidBRegexPattern = @"(\{[A-Z0-9]{8}-([A-Z0-9]{4}-){3}[A-Z0-9]{12}\})";

        /// <summary>
        /// Token for a GUID formated with `D` format specifier with lower case
        /// values.
        /// </summary>
        public const string GuidDLowerToken = @"\guidd";

        /// <summary>
        /// Regular expression pattern associated with the expected format for
        /// `D` format specifier with lower case.
        /// </summary>
        public const string GuidDLowerRegexPattern = "([a-z0-9]{8}-([a-z0-9]{4}-){3}[a-z0-9]{12})";

        /// <summary>
        /// Token for a GUID formated with `D` format specifier.
        /// </summary>
        public const string GuidDToken = @"\GUIDD";

        /// <summary>
        /// Regular expression pattern associated with the expected format for
        /// `D` format specifier.
        /// </summary>
        public const string GuidDRegexPattern = "([A-Z0-9]{8}-([A-Z0-9]{4}-){3}[A-Z0-9]{12})";

        /// <summary>
        /// Token for a GUID formated with `P` format specifier with lower case.
        /// </summary>
        public const string GuidPLowerToken = @"\guidp";

        /// <summary>
        /// Regular expression pattern associated with the expected format for
        /// `P` format specifier with lower case.
        /// </summary>
        public const string GuidPLowerRegexPattern = @"(\([a-z0-9]{8}-([a-z0-9]{4}-){3}[a-z0-9]{12}\))";

        /// <summary>
        /// Token for a GUID formated with `P` format specifier.
        /// </summary>
        public const string GuidPToken = @"\GUIDP";

        /// <summary>
        /// Regular expression pattern associated with the expected format for
        /// `P` format specifier.
        /// </summary>
        public const string GuidPRegexPattern = @"(\([A-Z0-9]{8}-([A-Z0-9]{4}-){3}[A-Z0-9]{12}\))";

        /// <summary>
        /// Token for a GUID formated with `N` format specifier with lower case.
        /// </summary>
        public const string GuidNLowerToken = @"\guidn";

        /// <summary>
        /// Regular expression pattern associated with the expected format for
        /// `N` format specifier with lower case.
        /// </summary>
        public const string GuidNLowerRegexPattern = "([a-z0-9]{32})";

        /// <summary>
        /// Token for a GUID formated with `N` format specifier.
        /// </summary>
        public const string GuidNToken = @"\GUIDN";

        /// <summary>
        /// Regular expression pattern associated with the expected format for
        /// `N` format specifier.
        /// </summary>
        public const string GuidNRegexPattern = "([A-Z0-9]{32})";

        /// <summary>
        /// Token for a GUID formated with `X` format specifier with lower case.
        /// </summary>
        public const string GuidXLowerToken = @"\guidx";

        /// <summary>
        /// Regular expression pattern associated with the expected format for
        /// `X` format specifier with lower case.
        /// </summary>
        public const string GuidXLowerRegexPattern = @"(\{0x[a-f0-9]{8},0x[a-f0-9]{4},0x[a-f0-9]{4},\{(0x[a-f0-9]{2},){7}(0x[a-f0-9]{2})\}\})";

        /// <summary>
        /// Token for a GUID formated with `X` format specifier.
        /// </summary>
        public const string GuidXToken = @"\GUIDX";

        /// <summary>
        /// Regular expression pattern associated with the expected format for
        /// `X` format specifier.
        /// </summary>
        public const string GuidXRegexPattern = @"(\{0x[A-F0-9]{8},0x[A-F0-9]{4},0x[A-F0-9]{4},\{(0x[A-F0-9]{2},){7}(0x[A-F0-9]{2})\}\})";

        /// <inheritdoc cref="Regex"/>
        public RegexGuid(string pattern) : this(pattern, RegexOptions.None)
        {
        }

        /// <inheritdoc cref="Regex"/>
        public RegexGuid(string pattern,
                         RegexOptions options)
          : this(pattern, options, Regex.InfiniteMatchTimeout)
        {
        }

        /// <inheritdoc cref="Regex"/>
        public RegexGuid(string pattern,
                         RegexOptions options,
                         TimeSpan matchTimeout)
          : base(ReplaceGuidPattern(pattern), options, matchTimeout)
        {
        }

        /// <summary>
        /// Replaces all instances of valid GUID tokens with the correct regular
        /// expression to match.
        /// </summary>
        /// <param name="pattern">
        /// Pattern to replace token for.
        /// </param>
        private static string ReplaceGuidPattern(string pattern)
          => pattern.Replace(GuidBLowerToken, GuidBLowerRegexPattern)
                    .Replace(GuidBToken, GuidBRegexPattern)
                    .Replace(GuidDLowerToken, GuidDLowerRegexPattern)
                    .Replace(GuidDToken, GuidDRegexPattern)
                    .Replace(GuidNLowerToken, GuidNLowerRegexPattern)
                    .Replace(GuidNToken, GuidNRegexPattern)
                    .Replace(GuidPLowerToken, GuidPLowerRegexPattern)
                    .Replace(GuidPToken, GuidPRegexPattern)
                    .Replace(GuidXLowerToken, GuidXLowerRegexPattern)
                    .Replace(GuidXToken, GuidXRegexPattern);
    }
}
