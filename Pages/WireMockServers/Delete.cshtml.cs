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
        private readonly WireMock.Server.WireMockServerContext _context;

        public DeleteModel(WireMock.Server.WireMockServerContext context)
        {
            _context = context;
        }

        [BindProperty]
        public WireMockServerModel WireMockServerModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wiremockservermodel = await _context.WireMockServerModel.FirstOrDefaultAsync(m => m.Id == id);

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

            var wiremockservermodel = await _context.WireMockServerModel.FindAsync(id);
            if (wiremockservermodel != null)
            {
                WireMockServerModel = wiremockservermodel;
                _context.WireMockServerModel.Remove(WireMockServerModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
