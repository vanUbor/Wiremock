using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WireMock.Server;

namespace WireMock.Pages_WireMockServers
{
    public class CreateModel : PageModel
    {
        private readonly WireMockServerContext _context;

        public CreateModel(WireMockServerContext context)
        {
            _context = context;
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
                return Page();
            }

            _context.WireMockServerModel.Add(WireMockServerModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("../Server");
        }
    }
}
