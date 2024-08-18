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

    private const string _errorPagePath = "../Error";
    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            WireMockServiceModel = await Repository.GetModelAsync(id);
            return Page();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Invalid operation: {ex.Message}");
            return RedirectToPage(_errorPagePath);
        }
        catch (DbUpdateException ex)
        {
            // Log the exception
            Console.WriteLine($"Database update error: {ex.Message}");
            return RedirectToPage(_errorPagePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unhandled exception: {ex.Message}");
            return RedirectToPage(_errorPagePath);
        }
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
        catch (DbUpdateConcurrencyException ex)
        {
            // Log the exception
            Console.WriteLine($"Concurrency error: {ex.Message}");
            return RedirectToPage(_errorPagePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unhandled Exception: {ex.Message}");
            return RedirectToPage(_errorPagePath);
        }

        return RedirectToPage("../Index");
    }
}