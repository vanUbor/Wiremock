using Microsoft.EntityFrameworkCore;

namespace WireMock.Server;

public class WireMockRepository : IWireMockRepository
{
    private readonly ILogger<IWireMockRepository> _logger;
    private readonly IDbContextFactory _contextFactory;

    public WireMockRepository(ILogger<IWireMockRepository> logger, IDbContextFactory contextFactory)
    {
        _logger = logger;
        _contextFactory = contextFactory;
    }

    public async Task AddModelAsync(WireMockServerModel model)
    {
        await using var context = _contextFactory.CreateDbContext();
        await context.WireMockServerModel.AddAsync(model);
        await context.SaveChangesAsync();
    }

    public async Task<WireMockServerModel> GetModelAsync(int id)
    {
        await using var context = _contextFactory.CreateDbContext();
        return await context.WireMockServerModel.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IList<WireMockServerModel>> GetModelsAsync()
    {
        await using var context = _contextFactory.CreateDbContext();
        return await context.WireMockServerModel.ToListAsync();
    }

    public async Task UpdateModelAsync(WireMockServerModel model)
    {
        await using var context = _contextFactory.CreateDbContext();
        context.WireMockServerModel.Update(model);
        await context.SaveChangesAsync();
    }

    public async Task RemoveModelAsync(int id)
    {
        await using var context = _contextFactory.CreateDbContext();
        var model = await context.WireMockServerModel.FirstOrDefaultAsync(m => m.Id == id);
        if (model != null)
        {
            context.WireMockServerModel.Remove(model);
            await context.SaveChangesAsync();
        }
    }
    

    public async Task UpdateMappingAsync(Guid guid, string raw)
    {
        
            await using var context = _contextFactory.CreateDbContext();
            var existingMapping = await context.WireMockServerMapping.FirstOrDefaultAsync(m => m.Guid == guid);

            if (existingMapping != null)
            {
                existingMapping.Raw = raw;

                context.WireMockServerMapping.Update(existingMapping);
                await context.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning("No mapping found with the provided Guid: {Guid}", guid);
            }
        
    }
}