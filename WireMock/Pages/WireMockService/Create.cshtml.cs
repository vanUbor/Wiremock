using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WireMock.Data;
using WireMock.Server;

namespace WireMock.Pages.WireMockService
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
        public WireMockServiceModel WireMockServiceModel { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("../Error");
            }

            await _repository.AddModelAsync(WireMockServiceModel);

            return RedirectToPage("../Index");
        }
    }
}
