using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WireMock.Server;

namespace WireMock.Pages;

public class Server : PageModel
{

    [BindProperty] public IList<WireMockService> Servers { get; set; } = default!;
    private readonly WireMockServerContext _context;
    private readonly ServerOrchestrator _serverOrchestrator;

    public Server(WireMockServerContext context, ServerOrchestrator serverOrchestrator)
    {
        _context = context;
        _serverOrchestrator = serverOrchestrator;
    }
    
    public async Task<IActionResult> OnGet()
    {
        Servers = await _serverOrchestrator.GetServicesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var wiremockservermodel = await _context.WireMockServerModel.FindAsync(id);
        if (wiremockservermodel != null)
        {
            _context.WireMockServerModel.Remove(wiremockservermodel);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage();
    }
}