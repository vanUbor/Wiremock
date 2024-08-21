using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WireMock.Data;
using WireMock.Server.Interfaces;

namespace WireMock.Pages.WireMockService;

public class DetailsModel(IWireMockRepository Repository, IOrchestrator Orchestrator) : PageModel
{
    public WireMockServiceModel WireMockServiceModel { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int serviceId)
    {
        WireMockServiceModel = await Repository.GetModelAsync(serviceId);
        return Page();
    }

    /// <summary>
    /// Determines if the service with the specified ID is running.
    /// Used to set a class in the cshtml to disabled if a service is not running
    /// </summary>
    /// <param name="serviceId">The ID of the service.</param>
    /// <returns>The status of the service. An empty string if running, "disabled" otherwise.</returns>
    public string IsRunning(int serviceId)
    {
        try
        {
            return Orchestrator.IsRunning(serviceId)
                ? ""
                : "disabled";
        }
        catch (Exception e)
        {
            Console.WriteLine($"Could not determine if service with id {serviceId} is running: {e.Message}");
            Console.WriteLine(e.ToString());
            return "disabled";
        }
    }
}