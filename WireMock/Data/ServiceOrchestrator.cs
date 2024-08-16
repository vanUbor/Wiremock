using System.Diagnostics.CodeAnalysis;
using NuGet.Packaging;
using NuGet.Protocol;
using WireMock.Server;
using WireMock.Server.Interfaces;

namespace WireMock.Data;

public interface IOrchestrator
{
    void RemoveService(int id);
    void Stop(int id);
    Task CreateServiceAsync(int id);
    Task<IList<WireMockService>> GetOrCreateServicesAsync();
    Task StartServiceAsync(int id);
    bool IsRunning(int serviceId);
}
public class ServiceOrchestrator : IOrchestrator
{
    private readonly IWireMockRepository _repo;
    private readonly WireMockServiceList _services;

    public ServiceOrchestrator(WireMockServiceList serviceList,
        IWireMockRepository repository)
    {
        _services = serviceList;
        _services.MappingAdded += (sender, changedMappingsArgs)
            => _ = SaveMappingToContextAsync(changedMappingsArgs);

        _services.MappingRemoved += RemoveMappingFromContext;
        _repo = repository;
    }


    /// <summary>
    /// Retrieves the existing WireMock services from the service list,
    /// or creates new services based on the models obtained from the repository,
    /// and adds them to the service list.
    /// The Services will only be created and NOT started
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.
    /// The result contains a list of WireMockService objects.</returns>
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
        await CreateServicesAsync();
        return _services;
    }

    /// <summary>
    /// Creates a WireMockService with the specified ID and adds it to the list of services.
    /// A Model with the ID must be present in the database, otherwise an exception is thrown
    /// The Service itself will only be created and NOT started
    /// </summary>
    /// <param name="id">The ID of the service.</param>
    public async Task CreateServiceAsync(int id)
    {
        var models = await _repo.GetModelsAsync();
        var m = models.Single(model => model.Id == id);
        _services.Add(CreateService(m));
    }

    /// <summary>
    /// Removes a WireMockService from the list of services.    
    /// </summary>
    /// <param name="id">The ID of the service to remove.</param>
    public virtual void RemoveService(int id)
    {
        var service = _services.Single(i => i.Id.Equals(id.ToString()
            , StringComparison.InvariantCultureIgnoreCase));
        _services.Remove(service);
    }

    /// <summary>
    /// Stops the WireMockService with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the service.</param>
    public virtual void Stop(int id)
    {
        var service = _services.Single(i => i.Id.Equals(id.ToString()
            , StringComparison.InvariantCultureIgnoreCase));

        service.Stop();
    }

    /// <summary>
    /// Asynchronously starts a WireMockService with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the service.</param>
    /// <returns>Task</returns>
    public virtual async Task StartServiceAsync(int id)
    {
        var service = _services.Single(i => i.Id.Equals(id.ToString()
            , StringComparison.InvariantCultureIgnoreCase));

        var models = await _repo.GetModelsAsync();

        var m = models.Single(model => model.Id == id);
        service.CreateAndStart(m.Mappings);
    }

    /// <summary>
    /// Saves the mappings to the database context.
    /// </summary>
    /// <param name="e">The event arguments containing the service ID and mapping GUIDs.</param>
    [ExcludeFromCodeCoverage]
    private async Task SaveMappingToContextAsync(ChangedMappingsArgs e)
    => await _repo.AddMappingsAsync(int.Parse(e.ServiceId), e.MappingModels.Select(mm
            => new Tuple<Guid, string>(mm.Guid!.Value, mm.ToJson())));
    

    [ExcludeFromCodeCoverage]
    private void RemoveMappingFromContext(object? sender, ChangedMappingsArgs e)
      => _repo.RemoveMappingsAsync(e.MappingModels.Select(mm => mm.Guid!.Value));
    

    private async Task CreateServicesAsync()
    {
        var models = await _repo.GetModelsAsync();
        _services.AddRange(models.Select(CreateService));
    }

    /// <summary>
    /// Creates a new WireMockService based on the provided WireMockServiceModel
    /// </summary>
    /// <param name="model">The WireMockServiceModel used to create the service.</param>
    private static WireMockService CreateService(WireMockServiceModel model)
        => new (model);

    /// <summary>
    /// Checks if a service with the provided service Id is currently running
    /// </summary>
    /// <param name="serviceId">the service id of the service which gets checked</param>
    public virtual bool IsRunning(int serviceId)
    {
        var service = _services.FirstOrDefault(s 
            => s.Id.Equals(serviceId.ToString()));
        return service?.IsRunning ?? false;
    }
}