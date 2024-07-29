using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using NuGet.Protocol;
using WireMock.Server;

namespace WireMock.Data;

public class ServiceOrchestrator
{
    private IWireMockRepository _repo;
    private WireMockServiceList _services = default!;

    public ServiceOrchestrator(WireMockServiceList serviceList,
        IWireMockRepository repository)
    {
        _services = serviceList;
        _services.MappingAdded += (sender, changedMappingsArgs) 
            => _ = SaveMappingToContextAsync(sender, changedMappingsArgs);
        
        _services.MappingRemoved += RemoveMappingFromContext;
        _repo = repository;
    }

    /// <summary>
    /// Saves the mappings to the database context.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments containing the service ID and mapping GUIDs.</param>
    private async Task SaveMappingToContextAsync(object? sender, ChangedMappingsArgs e)
    {
        try
        {
            await _repo.AddMappingsAsync(int.Parse(e.ServiceId), e.MappingModels.Select(mm
                => new Tuple<Guid, string>(mm.Guid!.Value, mm.ToJson())));
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }

    private void RemoveMappingFromContext(object? sender, ChangedMappingsArgs e)
    {
        _repo.RemoveMappingsAsync(e.MappingModels.Select(mm => mm.Guid!.Value));
    }

    public async Task<IList<WireMockService>> GetOrCreateServicesAsync()
    {
        if (_services is { Count: > 0 })
        {
            var models = await _repo.GetModelsAsync();

            // create new services for each model found in configuration that is not already in the service list
            foreach (var model in models)
            {
                if (_services.Any(s => s.Id.Equals(model.Id.ToString(),
                        StringComparison.CurrentCultureIgnoreCase)))
                    continue;
                _services.Add(CreateService(model));
            }

            return _services;
        }

        // if no service is created yet, create all services in the models configuration
        await CreateServices();
        return _services;
    }


    private async Task CreateServices()
    {
        var models = await _repo.GetModelsAsync();
        _services.AddRange(models.Select(CreateService));
    }


    public async Task CreateServiceAsync(int id)
    {
        var models = await _repo.GetModelsAsync();
        var m = models.Single(model => model.Id == id);
        _services.Add(CreateService(m));
    }

    /// <summary>
    /// Creates a WireMockService and adds it to the list of services.
    /// </summary>
    /// <returns>Void</returns>
    private WireMockService CreateService(WireMockServiceModel model)
    {
        var service = new WireMockService(model);
        return service;
    }

    internal async Task StartAsync(int id)
    {
        var service = _services.Single(i => i.Id.Equals(id.ToString()
            , StringComparison.InvariantCultureIgnoreCase));

        var models = await _repo.GetModelsAsync();
            
        var m = models.Single(model => model.Id == id);
        service.CreateAndStart(m.Mappings);
    }

    internal void Stop(int? id)
    {
        var service = _services.Single(i => i.Id.Equals(id.ToString()
            , StringComparison.InvariantCultureIgnoreCase));

        service.Stop();
    }

    public void RemoveService(int? id)
    {
        var service = _services.Single(i => i.Id.Equals(id.ToString()
            , StringComparison.InvariantCultureIgnoreCase));
        _services.Remove(service);
    }
}