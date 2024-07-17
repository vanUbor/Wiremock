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
        Servers = await _serverOrchestrator.GetOrCreateServicesAsync();
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
            _serverOrchestrator.Stop(id);
            _serverOrchestrator.RemoveService(id);
        }

        return RedirectToPage();
    }

    public IActionResult OnPostStartAsync(int? id)
    {
        if (id == null)
            return NotFound($"No service with id {id} found");

        _serverOrchestrator.Start(id.Value);
        
        return RedirectToPage();
    }
    
    public IActionResult OnPostStopAsync(int? id)
    {
        if (id == null)
            return NotFound($"No service with id {id} found");

        _serverOrchestrator.Stop(id.Value);
        
        return RedirectToPage();
    }
}