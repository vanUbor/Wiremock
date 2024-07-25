using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WireMock.Data;
using WireMock.Server;

namespace WireMock.Pages.WireMockService;

public class Mappings(IWireMockRepository Repository) : PageModel
{
    [BindProperty] public WireMockMappingModel[] Maps { get; set; } = [];
    [BindProperty] public string MapJsonContent { get; set; } = string.Empty;

    public async Task<IActionResult> OnGet(int id)
    {
        var wireMockServerModel = await Repository.GetModelAsync(id);
        await GetMappings(wireMockServerModel);
        return Page();
    }

    private async Task GetMappings(WireMockServiceModel model)
    {
        var client = new HttpClient();
        var response = await client.GetAsync($"http://localhost:{model.Port}/__admin/mappings");
        if (!response.IsSuccessStatusCode)
            return;


        var mappingString = await response.Content.ReadAsStringAsync();
        Maps = JsonSerializer.Deserialize<WireMockMappingModel[]>(mappingString) ?? [];
        foreach (var map in Maps)
        {
            map.Raw = JsonSerializer.Serialize(map, new JsonSerializerOptions() { WriteIndented = true });
        }
    }

    public async Task<IActionResult> OnPostSaveAndUpdate(string id, string guid, string raw)
    {
        var model = JsonSerializer.Deserialize<WireMockMappingModel>(raw);
        var wireMockServerModel = await Repository.GetModelAsync(int.Parse(id));
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
            return RedirectToPage(new { id });

        // if successful set, save to DB
        if (Guid.TryParse(guid, out var guidObj))
        {
            await Repository.UpdateMappingAsync(guidObj, raw);
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostResetMapping(string id, string guid)
    {
        var wireMockServerModel = await Repository.GetModelAsync(int.Parse(id));
        var client = new HttpClient();
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"http://localhost:{wireMockServerModel.Port}/__admin/mappings/{guid}"),
        };
        var response = await client.SendAsync(request);
        if (response.IsSuccessStatusCode)
            return RedirectToPage(new { id });
        return RedirectToPage("../Error");
    }

    public async Task<IActionResult> OnPostResetAllMappings(string id)
    {
        var wireMockServerModel = await Repository.GetModelAsync(int.Parse(id));
        var client = new HttpClient();
        var context = new StringContent(string.Empty);
        await client.PostAsync($"http://localhost:{wireMockServerModel.Port}/__admin/mappings/reset",
            context);
        
        return RedirectToPage(new { id });
    }
}