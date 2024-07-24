using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WireMock.Server;

namespace WireMock.Pages_WireMockServers
{
    public class CreateModel : PageModel
    {
        private readonly IWireMockRepository _repository;
        
        public CreateModel(IWireMockRepository repository)
        {
            _repository = repository;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public WireMockServerModel WireMockServerModel { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("../Error");
            }

            await _repository.AddModelAsync(WireMockServerModel);

            return RedirectToPage("../Server");
        }
    }
}
