using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WireMock.Server;

namespace WireMock.Pages.WireMockServers;

public class Mappings : PageModel
{
    private readonly IDbContextFactory _contextFactory;
    
    [BindProperty] public string Maps { get; set; }

    public Mappings(IDbContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IActionResult> OnGet(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var context = _contextFactory.CreateDbContext();
        var wireMockServerModel = await context.WireMockServerModel.FirstOrDefaultAsync(m => m.Id == id);
        if (wireMockServerModel == null)
        {
            return NotFound();
        }

        Maps = await GetMappings(wireMockServerModel); 
        return Page();
    }

    private async Task<string> GetMappings(WireMockServerModel model)
    {
        var client = new HttpClient();
        var response = await client.GetAsync($"http://localhost:{model.Port}/__admin/mappings");
        return await response.Content.ReadAsStringAsync();
    }
}