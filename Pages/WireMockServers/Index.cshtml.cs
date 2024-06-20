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
    public class IndexModel : PageModel
    {
        private readonly WireMock.Server.WireMockServerContext _context;

        public IndexModel(WireMock.Server.WireMockServerContext context)
        {
            _context = context;
        }

        public IList<WireMockServerModel> WireMockServerModel { get;set; } = default!;

        public async Task OnGetAsync()
        {
            WireMockServerModel = await _context.WireMockServerModel.ToListAsync();
        }
    }
}
