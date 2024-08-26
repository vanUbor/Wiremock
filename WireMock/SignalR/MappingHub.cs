using Microsoft.AspNetCore.SignalR;

namespace WireMock.SignalR;

public class MappingHub : Hub
{
    public async Task SendMappingUpdate()
        => await Clients.All.SendAsync("ReceiveMappingUpdate");
}