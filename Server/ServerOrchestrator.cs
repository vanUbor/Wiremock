using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace WireMock.Server;

public class ServerOrchestrator
{
    private ILogger _logger;
    private IDbContextFactory _contextFactory;
    private IList<WireMockService> _services = default!;

    public ServerOrchestrator(ILogger<ServerOrchestrator> logger, IDbContextFactory contextFactory)
    {
        _logger = logger;
        _contextFactory = contextFactory;
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
        _services = models.Select(CreateService).ToList();
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

    internal void Start(int id)
    {
        var service = _services.Single(i => i.Id.Equals(id.ToString()
            , StringComparison.InvariantCultureIgnoreCase));

        service.CreateAndStart();
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