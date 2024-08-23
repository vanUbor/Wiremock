using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WireMock.Data;
using WireMock.Server.Interfaces;

namespace WireMock.Pages.WireMockService;

public class DetailsModel(IWireMockRepository Repository) : PageModel
{
    public WireMockServiceModel WireMockServiceModel { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int serviceId)
    {
        WireMockServiceModel = await Repository.GetModelAsync(serviceId);
        return Page();
    }
}