using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WireMock.Server;

namespace WireMock.Pages_WireMockServers
{
    public class IndexModel : PageModel
    {
        private readonly IDbContextFactory _contextFactory;

        public IndexModel(IDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public IList<WireMockServerModel> WireMockServerModel { get;set; } = default!;

        public async Task OnGetAsync()
        {
            var context = _contextFactory.CreateDbContext();
            WireMockServerModel = await context.WireMockServerModel.ToListAsync();
        }
    }
}
