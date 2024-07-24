using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WireMock.Data;
using WireMock.Server;

namespace WireMock.Pages.WireMockServers;

public class Mappings : PageModel
{
    private readonly IWireMockRepository _repository;

    [BindProperty] public WireMockMappingModel[]? Maps { get; set; }
    [BindProperty] public string MapJsonContent { get; set; }

    public Mappings(IWireMockRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> OnGet(int id)
    {
        var wireMockServerModel = await _repository.GetModelAsync(id);
        await GetMappings(wireMockServerModel);
        return Page();
    }

    private async Task GetMappings(WireMockServiceModel model)
    {
        var client = new HttpClient();
        var response = await client.GetAsync($"http://localhost:{model.Port}/__admin/mappings");
        if (!response.IsSuccessStatusCode)
        {
            Maps = Array.Empty<WireMockMappingModel>();
            return;
        }
        
        var mappingString = await response.Content.ReadAsStringAsync();
        Maps = JsonSerializer.Deserialize<WireMockMappingModel[]>(mappingString);
        foreach (var map in Maps)
        {
            map.Raw = JsonSerializer.Serialize(map, new JsonSerializerOptions() { WriteIndented = true });
        }
    }

    public async Task<IActionResult> OnPostSaveAndUpdate(string id, string guid, string raw)
    {
        var model = JsonSerializer.Deserialize<WireMockMappingModel>(raw);
        var wireMockServerModel = await _repository.GetModelAsync(int.Parse(id));
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
            await _repository.UpdateMappingAsync(guidObj, raw);
        }

        return RedirectToPage(new { id = id });
    }

    public async Task<IActionResult> OnPostResetMapping(string id, string guid)
    {
        var wireMockServerModel = await _repository.GetModelAsync(int.Parse(id));
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
        var wireMockServerModel = await _repository.GetModelAsync(int.Parse(id));
        var client = new HttpClient();
        var context = new StringContent(string.Empty);
        await client.PostAsync($"http://localhost:{wireMockServerModel.Port}/__admin/mappings/reset",
            context);
        return RedirectToPage(new { id = id });
    }
}