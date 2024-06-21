using WireMock.Admin.Requests;
using WireMock.Logging;

namespace WireMock.Server;

public class WireMockLogger : IWireMockLogger
{
    private ILogger _logger;

    public WireMockLogger(ILogger logger)
    {
        _logger = logger;
    }

    public void Debug(string formatString, params object[] args)
        => _logger.LogDebug(formatString, args);

    public void Info(string formatString, params object[] args)
        => _logger.LogInformation(formatString, args);

    public void Warn(string formatString, params object[] args)
        => _logger.LogWarning(formatString, args);

    public void Error(string formatString, params object[] args)
        => _logger.LogError(formatString, args);

    public void Error(string formatString, Exception exception)
        => _logger.LogError(exception, formatString);

    public void DebugRequestResponse(LogEntryModel logEntryModel, bool isAdminRequest)
        => _logger.LogDebug("N/A");
}