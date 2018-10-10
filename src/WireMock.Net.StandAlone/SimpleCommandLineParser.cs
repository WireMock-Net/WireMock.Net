using System;
using System.Collections.Generic;
using System.Linq;

namespace WireMock.Net.StandAlone
{
    // Based on http://blog.gauffin.org/2014/12/simple-command-line-parser/
    internal class SimpleCommandLineParser
    {
        private const string Sigil = "--";
        private const string SigilAzureServiceFabric = "'--";

        private enum SigilType
        {
            Normal,
            AzureServiceFabric
        }

        private IDictionary<string, string[]> Arguments { get; } = new Dictionary<string, string[]>();

        public void Parse(string[] args)
        {
            SigilType sigil = SigilType.Normal;
            string currentName = null;

            var values = new List<string>();
            foreach (string arg in args)
            {
                if (arg.StartsWith(Sigil))
                {
                    sigil = SigilType.Normal;

                    if (!string.IsNullOrEmpty(currentName))
                    {
                        Arguments[currentName] = values.ToArray();
                    }

                    values.Clear();
                    currentName = arg.Substring(Sigil.Length);
                }
                // Azure Service Fabric passes the command line parameter surrounded with single quotes. (https://github.com/Microsoft/service-fabric/issues/234)
                else if (arg.StartsWith(SigilAzureServiceFabric))
                {
                    sigil = SigilType.AzureServiceFabric;

                    if (!string.IsNullOrEmpty(currentName))
                    {
                        Arguments[currentName] = values.ToArray();
                    }

                    values.Clear();
                    currentName = arg.Substring(SigilAzureServiceFabric.Length);
                }
                else if (string.IsNullOrEmpty(currentName))
                {
                    Arguments[arg] = new string[0];
                }
                else
                {
                    if (sigil == SigilType.Normal)
                    {
                        values.Add(arg);
                    }
                    else
                    {
                        values.Add(arg.Substring(0, arg.Length - 1));
                    }
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