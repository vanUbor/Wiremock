using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WireMock.Server;

namespace WireMock.Pages_WireMockServers
{
    public class CreateModel : PageModel
    {
        private readonly IDbContextFactory _contextFactory;

        public CreateModel(IDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
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
            var context = _contextFactory.CreateDbContext();
            context.WireMockServerModel.Add(WireMockServerModel);
            await context.SaveChangesAsync();

            return RedirectToPage("../Server");
        }
    }
}
