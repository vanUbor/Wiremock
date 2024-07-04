using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WireMock.Server;

namespace WireMock.Pages.WireMockServers;

public class Mappings : PageModel
{
    private readonly IDbContextFactory _contextFactory;
    
    [BindProperty] public WireMockModel[]? Maps { get; set; }

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

        await GetMappings(wireMockServerModel); 
        return Page();
    }

    private async Task GetMappings(WireMockServerModel model)
    {
        var client = new HttpClient();
        var response = await client.GetAsync($"http://localhost:{model.Port}/__admin/mappings");
        var mappingString = await response.Content.ReadAsStringAsync();

        Maps = JsonSerializer.Deserialize<WireMockModel[]>(mappingString);
        foreach (var map in Maps)
        {
            map.Raw = JsonSerializer.Serialize(map, new JsonSerializerOptions(){WriteIndented = true});
        }
    }
}