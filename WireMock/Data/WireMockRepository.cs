using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using WireMock.Server.Interfaces;

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
        return await context.WireMockServerModel.Include(wireMockServiceModel
            => wireMockServiceModel.Mappings).FirstAsync(m => m.Id == id);
    }

    public async Task<IList<WireMockServiceModel>> GetModelsAsync()
    {
        await using var context = await ContextFactory.CreateDbContextAsync();
        return await context.WireMockServerModel.Include(wireMockServiceModel
            => wireMockServiceModel.Mappings).ToListAsync();
    }

    public async Task UpdateModelAsync(WireMockServiceModel model)
    {
        await using var context = await ContextFactory.CreateDbContextAsync();
        context.WireMockServerModel.Update(model);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Removes a WireMock service model with the given ID from the repository.
    /// </summary>
    /// <param name="id">The ID of the WireMock service model to be removed</param>
    /// <returns>Returns a task representing the asynchronous operation</returns>
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

    /// <summary>
    /// Checks if a WireMock service model with the given ID exists in the repository.
    /// </summary>
    /// <param name="id">The ID of the WireMock service model</param>
    /// <returns>Returns true if the service model exists, otherwise false</returns>
    public async Task<bool> CheckModelExistsAsync(int id)
    {
        await using var context = await ContextFactory.CreateDbContextAsync();
        return await context.WireMockServerModel.AnyAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<WireMockServerMapping>> GetMappingsAsync(int id)
    {
            await using var context = await ContextFactory.CreateDbContextAsync();
            return  await context.WireMockServerMapping
                .Where(mapping => mapping.WireMockServerModelId == id).ToListAsync();
    }

    /// <summary>
    /// Adds new mappings to a WireMock service identified by the given service ID.
    /// If the mapping already exists in the service (identified by the guid), it will be skipped.
    /// </summary>
    /// <param name="newMappings">A collection of mappings that will be added</param>
    public async Task AddMappingsAsync(IEnumerable<WireMockServerMapping> newMappings)
    {
        await using var context = await ContextFactory.CreateDbContextAsync();
        
        // get all existing Mappings
        var existingMappings = await context.WireMockServerModel
            .Include(wireMockServiceModel => wireMockServiceModel.Mappings)
            .SelectMany(s => s.Mappings)
            .ToListAsync();

        // exclude all mappings that already exist
        var mappingsToAdd =
            (from newMapping in newMappings
                where !existingMappings.Exists(existingMapping
                    => existingMapping.Guid.Equals(newMapping.Guid))
                select new WireMockServerMapping
                {
                    Guid = newMapping.Guid,
                    Raw = newMapping.Raw,
                    Title = newMapping.Title,
                    LastChange = DateTime.Now,
                    WireMockServerModelId = newMapping.WireMockServerModelId
                })
            .ToList();

        if (mappingsToAdd.Count != 0)
        {
            await context.BulkInsertAsync(mappingsToAdd);
        }

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates the raw content of an existing mapping identified by the given GUID.
    /// If the mapping does not exist, the method does nothing.
    /// </summary>
    /// <param name="updatedMapping">The mapping that will be updated</param>
    public async Task<WireMockServerMapping> UpdateMappingAsync(WireMockServerMapping updatedMapping)
    {
        await using var context = await ContextFactory.CreateDbContextAsync();
        var existingMapping = await context.WireMockServerMapping
            .FirstOrDefaultAsync(m => m.Guid == updatedMapping.Guid);

        if (existingMapping == null)
            throw new InvalidOperationException("Could not find an mapping to update");

        existingMapping.Raw = updatedMapping.Raw;
        existingMapping.LastChange = DateTime.Now;
        existingMapping.Title = updatedMapping.Title ?? existingMapping.Title;
        var entry = context.WireMockServerMapping.Update(existingMapping);
        await context.SaveChangesAsync();
        return entry.Entity;
    }

    /// <summary>
    /// Updates the mappings of WireMockServerMapping objects in the database.
    /// If the mapping with the guid does not exist in the database, it will be skipped
    /// </summary>
    /// <param name="mappings">A collection of Tuple consisting of Guid and string representing the mapping to be updated.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task UpdateMappingsAsync(IEnumerable<Tuple<Guid, string>> mappings)
    {
        await using var context = await ContextFactory.CreateDbContextAsync();
        var existingMappings = await context.WireMockServerMapping.ToListAsync();

        foreach (var updatedMapping in mappings)
        {
            if (!existingMappings.Exists(existingMapping => existingMapping.Guid.Equals(updatedMapping.Item1)))
            {
                continue;
            }

            existingMappings.First(existingMapping => existingMapping.Guid.Equals(updatedMapping.Item1))
                .Raw = updatedMapping.Item2;
        }

        await context.SaveChangesAsync();
    }

    public async Task RemoveMappingsAsync(IEnumerable<Guid> guids)
    {
        await using var context = await ContextFactory.CreateDbContextAsync();
        var mappingsToRemove = (await context.WireMockServerMapping
                .ToListAsync())
            .Where(m => guids.Any(guid => guid.Equals(m.Guid)));

        await context.BulkDeleteAsync(mappingsToRemove);

        await context.SaveChangesAsync();
    }
    
    public async Task RemoveMappingAsync(Guid guid)
    {
        await using var context = await ContextFactory.CreateDbContextAsync();
        var mappingToRemove = await context.WireMockServerMapping
            .SingleAsync(m => guid.Equals(m.Guid));

        context.WireMockServerMapping.Remove(mappingToRemove);

        await context.SaveChangesAsync();
    }
}