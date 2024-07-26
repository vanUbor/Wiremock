using Microsoft.AspNetCore.Mvc.RazorPages;
using WireMock.Data;
using WireMock.Server;

namespace WireMock.Pages.WireMockService
{
    public class IndexModel : PageModel
    {
        public IList<WireMockServiceModel> WireMockServerModel { get;set; } = default!;
        
        private readonly IWireMockRepository _repository;

        public IndexModel(IWireMockRepository repository)
        {
            _repository = repository;
        }


        public async Task OnGetAsync()
        {
            WireMockServerModel = await _repository.GetModelsAsync();
        }
    }
}
