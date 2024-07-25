using WireMock.Data;

namespace WireMock.Server;

public interface IWireMockRepository
{
    Task AddModelAsync(WireMockServiceModel model);
    Task<WireMockServiceModel> GetModelAsync(int id);
    Task<IList<WireMockServiceModel>> GetModelsAsync();
    Task UpdateModelAsync(WireMockServiceModel model);
    Task RemoveModelAsync(int id);
    Task UpdateMappingAsync(Guid guid, string raw);
    Task<bool> CheckModelExistsAsync(int id);
}