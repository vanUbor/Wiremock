using System.Timers;
using Newtonsoft.Json;
using NuGet.Packaging;
using WireMock.Admin.Mappings;
using WireMock.Data;
using WireMock.Settings;
using Timer = System.Timers.Timer;

namespace WireMock.Server;

public class WireMockService(WireMockServiceModel model)
{
    public string Id => model.Id.ToString();
    public string Name => model.Name;
    public virtual bool IsRunning => _server?.IsStarted ?? false;

    public EventHandler<ChangedMappingsArgs>? MappingsAdded;
    public EventHandler<ChangedMappingsArgs>? MappingsRemoved;

    private WireMockServer? _server;
    private readonly WireMockServerSettings _settings = model.ToSettings();

    private readonly List<MappingModel> _lastKnownMappings = new ();
    private readonly Timer _checkMappingsTimer = new(2000);

    public void CreateAndStart(IEnumerable<WireMockServerMapping>? mappings = default)
    {
        _server = WireMockServer.Start(_settings);
        if (mappings != null)
            foreach (var m in mappings
                         .Where(m => !string.IsNullOrWhiteSpace(m.Raw)))
            {
                if (m.Raw == null)
                    continue;

                var mapping = JsonConvert.DeserializeObject<MappingModel>(m.Raw);

                if (mapping != null)
                    _server.WithMapping(mapping);
            }

        // create the timer to check for new mappings
        CreateAndStartTimer();
    }
    
    public void Stop()
    {
        _server?.Stop();
        _checkMappingsTimer.Stop();
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
            if (_server == null)
                return;

            var currentMappings = _server.MappingModels.Select(m => m).ToList();

            var newMappings = currentMappings
                .Where(m => _lastKnownMappings.TrueForAll(lm => lm.Guid != m.Guid))
                .ToList();

            var removedMapping = _lastKnownMappings
                .Where(lkm => currentMappings.TrueForAll(cm => cm.Guid != lkm.Guid))
                .ToList();

            // Raise events for new and removed mappings
            if (newMappings is { Count: > 0 })
                RaiseNewMappings(newMappings);
            if (removedMapping is { Count: > 0 })
                RaiseMappingRemoved(removedMapping);

            _lastKnownMappings.Clear();
            _lastKnownMappings.AddRange(currentMappings);
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

 
}

public class ChangedMappingsArgs(IList<MappingModel> mappingModels) : EventArgs
{
    public required string ServiceId { get; init; }
    public IList<MappingModel> MappingModels { get; } = mappingModels;
}