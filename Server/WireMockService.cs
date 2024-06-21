using Newtonsoft.Json;
using WireMock.Pages.WireMockServers;
using WireMock.Settings;

namespace WireMock.Server;

public class WireMockService
{
    public string Id => _model.Id.ToString();
    public string Name => _model.Name;
    public bool IsRunning => _server.IsStarted;

    private WireMockServer _server;
    private WireMockServerSettings _settings;
    private WireMockServerModel _model;
    private ILogger _logger;

    public WireMockService(ILogger logger, WireMockServerModel model)
    {
        _logger = logger;
        _model = model;
        _settings = model.ToSettings();
        _settings.Logger = new WireMockLogger(logger);
    }

    public void Run()
    {
        _logger.LogInformation("WireMock.Net server starting");
        _server = WireMockServer.Start(_settings);
        _logger.LogInformation($"WireMock.Net server settings {JsonConvert.SerializeObject(_settings)}");
    }

    public void Stop()
    {
        _server.Stop();
    }
}