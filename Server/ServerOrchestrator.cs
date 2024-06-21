using Microsoft.EntityFrameworkCore;

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

    public async Task<IList<WireMockService>> GetServicesAsync()
    {
        var context = _contextFactory.CreateDbContext();
        var models = await context.WireMockServerModel.ToListAsync();
        _services = models.Select(s 
            => new WireMockService(_logger, s)
        ).ToList();
        return _services;
    }

    internal async Task Start(int id)
    {
        var service = _services.Single(i => i.Id.Equals(id.ToString()
            , StringComparison.InvariantCultureIgnoreCase));
        
        service.Start();

        var context = _contextFactory.CreateDbContext();
        await context.SaveChangesAsync();
    }
}