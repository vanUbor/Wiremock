using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WireMock.Data;
using WireMock.Server;

namespace WireMock.Pages_WireMockServers
{
    public class DeleteModel : PageModel
    {
        private readonly IWireMockRepository _repository;
        
        public DeleteModel(IWireMockRepository repository)
        {
            _repository = repository;
        }

        [BindProperty]
        public WireMockServiceModel WireMockServiceModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            WireMockServiceModel = await _repository.GetModelAsync(id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            await _repository.RemoveModelAsync(id);
            return RedirectToPage("./Index");
        }
    }
}
