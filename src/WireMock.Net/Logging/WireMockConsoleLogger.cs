using System;
using Newtonsoft.Json;
using WireMock.Admin.Requests;

namespace WireMock.Logging;

/// <summary>
/// WireMockConsoleLogger which logs to Console
/// </summary>
/// <seealso cref="IWireMockLogger" />
public class WireMockConsoleLogger : IWireMockLogger
{
    private readonly bool _removeNewLines;

    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockConsoleLogger"/> class.
    /// </summary>
    public WireMockConsoleLogger(bool removeNewLines = false)
    {
        _removeNewLines = removeNewLines;

        Console.OutputEncoding = System.Text.Encoding.UTF8;
    }

    /// <inheritdoc />
    public void Debug(string formatString, params object[] args)
    {
        WriteLine(Format("Debug", formatString, args));
    }

    /// <inheritdoc />
    public void Info(string formatString, params object[] args)
    {
        WriteLine(Format("Info", formatString, args));
    }

    /// <inheritdoc />
    public void Warn(string formatString, params object[] args)
    {
        WriteLine(Format("Warn", formatString, args));
    }

    /// <inheritdoc />
    public void Error(string formatString, params object[] args)
    {
        WriteLine(Format("Error", formatString, args));
    }

    /// <inheritdoc />
    public void Error(string formatString, Exception exception)
    {
        WriteLine(Format("Error", formatString, exception.Message));

        if (exception is AggregateException ae)
        {
            ae.Handle(ex =>
            {
                WriteLine(Format("Error", "Exception {0}", ex.Message));
                return true;
            });
        }
    }

    /// <inheritdoc />
    public void DebugRequestResponse(LogEntryModel logEntryModel, bool isAdminRequest)
    {
        string message = JsonConvert.SerializeObject(logEntryModel, Formatting.Indented);
        WriteLine(Format("DebugRequestResponse", "Admin[{0}] {1}", isAdminRequest, message));
    }

    private static string Format(string level, string formatString, params object[] args)
    {
        var message = args.Length > 0 ? string.Format(formatString, args) : formatString;

        return $"{DateTime.UtcNow} [{level}] : {message}";
    }

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the console.
    /// </summary>
    /// <param name="value">The value to write.</param>
    private void WriteLine(string value)
    {
        Console.WriteLine(!_removeNewLines ? value : value.Replace(Environment.NewLine, string.Empty));
    }
}