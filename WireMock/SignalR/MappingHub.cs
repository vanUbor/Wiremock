using Microsoft.AspNetCore.SignalR;

namespace WireMock.SignalR;

    public class MappingHub : Hub
    {
        public async Task SendMappingUpdate(string update)
        {
            await Clients.All.SendAsync("ReceiveMappingUpdate", update);
        }
    }
