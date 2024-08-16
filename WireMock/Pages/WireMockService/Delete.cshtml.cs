using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WireMock.Data;
using WireMock.Server.Interfaces;

namespace WireMock.Pages.WireMockService
{
    public class DeleteModel(IWireMockRepository Repository) : PageModel
    {
        [BindProperty]
        public WireMockServiceModel WireMockServiceModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            WireMockServiceModel = await Repository.GetModelAsync(id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            await Repository.RemoveModelAsync(id);
            return RedirectToPage("./Index");
        }
    }
}
