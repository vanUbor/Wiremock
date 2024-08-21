using WireMock.Data;

namespace WireMock.Server.Interfaces;

public interface IWireMockRepository
{
    Task AddModelAsync(WireMockServiceModel model);
    Task<WireMockServiceModel> GetModelAsync(int id);
    Task<IList<WireMockServiceModel>> GetModelsAsync();
    Task UpdateModelAsync(WireMockServiceModel model);
    Task RemoveModelAsync(int id);
    
    // Mappings
    Task<IEnumerable<WireMockServerMapping>> GetMappingsAsync(int id);
    Task<WireMockServerMapping> UpdateMappingAsync(WireMockServerMapping updatedMapping);
    Task<bool> CheckModelExistsAsync(int id);
    Task AddMappingsAsync(IEnumerable<WireMockServerMapping> newMappings);
    Task UpdateMappingsAsync(IEnumerable<Tuple<Guid, string>> mappings);
    Task RemoveMappingsAsync(IEnumerable<Guid> guids);
}