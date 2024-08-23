namespace WireMock.Server.Interfaces;

public interface IOrchestrator
{
    void RemoveService(int serviceId);
    void Stop(int serviceId);
    Task CreateServiceAsync(int serviceId);
    Task<IList<WireMockService>> GetOrCreateServicesAsync();
    Task StartServiceAsync(int serviceId);
    bool IsRunning(int serviceId);
}