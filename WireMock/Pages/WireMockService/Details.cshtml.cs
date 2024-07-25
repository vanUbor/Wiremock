using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WireMock.Data;
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

        public WireMockServiceModel WireMockServiceModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            WireMockServiceModel = await _repository.GetModelAsync(id);
            return Page();
        }
    }
}