using Microsoft.EntityFrameworkCore;
using WireMock.Server;

namespace WireMock.Data;

public class WireMockRepository(
    IDbContextFactory<WireMockServerContext> ContextFactory)
    : IWireMockRepository
{
    public async Task AddModelAsync(WireMockServiceModel model)
    {
        await using var context = await ContextFactory.CreateDbContextAsync();
        await context.WireMockServerModel.AddAsync(model);
        await context.SaveChangesAsync();
    }

    public async Task<WireMockServiceModel> GetModelAsync(int id)
    {
        await using var context = await ContextFactory.CreateDbContextAsync();
        return await context.WireMockServerModel.FirstAsync(m => m.Id == id);
    }

    public async Task<IList<WireMockServiceModel>> GetModelsAsync()
    {
        await using var context = await ContextFactory.CreateDbContextAsync();
        return await context.WireMockServerModel.ToListAsync();
    }

    public async Task UpdateModelAsync(WireMockServiceModel model)
    {
        await using var context = await ContextFactory.CreateDbContextAsync();
        context.WireMockServerModel.Update(model);
        await context.SaveChangesAsync();
    }

    public async Task RemoveModelAsync(int id)
    {
        await using var context = await ContextFactory.CreateDbContextAsync();
        var model = await context.WireMockServerModel.FirstOrDefaultAsync(m => m.Id == id);
        if (model != null)
        {
            context.WireMockServerModel.Remove(model);
            await context.SaveChangesAsync();
        }
    }
    

    public async Task UpdateMappingAsync(Guid guid, string raw)
    {
        
            await using var context = await ContextFactory.CreateDbContextAsync();
            var existingMapping = await context.WireMockServerMapping.FirstOrDefaultAsync(m => m.Guid == guid);

            if (existingMapping != null)
            {
                existingMapping.Raw = raw;

                context.WireMockServerMapping.Update(existingMapping);
                await context.SaveChangesAsync();
            }
    }
    
    public async Task<bool> CheckModelExistsAsync(int id)
    {
        await using var context = await ContextFactory.CreateDbContextAsync();
        return await context.WireMockServerModel.AnyAsync(x => x.Id == id);
    }
}