using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WireMock.Data;
using WireMock.Server.Interfaces;

namespace WireMock.Pages.WireMockService;

public class EditModel(IWireMockRepository Repository, IOrchestrator ServiceOrchestrator)
    : PageModel
{
    [BindProperty] public WireMockServiceModel WireMockServiceModel { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        WireMockServiceModel = await Repository.GetModelAsync(id);
        return Page();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await Repository.UpdateModelAsync(WireMockServiceModel);
            ServiceOrchestrator.Stop(WireMockServiceModel.Id);
            ServiceOrchestrator.RemoveService(WireMockServiceModel.Id);
            await ServiceOrchestrator.CreateServiceAsync(WireMockServiceModel.Id);
        }
        catch (DbUpdateConcurrencyException)
        {
            return NotFound();
        }

        return RedirectToPage("../Index");
    }
}