using System.Linq;
using System.Text.RegularExpressions;

namespace WireMock.Util
{
    internal static class StringUtils
    {
        public static bool TryParseQuotedString(string value, out string result, out char quote)
        {
            result = null;
            quote = '\0';

            if (value == null || value.Length < 2)
            {
                return false;
            }

            quote = value[0]; // This can be single or a double quote
            if (quote != '"' && quote != '\'')
            {
                return false;
            }

            if (value.Last() != quote)
            {
                return false;
            }

            try
            {
                result = Regex.Unescape(value.Substring(1, value.Length - 2));
                return true;
            }
            catch
            {
                // Ignore Exception, just continue and return false.
            }

            return false;
        }
    }
}