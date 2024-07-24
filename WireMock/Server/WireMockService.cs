using System.Timers;
using Newtonsoft.Json;
using NuGet.Packaging;
using WireMock.Admin.Mappings;
using WireMock.Data;
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
    private WireMockServiceModel _model;
    private ILogger _logger;

    private IList<MappingModel> _lastKnonwMappings = new List<MappingModel>();
    private Timer _checkMappingsTimer = new(2000);

    public WireMockService(ILogger logger, WireMockServiceModel model)
    {
        _logger = logger;
        _model = model;
        _settings = model.ToSettings();
        _settings.Logger = new WireMockLogger(logger);
    }

    public void CreateAndStart(WireMockServiceModel model)
    {
        _logger?.LogInformation("WireMock.Net server starting");
        _server = WireMockServer.Start(_settings);
        foreach (var m in model.Mappings)
        {
            var mapping =JsonConvert.DeserializeObject<MappingModel>(m.Raw);
            _server.WithMapping(mapping);
        }
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

    /// <summary>
    /// This method checks for new mappings periodically by using a timer.
    /// It compares the current mappings with the previous set of known mappings and raises events
    /// for any new mappings or removed mappings.
    /// </summary>
    /// <remarks>
    /// This method is called internally by the WireMockService class and should not be called directly.
    /// </remarks>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments for the Elapsed event.</param>
    private void CheckMappings(object? sender = null, ElapsedEventArgs? e = null)
    {
        // lock makes debugging simpler as we could change the lists of known mappings in a breakpoint
        // its probably not needed in RL as the handling is quick enough (hopefully :P)
        lock (this)
        {
            var currentMappings = _server.MappingModels.Select(m => m).ToList();

            // var newMappings = currentMappings.Except(_lastKnonwMappings).ToList();
            var newMappings = currentMappings
                .Where(m => _lastKnonwMappings.All(lm => lm.Guid != m.Guid))
                .ToList();

            // var removedMapping = _lastKnonwMappings.Except(currentMappings).ToList();
            var removedMapping = _lastKnonwMappings
                .Where(lkm => currentMappings.All(cm => cm.Guid != lkm.Guid))
                .ToList();

            // Raise events for new and removed mappings
            if (newMappings is { Count: > 0 })
                RaiseNewMappings(newMappings);
            if (removedMapping is { Count: > 0 })
                RaiseMappingRemoved(removedMapping);

            _lastKnonwMappings.Clear();
            _lastKnonwMappings.AddRange(currentMappings);
        }
    }

    private void RaiseMappingRemoved(List<MappingModel> removedMappings)
    {
        MappingsRemoved?.Invoke(this, new ChangedMappingsArgs(removedMappings)
        {
            ServiceId = Id,
        });
    }

    private void RaiseNewMappings(List<MappingModel> deltaMappings)
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
    public IList<MappingModel> MapGuid { get; set; }


    public ChangedMappingsArgs(IList<MappingModel> mapGuid)
    {
        MapGuid = mapGuid;
    }
}