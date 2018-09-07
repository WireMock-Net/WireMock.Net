using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WireMock.Utils
{
    internal static class RegexUtils
    {
        public static Dictionary<string, string> GetNamedGroups(Regex regex, string input)
        {
            var namedGroupsDictionary = new Dictionary<string, string>();

            GroupCollection groups = regex.Match(input).Groups;
            foreach (string groupName in regex.GetGroupNames())
            {
                if (groups[groupName].Captures.Count > 0)
                {
                    namedGroupsDictionary.Add(groupName, groups[groupName].Value);
                }
            }

            return namedGroupsDictionary;
        }
    }
}
