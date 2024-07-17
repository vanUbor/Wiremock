using System.Text;
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
    [BindProperty] public string MapJsonContent { get; set; }

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

        var wireMockServerModel = await GetModel(id.Value);

        await GetMappings(wireMockServerModel);
        return Page();
    }

    private async Task<WireMockServerModel> GetModel(int id)
    {
        var context = _contextFactory.CreateDbContext();
        var wireMockServerModel = await context.WireMockServerModel.FirstOrDefaultAsync(m
            => m.Id == id);

        if (wireMockServerModel == null)
            throw new KeyNotFoundException($"could not find server with id {id}");

        return wireMockServerModel;
    }

    private async Task GetMappings(WireMockServerModel model)
    {
        var client = new HttpClient();
        var response = await client.GetAsync($"http://localhost:{model.Port}/__admin/mappings");
        var mappingString = await response.Content.ReadAsStringAsync();

        var ipMap = JsonSerializer.Deserialize<WireMockModel[]>(mappingString);
        Maps = JsonSerializer.Deserialize<WireMockModel[]>(mappingString);
        foreach (var map in Maps)
        {
            map.Raw = JsonSerializer.Serialize(map, new JsonSerializerOptions() { WriteIndented = true });
        }
    }

    public async Task<IActionResult> OnPostSaveAndUpdate(string id, string guid, string raw)
    {
        var model = JsonSerializer.Deserialize<WireMockModel>(raw);
        var wireMockServerModel = await GetModel(int.Parse(id));
        var client = new HttpClient();
        var content = JsonContent.Create(model);
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri($"http://localhost:{wireMockServerModel.Port}/__admin/mappings/{guid}"),
            Content = content
        };
        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            return RedirectToPage(new { id = id });

        // if successful set, save to DB
        if (Guid.TryParse(guid, out var guidObj))
        {
            var context = _contextFactory.CreateDbContext();
            var mapping = context.WireMockServerMapping
                .Single(m => m.Guid == guidObj);
            mapping.Raw = raw;
            context.SaveChanges();
        }

        return RedirectToPage(new { id = id });
    }

    public async Task<IActionResult> OnPostResetMapping(string id, string guid)
    {
        var wireMockServerModel = await GetModel(int.Parse(id));
        var client = new HttpClient();
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"http://localhost:{wireMockServerModel.Port}/__admin/mappings/{guid}"),
        };
        await client.SendAsync(request);
        return RedirectToPage(new { id = id });
    }

    public async Task<IActionResult> OnPostResetAllMappings(string id)
    {
        var wireMockServerModel = await GetModel(int.Parse(id));
        var client = new HttpClient();
        var context = new StringContent(string.Empty);
        await client.PostAsync($"http://localhost:{wireMockServerModel.Port}/__admin/mappings/reset",
            context);
        return RedirectToPage(new { id = id });
    }
}