namespace WireMock.Server;

public interface IWireMockRepository
{
    Task AddModelAsync(WireMockServerModel model);
    Task<WireMockServerModel> GetModelAsync(int id);
    Task<IList<WireMockServerModel>> GetModelsAsync();
    Task UpdateModelAsync(WireMockServerModel model);
    Task RemoveModelAsync(int id);
    Task UpdateMappingAsync(Guid guid, string raw);
}