namespace WireMock.Server.Interfaces;

public interface IOrchestrator
{
    void RemoveService(int id);
    void Stop(int id);
    Task CreateServiceAsync(int id);
    Task<IList<WireMockService>> GetOrCreateServicesAsync();
    Task StartServiceAsync(int id);
    bool IsRunning(int serviceId);
}