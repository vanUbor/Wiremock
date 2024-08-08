using EFCore.BulkExtensions;
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

    /// <summary>
    /// Adds new mappings to a WireMock service identified by the given service ID.
    /// If the mapping already exists in the service, it will be skipped.
    /// </summary>
    /// <param name="serviceId">The ID of the WireMock service</param>
    /// <param name="newMappings">A collection of Tuple consisting of Guid and string representing the new mappings to be added</param>
    public async Task AddMappingsAsync(int serviceId, IEnumerable<Tuple<Guid, string>> newMappings)
    {
        await using var context = await ContextFactory.CreateDbContextAsync();
        var service = context.WireMockServerModel
            .Include(wireMockServiceModel => wireMockServiceModel.Mappings)
            .Single(m
                => m.Id == serviceId);

        var existingMappings = service.Mappings.ToList();
        var mappingsToAdd =
            (from newMapping in newMappings
                where !existingMappings.Any(existingMapping
                    => existingMapping.Guid.Equals(newMapping.Item1))
                select new WireMockServerMapping
                    { Guid = newMapping.Item1, Raw = newMapping.Item2 })
            .ToList();

        if (mappingsToAdd.Count != 0)
            await context.BulkInsertAsync(mappingsToAdd);

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates the raw content of an existing mapping identified by the given GUID.
    /// If the mapping does not exist, the method does nothing.
    /// </summary>
    /// <param name="guid">The GUID identifying the mapping</param>
    /// <param name="raw">The updated raw content of the mapping</param>
    public async Task UpdateMappingAsync(Guid guid, string raw)
    {
        await using var context = await ContextFactory.CreateDbContextAsync();
        var existingMapping = await context.WireMockServerMapping
            .FirstOrDefaultAsync(m => m.Guid == guid);

        if (existingMapping != null)
        {
            existingMapping.Raw = raw;

            context.WireMockServerMapping.Update(existingMapping);
            await context.SaveChangesAsync();
        }
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
        var existingMappings = context.WireMockServerMapping.ToList();

        foreach (var updatedMapping in mappings)
        {
            if (!existingMappings.Any(existingMapping => existingMapping.Guid.Equals(updatedMapping.Item1)))
                continue;

            existingMappings.First(existingMapping => existingMapping.Guid.Equals(updatedMapping.Item1))
                .Raw = updatedMapping.Item2;
        }

        await context.SaveChangesAsync();
    }

    public async Task RemoveMappingsAsync(IEnumerable<Guid> guids)
    {
        await using var context = await ContextFactory.CreateDbContextAsync();
        var mappingsToRemove = context.WireMockServerMapping
            .ToList()
            .Where(m => guids.Any(guid => guid.Equals(m.Guid)));

        await context.BulkDeleteAsync(mappingsToRemove);
        // context.WireMockServerMapping.RemoveRange(mappingsToRemove);

        await context.SaveChangesAsync();
    }
}