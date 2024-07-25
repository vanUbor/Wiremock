using Microsoft.EntityFrameworkCore;
using WireMock.Server;

namespace WireMock.Data;

public class WireMockRepository : IWireMockRepository
{
    private readonly ILogger<IWireMockRepository> _logger;
    private readonly IDbContextFactory<WireMockServerContext> _contextFactory;

    public WireMockRepository(ILogger<IWireMockRepository> logger, IDbContextFactory<WireMockServerContext> contextFactory)
    {
        _logger = logger;
        _contextFactory = contextFactory;
    }

    public async Task AddModelAsync(WireMockServiceModel model)
    {
        await using var context = _contextFactory.CreateDbContext();
        await context.WireMockServerModel.AddAsync(model);
        await context.SaveChangesAsync();
    }

    public async Task<WireMockServiceModel> GetModelAsync(int id)
    {
        await using var context = _contextFactory.CreateDbContext();
        return await context.WireMockServerModel.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IList<WireMockServiceModel>> GetModelsAsync()
    {
        await using var context = _contextFactory.CreateDbContext();
        return await context.WireMockServerModel.ToListAsync();
    }

    public async Task UpdateModelAsync(WireMockServiceModel model)
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
    
    public async Task<bool> CheckModelExistsAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.WireMockServerModel.AnyAsync(x => x.Id == id);
    }
}