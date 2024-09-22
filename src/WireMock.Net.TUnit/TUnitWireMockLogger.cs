// Copyright © WireMock.Net

using System;
using Newtonsoft.Json;
using Stef.Validation;
using TUnit.Core.Logging;
using WireMock.Admin.Requests;
using WireMock.Logging;

namespace WireMock.Net.TUnit;

/// <summary>
/// When using TUnit, this class enables to log the output from WireMock.Net to the <see cref="TUnitLogger"/>.
/// </summary>
// ReSharper disable once InconsistentNaming
public sealed class TUnitWireMockLogger : IWireMockLogger
{
    private readonly TUnitLogger _tUnitLogger;

    /// <summary>
    /// Create a new instance on the <see cref="TUnitWireMockLogger"/>.
    /// </summary>
    /// <param name="tUnitLogger">Represents a class which can be used to provide test output.</param>
    public TUnitWireMockLogger(TUnitLogger tUnitLogger)
    {
        _tUnitLogger = Guard.NotNull(tUnitLogger);
    }

    /// <inheritdoc />
    public void Debug(string formatString, params object[] args)
    {
        _tUnitLogger.LogDebug(Format("Debug", formatString, args));
    }

    /// <inheritdoc />
    public void Info(string formatString, params object[] args)
    {
        _tUnitLogger.LogInformation(Format("Info", formatString, args));
    }

    /// <inheritdoc />
    public void Warn(string formatString, params object[] args)
    {
        _tUnitLogger.LogWarning(Format("Warning", formatString, args));
    }

    /// <inheritdoc />
    public void Error(string formatString, params object[] args)
    {
        _tUnitLogger.LogError(Format("Error", formatString, args));
    }

    /// <inheritdoc />
    public void Error(string formatString, Exception exception)
    {
        _tUnitLogger.LogError(Format("Error", formatString, exception.Message), exception);

        if (exception is AggregateException ae)
        {
            ae.Handle(ex =>
            {
                _tUnitLogger.LogError(Format("Error", "Exception {0}", ex.Message), exception);
                return true;
            });
        }
    }

    /// <inheritdoc />
    public void DebugRequestResponse(LogEntryModel logEntryModel, bool isAdminRequest)
    {
        var message = JsonConvert.SerializeObject(logEntryModel, Formatting.Indented);
        _tUnitLogger.LogDebug(Format("DebugRequestResponse", "Admin[{0}] {1}", isAdminRequest, message));
    }

    private static string Format(string level, string formatString, params object[] args)
    {
        var message = args.Length > 0 ? string.Format(formatString, args) : formatString;
        return $"{DateTime.UtcNow} [{level}] : {message}";
    }
}