using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WireMock.Server;

namespace WireMock.Pages_WireMockServers
{
    public class DetailsModel : PageModel
    {
        private readonly IWireMockRepository _repository;

        public DetailsModel(IWireMockRepository repository)
        {
            _repository = repository;
        }

        public WireMockServerModel WireMockServerModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            WireMockServerModel = await _repository.GetModelAsync(id);
            return Page();
        }
    }
}