using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WireMock.Data;
using WireMock.Server.Interfaces;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WireMock.Pages.WireMockService;

public class Mappings(IHttpClientFactory clientFactory,
    IOrchestrator serviceOrchestrator,
    IWireMockRepository Repository, 
    IConfiguration Config) 
    : PageModel
{
    public int ServiceId { get; set; }
    public string? GuidSort { get; set; }
    public string? TitleSort { get; set; }
    public string? DateSort { get; set; }

    public PaginatedList<WireMockMappingModel>? Maps { get; set; }

    [BindProperty] public string MapJsonContent { get; set; } = string.Empty;

    private readonly HttpClient _client = clientFactory.CreateClient();
    public async Task<IActionResult> OnGet(int id, string sortOrder, int? pageIndex)
    {
        if (!serviceOrchestrator.IsRunning(id))
        {
            return RedirectToPage("../Error");
        }
        
        ServiceId = id;
        GuidSort = string.IsNullOrEmpty(sortOrder) ? "guid_desc" : "";
        TitleSort = sortOrder == "title" ? "title_desc" : "title";
        DateSort = sortOrder == "date" ? "date_desc" : "date";

        var serviceModel = await Repository.GetModelAsync(id);
        var mappings = await GetMappings(serviceModel);

        var maps = SetMapsOrdered(mappings, sortOrder).ToList();
        var pageSize = Config.GetValue("PageSize", 4);
        Maps = PaginatedList<WireMockMappingModel>.CreatePage(maps, pageIndex ?? 1, pageSize);

        return Page();
    }

    private async Task<IList<WireMockMappingModel>> GetMappings(WireMockServiceModel model)
    {
        var response = await _client.GetAsync($"http://localhost:{model.Port}/__admin/mappings");
        if (!response.IsSuccessStatusCode)
            return Array.Empty<WireMockMappingModel>();


        var mappingString = await response.Content.ReadAsStringAsync();
        var maps = JsonSerializer.Deserialize<WireMockMappingModel[]>(mappingString) ?? [];
        var options = new JsonSerializerOptions { WriteIndented = true };
        foreach (var map in maps)
        {
            map.Raw = JsonSerializer.Serialize(map, options);
        }

        return maps;
    }

    private static IEnumerable<WireMockMappingModel> SetMapsOrdered(IList<WireMockMappingModel> mappings, string sortOrder)
    {
        return sortOrder switch
        {
            "date" => mappings.OrderBy(m => m.UpdatedAt),
            "date_desc" => mappings.OrderByDescending(m => m.UpdatedAt),
            "title" => mappings.OrderBy(m => m.Title),
            "title_desc" => mappings.OrderByDescending(m => m.Title),
            "guid_desc" => mappings.OrderByDescending(m => m.Guid),
            _ => mappings.OrderBy(m => m.Guid)
        };
    }

    public async Task<IActionResult> OnPostSaveAndUpdate(string id, string guid, string raw)
    {
        var model = JsonSerializer.Deserialize<WireMockMappingModel>(raw);
        var wireMockServerModel = await Repository.GetModelAsync(int.Parse(id));
        var content = JsonContent.Create(model);
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri($"http://localhost:{wireMockServerModel.Port}/__admin/mappings/{guid}"),
            Content = content
        };
        var response = await _client.SendAsync(request);
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
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"http://localhost:{wireMockServerModel.Port}/__admin/mappings/{guid}")
        };
        var response = await _client.SendAsync(request);
        
        return response.IsSuccessStatusCode 
            ? RedirectToPage(new { id }) 
            : RedirectToPage("../Error");
    }

    public async Task<IActionResult> OnPostResetAllMappings(string id)
    {
        var wireMockServerModel = await Repository.GetModelAsync(int.Parse(id));
        var context = new StringContent(string.Empty);
        await _client.PostAsync($"http://localhost:{wireMockServerModel.Port}/__admin/mappings/reset",
            context);

        return RedirectToPage(new { id });
    }
}