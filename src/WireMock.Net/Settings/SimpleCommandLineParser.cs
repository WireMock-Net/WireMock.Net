using System;
using System.Collections.Generic;
using System.Linq;

namespace WireMock.Settings;

// Based on http://blog.gauffin.org/2014/12/simple-command-line-parser/
internal class SimpleCommandLineParser
{
    private const string Sigil = "--";

    private IDictionary<string, string[]> Arguments { get; } = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

    public void Parse(string[] arguments)
    {
        string currentName = string.Empty;

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

    public string[] GetValues(string name, string[] defaultValue)
    {
        return Contains(name) ? Arguments[name] : defaultValue;
    }

    public string[]? GetValues(string name)
    {
        return Contains(name) ? Arguments[name] : default;
    }

    public T GetValue<T>(string name, Func<string[], T> func, T defaultValue)
    {
        return Contains(name) ? func(Arguments[name]) : defaultValue;
    }

    public T? GetValue<T>(string name, Func<string[], T> func)
    {
        return Contains(name) ? func(Arguments[name]) : default;
    }

    public bool GetBoolValue(string name, bool defaultValue = false)
    {
        return GetValue(name, values =>
        {
            var value = values.FirstOrDefault();
            return !string.IsNullOrEmpty(value) ? bool.Parse(value) : defaultValue;
        }, defaultValue);
    }

    public bool GetBoolSwitchValue(string name)
    {
        return Contains(name);
    }

    public int? GetIntValue(string name, int? defaultValue = null)
    {
        return GetValue(name, values =>
        {
            var value = values.FirstOrDefault();
            return !string.IsNullOrEmpty(value) ? int.Parse(value) : defaultValue;
        }, defaultValue);
    }

    public TEnum? GetEnumValue<TEnum>(string name)
        where TEnum : struct
    {
        return GetValue(name, values =>
        {
            var value = values.FirstOrDefault();
            return Enum.TryParse<TEnum>(value, true, out var enumValue) ? enumValue : (TEnum?)null;
        });
    }

    public TEnum GetEnumValue<TEnum>(string name, TEnum defaultValue)
        where TEnum : struct
    {
        return GetValue(name, values =>
        {
            var value = values.FirstOrDefault();
            return Enum.TryParse<TEnum>(value, true, out var enumValue) ? enumValue : defaultValue;
        }, defaultValue);
    }

    public string GetStringValue(string name, string defaultValue)
    {
        return GetValue(name, values => values.FirstOrDefault() ?? defaultValue, defaultValue);
    }

    public string? GetStringValue(string name)
    {
        return GetValue(name, values => values.FirstOrDefault());
    }
}