using System.Timers;
using Microsoft.Build.Construction;
using Newtonsoft.Json;
using NuGet.Packaging;
using WireMock.Settings;
using Timer = System.Timers.Timer;

namespace WireMock.Server;

public class WireMockService
{
    public string Id => _model.Id.ToString();
    public string Name => _model.Name;
    public bool IsRunning => _server?.IsStarted ?? false;

    public EventHandler<ChangedMappingsArgs>? MappingsAdded;
    public EventHandler<ChangedMappingsArgs>? MappingsRemoved;

    private WireMockServer _server;
    private WireMockServerSettings _settings;
    private WireMockServerModel _model;
    private ILogger _logger;

    private IList<Guid> _lastKnonwMappings = new List<Guid>();
    private Timer _checkMappingsTimer = new(2000);

    public WireMockService(ILogger logger, WireMockServerModel model)
    {
        _logger = logger;
        _model = model;
        _settings = model.ToSettings();
        _settings.Logger = new WireMockLogger(logger);
    }

    public void CreateAndStart()
    {
        _logger?.LogInformation("WireMock.Net server starting");
        _server = WireMockServer.Start(_settings);

        // check mappings before starting to register all "default" mappings as already known
        CheckMappings();

        // create the timer to check for new mappings
        CreateAndStartTimer();
        _logger?.LogInformation($"WireMock.Net server settings {JsonConvert.SerializeObject(_settings)}");
    }

    /// <summary>
    /// Creates and starts the timer to periodically check for new mappings.
    /// </summary>
    private void CreateAndStartTimer()
    {
        _checkMappingsTimer.Elapsed += CheckMappings;
        _checkMappingsTimer.AutoReset = true;
        _checkMappingsTimer.Start();
    }

    private void CheckMappings(object? sender = null, ElapsedEventArgs? e = null)
    {
        // lock makes debugging simpler as we could change the lists of known mappings in a breakpoint
        // its probably not needed in RL as the handling is quick enough (hopefully :P)
        lock (this)
        {
            var currentMappings = _server.Mappings.Select(m => m.Guid).ToList();
            var newMappings = currentMappings.Except(_lastKnonwMappings).ToList();
            var removedMapping = _lastKnonwMappings.Except(currentMappings).ToList();

            // Raise events for new and removed mappings
            if (newMappings is { Count: > 0 })
                RaiseNewMappings(newMappings);
            if (removedMapping is { Count: > 0 })
                RaiseMappingRemoved(removedMapping);

            _lastKnonwMappings.Clear();
            _lastKnonwMappings.AddRange(currentMappings);
        }
    }

    private void RaiseMappingRemoved(List<Guid> removedMappings)
    {
        MappingsRemoved?.Invoke(this, new ChangedMappingsArgs(removedMappings)
        {
            ServiceId = Id,
        });
    }

    private void RaiseNewMappings(List<Guid> deltaMappings)
    {
        MappingsAdded?.Invoke(this, new ChangedMappingsArgs(deltaMappings)
        {
            ServiceId = Id,
        });
    }


    public void Stop()
    {
        _server?.Stop();
        _checkMappingsTimer.Stop();
    }
}

public class ChangedMappingsArgs : EventArgs
{
    public string ServiceId { get; set; }
    public IList<Guid> MapGuid { get; set; }

    public ChangedMappingsArgs(IList<Guid> mapGuid)
    {
        MapGuid = mapGuid;
    }
}