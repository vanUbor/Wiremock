using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WireMock.Data;
using WireMock.Server;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WireMock.Pages.WireMockService;

public class Mappings(IWireMockRepository Repository) : PageModel
{
    public int ServiceId { get; set; }
    public string GuidSort { get; set; }
    public string TitleSort { get; set; }
    public string DateSort { get; set; }
    
    [BindProperty] public IList<WireMockMappingModel> Maps { get; set; }
    [BindProperty] public string MapJsonContent { get; set; } = string.Empty;
    
    public async Task<IActionResult> OnGet(int id, string sortOrder)
    {
        ServiceId = id;
        GuidSort = String.IsNullOrEmpty(sortOrder) ? "guid_desc" : "";
        TitleSort = sortOrder == "title" ? "title_desc" : "title";
        DateSort = sortOrder == "date" ? "date_desc" : "date";
        
        var wireMockServerModel = await Repository.GetModelAsync(id);
        var mappings = await GetMappings(wireMockServerModel);
        Maps = SetMapsOrdered(mappings, sortOrder).ToList();
        
        return Page();
    }

    private async Task<IList<WireMockMappingModel>> GetMappings(WireMockServiceModel model)
    {
        var client = new HttpClient();
        var response = await client.GetAsync($"http://localhost:{model.Port}/__admin/mappings");
        if (!response.IsSuccessStatusCode)
            return Array.Empty<WireMockMappingModel>();


        var mappingString = await response.Content.ReadAsStringAsync();
        var maps = JsonSerializer.Deserialize<WireMockMappingModel[]>(mappingString) ?? [];
        foreach (var map in maps)
        {
            map.Raw = JsonSerializer.Serialize(map, new JsonSerializerOptions() { WriteIndented = true });
        }

        return maps;
    }

    private IEnumerable<WireMockMappingModel> SetMapsOrdered(IList<WireMockMappingModel> mappings, string sortOrder)
    {
        switch (sortOrder)
        {
            case "date":
                return mappings.OrderBy(m => m.UpdatedAt);
            case "date_desc":
                return mappings.OrderByDescending(m => m.UpdatedAt);
            case "title":
                return mappings.OrderBy(m => m.Title);
            case "title_desc":
                return mappings.OrderByDescending(m => m.Title);
            case "guid_desc":
                return mappings.OrderByDescending(m => m.Guid);
            default:
                return mappings.OrderBy(m => m.Guid);
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