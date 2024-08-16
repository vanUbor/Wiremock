using WireMock.Data;

namespace WireMock.Server.Interfaces;

public interface IWireMockRepository
{
    Task AddModelAsync(WireMockServiceModel model);
    Task<WireMockServiceModel> GetModelAsync(int id);
    Task<IList<WireMockServiceModel>> GetModelsAsync();
    Task UpdateModelAsync(WireMockServiceModel model);
    Task RemoveModelAsync(int id);
    Task UpdateMappingAsync(Guid guid, string raw);
    Task<bool> CheckModelExistsAsync(int id);
    Task AddMappingsAsync(int serviceId, IEnumerable<Tuple<Guid, string>> newMappings);
    Task UpdateMappingsAsync(IEnumerable<Tuple<Guid, string>> mappings);
    Task RemoveMappingsAsync(IEnumerable<Guid> guids);
}