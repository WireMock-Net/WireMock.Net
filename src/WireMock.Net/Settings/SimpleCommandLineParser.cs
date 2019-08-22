using System;
using System.Collections.Generic;
using System.Linq;

namespace WireMock.Settings
{
    // Based on http://blog.gauffin.org/2014/12/simple-command-line-parser/
    internal class SimpleCommandLineParser
    {
        private const string Sigil = "--";

        private IDictionary<string, string[]> Arguments { get; } = new Dictionary<string, string[]>();

        public void Parse(string[] arguments)
        {
            string currentName = null;

            var values = new List<string>();

            // Split a single argument on a space character to fix issue (e.g. Azure Service Fabric) when an argument is supplied like "--x abc" or '--x abc'
            foreach (string arg in arguments.SelectMany(arg => arg.Split(' ')))
            {
                if (arg.StartsWith(Sigil))
                {
                    if (!string.IsNullOrEmpty(currentName))
                    {
                        Arguments[currentName] = values.ToArray();
                    }

                    values.Clear();
                    currentName = arg.Substring(Sigil.Length);
                }
                else if (string.IsNullOrEmpty(currentName))
                {
                    Arguments[arg] = new string[0];
                }
                else
                {
                    values.Add(arg);
                }
            }

            if (!string.IsNullOrEmpty(currentName))
            {
                Arguments[currentName] = values.ToArray();
            }
        }

        public bool Contains(string name)
        {
            return Arguments.ContainsKey(name);
        }

        public string[] GetValues(string name, string[] defaultValue = null)
        {
            return Contains(name) ? Arguments[name] : defaultValue;
        }

        public T GetValue<T>(string name, Func<string[], T> func, T defaultValue = default(T))
        {
            return Contains(name) ? func(Arguments[name]) : defaultValue;
        }

        public bool GetBoolValue(string name, bool defaultValue = false)
        {
            return GetValue(name, values =>
            {
                string value = values.FirstOrDefault();
                return !string.IsNullOrEmpty(value) ? bool.Parse(value) : defaultValue;
            }, defaultValue);
        }

        public int? GetIntValue(string name, int? defaultValue = null)
        {
            return GetValue(name, values =>
            {
                string value = values.FirstOrDefault();
                return !string.IsNullOrEmpty(value) ? int.Parse(value) : defaultValue;
            }, defaultValue);
        }

        public string GetStringValue(string name, string defaultValue = null)
        {
            return GetValue(name, values => values.FirstOrDefault() ?? defaultValue, defaultValue);
        }
    }
}