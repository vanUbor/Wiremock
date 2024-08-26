namespace WireMock.Server.Interfaces;

public interface IOrchestrator
{
    /// <summary>
    /// Is raised when the mappings of a service have changed
    /// </summary>
    event EventHandler<EventArgs>? MappingsChanged;
    void RemoveService(int serviceId);  
    void Stop(int serviceId);
    Task CreateServiceAsync(int serviceId);
    Task<IList<WireMockService>> GetOrCreateServicesAsync();
    Task StartServiceAsync(int serviceId);
    bool IsRunning(int serviceId);
}