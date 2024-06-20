using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WireMock.Server;

namespace WireMock.Pages_WireMockServers
{
    public class EditModel : PageModel
    {
        private readonly WireMock.Server.WireMockServerContext _context;

        public EditModel(WireMock.Server.WireMockServerContext context)
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

            var wiremockservermodel =  await _context.WireMockServerModel.FirstOrDefaultAsync(m => m.Id == id);
            if (wiremockservermodel == null)
            {
                return NotFound();
            }
            WireMockServerModel = wiremockservermodel;
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

            _context.Attach(WireMockServerModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WireMockServerModelExists(WireMockServerModel.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("../Server");
        }

        private bool WireMockServerModelExists(int id)
        {
            return _context.WireMockServerModel.Any(e => e.Id == id);
        }
    }
}
