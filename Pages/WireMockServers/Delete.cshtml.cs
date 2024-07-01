using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WireMock.Server;

namespace WireMock.Pages_WireMockServers
{
    public class DeleteModel : PageModel
    {
        private readonly IDbContextFactory _contextFactory;

        public DeleteModel(IDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        [BindProperty]
        public WireMockServerModel WireMockServerModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var context = _contextFactory.CreateDbContext();
            var wiremockservermodel = await context.WireMockServerModel.FirstOrDefaultAsync(m => m.Id == id);

            if (wiremockservermodel == null)
            {
                return NotFound();
            }
            else
            {
                WireMockServerModel = wiremockservermodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var context = _contextFactory.CreateDbContext();
            var wiremockservermodel = await context.WireMockServerModel.FindAsync(id);
            if (wiremockservermodel != null)
            {
                WireMockServerModel = wiremockservermodel;
                context.WireMockServerModel.Remove(WireMockServerModel);
                await context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
