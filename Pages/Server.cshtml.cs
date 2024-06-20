using Microsoft.AspNetCore.Mvc.RazorPages;
using WireMock.Server;

namespace WireMock.Pages;

public class Server : PageModel
{

    public IList<WireMockServer> Servers { get; set; }
    
    public void OnGet()
    {
        Servers = new List<WireMockServer>(3)
        {
            new WireMockServer() { Id = "First", Name ="MapDB", IsRunning = true },
            new WireMockServer() { Id = "Second", Name ="AdminApi", IsRunning = false },
            new WireMockServer() { Id = "3333", Name ="TrimGen API", IsRunning = true }
        };
    }
}