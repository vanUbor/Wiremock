using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WireMock.Server;

namespace WireMock.Pages;

public class Server : PageModel
{

    [BindProperty] public IList<WireMockService> Servers { get; set; } = default!;
    private readonly IDbContextFactory _contextFactory;
    private readonly ServerOrchestrator _serverOrchestrator;

    public Server(IDbContextFactory contextFactory, ServerOrchestrator serverOrchestrator)
    {
        _serverOrchestrator = serverOrchestrator;
        _contextFactory = contextFactory;
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

        var context = _contextFactory.CreateDbContext();
        var wiremockservermodel = await context.WireMockServerModel.FindAsync(id);
        if (wiremockservermodel != null)
        {
            context.WireMockServerModel.Remove(wiremockservermodel);
            await context.SaveChangesAsync();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostStartAsync(int? id)
    {
        if (id == null)
            return NotFound();

        await _serverOrchestrator.Start(id.Value);
        
        return RedirectToPage();
    }
}