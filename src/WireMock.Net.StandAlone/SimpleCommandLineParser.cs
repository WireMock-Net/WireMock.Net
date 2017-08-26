using System;
using System.Collections.Generic;
using System.Linq;

namespace WireMock.Net.StandAlone
{
    // Based on http://blog.gauffin.org/2014/12/simple-command-line-parser/
    internal class SimpleCommandLineParser
    {
        public IDictionary<string, string[]> Arguments { get; private set; } = new Dictionary<string, string[]>();

        public void Parse(string[] args)
        {
            string currentName = null;

            var values = new List<string>();
            foreach (string arg in args)
            {
                if (arg.StartsWith("--"))
                {
                    if (!string.IsNullOrEmpty(currentName))
                    {
                        Arguments[currentName] = values.ToArray();
                    }

                    values.Clear();
                    currentName = arg.Substring(2);
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
            return GetValue<bool>(name, (values) =>
            {
                string value = values.FirstOrDefault();
                return !string.IsNullOrEmpty(value) ? bool.Parse(value) : defaultValue;
            }, defaultValue);
        }

        public int? GetIntValue(string name, int? defaultValue = null)
        {
            return GetValue<int?>(name, (values) =>
            {
                string value = values.FirstOrDefault();
                return !string.IsNullOrEmpty(value) ? int.Parse(value) : defaultValue;
            }, defaultValue);
        }

        public string GetStringValue(string name, string defaultValue = null)
        {
            return GetValue<string>(name, (values) =>
            {
                return values.FirstOrDefault() ?? defaultValue;
            }, defaultValue);
        }
    }
}