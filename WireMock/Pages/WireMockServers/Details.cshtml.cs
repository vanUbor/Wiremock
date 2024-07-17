using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WireMock.Server;

namespace WireMock.Pages_WireMockServers
{
    public class DetailsModel : PageModel
    {
        private readonly IDbContextFactory _contextFactory;

        public DetailsModel(IDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public WireMockServerModel WireMockServerModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var context = _contextFactory.CreateDbContext();
            var wireMockServerModel = await context.WireMockServerModel.FirstOrDefaultAsync(m => m.Id == id);
            if (wireMockServerModel == null)
            {
                return NotFound();
            }

            WireMockServerModel = wireMockServerModel;

            return Page();
        }
    }
}