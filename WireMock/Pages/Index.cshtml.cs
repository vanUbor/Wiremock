using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WireMock.Data;
using WireMock.Server.Interfaces;

namespace WireMock.Pages;

public class Index(IWireMockRepository Repository, ServiceOrchestrator ServiceOrchestrator)
    : PageModel
{
    [BindProperty] public IList<WireMock.Server.WireMockService> Servers { get; set; } = default!;

    public async Task<IActionResult> OnGet()
    {
        Servers = await ServiceOrchestrator.GetOrCreateServicesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await Repository.RemoveModelAsync(id);
        ServiceOrchestrator.Stop(id);
        ServiceOrchestrator.RemoveService(id);
        
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostStartAsync(int? id)
    {
        if (id == null)
            return NotFound($"No service with id {id} found");

        await ServiceOrchestrator.StartServiceAsync(id.Value);

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostStopAsync(int? id)
    {
        if (id == null)
            return NotFound($"No service with id {id} found");

        await Task.Run(() => ServiceOrchestrator.Stop(id.Value));

        return RedirectToPage();
    }
}