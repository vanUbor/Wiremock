using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;

namespace WireMock.Server;

public class ServerOrchestrator
{
    private ILogger _logger;
    private WireMockServerContext _context;
    private IList<WireMockService> _services = default!;
    
    public ServerOrchestrator(ILogger<ServerOrchestrator> logger, WireMockServerContext context)
    {
        _context = context;
    }

    public async Task<IList<WireMockService>> GetServicesAsync()
    {
        var models = await _context.WireMockServerModel.ToListAsync();
        return models.Select(s 
            => new WireMockService(_logger, s)
        ).ToList();
    }
}