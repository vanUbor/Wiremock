using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WireMock.Data;
using WireMock.Server;

namespace WireMock.Pages;

public class Server : PageModel
{
    [BindProperty] public IList<WireMock.Server.WireMockService> Servers { get; set; } = default!;

    private readonly IWireMockRepository _repository;
    private readonly ServiceOrchestrator _serviceOrchestrator;

    public Server(IWireMockRepository repository, ServiceOrchestrator serviceOrchestrator)
    {
        _repository = repository;
        _serviceOrchestrator = serviceOrchestrator;
    }

    public async Task<IActionResult> OnGet()
    {
        Servers = await _serviceOrchestrator.GetOrCreateServicesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _repository.RemoveModelAsync(id);
        _serviceOrchestrator.Stop(id);
        _serviceOrchestrator.RemoveService(id);
        
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostStartAsync(int? id)
    {
        if (id == null)
            return NotFound($"No service with id {id} found");

        await _serviceOrchestrator.StartAsync(id.Value);

        return RedirectToPage();
    }

    public IActionResult OnPostStopAsync(int? id)
    {
        if (id == null)
            return NotFound($"No service with id {id} found");

        _serviceOrchestrator.Stop(id.Value);

        return RedirectToPage();
    }
}