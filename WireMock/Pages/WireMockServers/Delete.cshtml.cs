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
        private readonly IWireMockRepository _repository;
        
        public DeleteModel(IWireMockRepository repository)
        {
            _repository = repository;
        }

        [BindProperty]
        public WireMockServerModel WireMockServerModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            WireMockServerModel = await _repository.GetModelAsync(id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            await _repository.RemoveModelAsync(id);
            return RedirectToPage("./Index");
        }
    }
}
