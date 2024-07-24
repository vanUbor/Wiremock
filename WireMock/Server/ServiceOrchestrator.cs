using Microsoft.EntityFrameworkCore;
using System.Linq;
using NuGet.Packaging;
using NuGet.Protocol;

namespace WireMock.Server;

public class ServiceOrchestrator
{
    private ILogger _logger;
    private IDbContextFactory _contextFactory;
    private WireMockServiceList _services = default!;

    public ServiceOrchestrator(ILogger<ServiceOrchestrator> logger, WireMockServiceList serviceList,
        IDbContextFactory contextFactory)
    {
        _services = serviceList;
        _services.MappingAdded += SaveMappingToContext;
        _services.MappingRemoved += RemoveMappingFromContext;
        _logger = logger;
        _contextFactory = contextFactory;
    }

    /// <summary>
    /// Saves the mappings to the database context.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments containing the service ID and mapping GUIDs.</param>
    private void SaveMappingToContext(object? sender, ChangedMappingsArgs e)
    {
        try
        {
            var context = _contextFactory.CreateDbContext();
            var serviceModel = context.WireMockServerModel.Single(m
                => m.Id.ToString()
                    .Equals(e.ServiceId));

            foreach (var guid in e.MapGuid)
            {
                // TODO: fix this
                // no need to save new mappings if already existing
                // this happens during the startup, no idea why
                if (serviceModel.Mappings.Any(m => m.Guid.Equals(guid.Guid)))
                    continue;
                serviceModel.Mappings.Add(new WireMockServerMapping()
                {
                    Guid = guid.Guid.Value,
                    Raw = guid.ToJson()
                });
            }

            context.SaveChanges();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }

    private void RemoveMappingFromContext(object? sender, ChangedMappingsArgs e)
    {
        var context = _contextFactory.CreateDbContext();


        List<WireMockServerMapping> mappingsToRemove = context.WireMockServerMapping
            .AsEnumerable()
            .Where(m
                => e.MapGuid.Any(iMapping => iMapping.Guid.Equals(m.Guid)))
            .ToList();

        context.WireMockServerMapping.RemoveRange(mappingsToRemove);

        context.SaveChanges();
    }

    public async Task<IList<WireMockService>> GetOrCreateServicesAsync()
    {
        if (_services is { Count: > 0 })
        {
            var context = _contextFactory.CreateDbContext();
            var models = await context.WireMockServerModel.ToListAsync();

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
        var context = _contextFactory.CreateDbContext();
        var models = await context.WireMockServerModel.ToListAsync();
        _services.AddRange(models.Select(CreateService));
    }


    public async Task CreateService(int id)
    {
        var context = _contextFactory.CreateDbContext();
        var models = await context.WireMockServerModel.ToListAsync();
        var m = models.Single(model => model.Id == id);
        _services.Add(CreateService(m));
    }

    /// <summary>
    /// Creates a WireMockService and adds it to the list of services.
    /// </summary>
    /// <returns>Void</returns>
    private WireMockService CreateService(WireMockServerModel model)
    {
        var service = new WireMockService(_logger, model);
        return service;
    }

    internal async Task Start(int id)
    {
        var service = _services.Single(i => i.Id.Equals(id.ToString()
            , StringComparison.InvariantCultureIgnoreCase));

        var context = _contextFactory.CreateDbContext();
        var models = await context.WireMockServerModel.ToListAsync();
        var mappings = await context.WireMockServerMapping.ToListAsync();
        var m = models.Single(model => model.Id == id);
        service.CreateAndStart(m);
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