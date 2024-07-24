using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WireMock.Server;

namespace WireMock.Pages_WireMockServers
{
    public class IndexModel : PageModel
    {
        public IList<WireMockServerModel> WireMockServerModel { get;set; } = default!;
        
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
