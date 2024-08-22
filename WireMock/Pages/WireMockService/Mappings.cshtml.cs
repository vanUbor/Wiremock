using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WireMock.Data;
using WireMock.Server.Interfaces;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WireMock.Pages.WireMockService;

public class Mappings(
    IHttpClientFactory clientFactory,
    IOrchestrator serviceOrchestrator,
    IWireMockRepository Repository,
    IConfiguration Config)
    : PageModel
{
    public int ServiceId { get; set; }
    public string? GuidSort { get; set; }
    public string? TitleSort { get; set; }
    public string? DateSort { get; set; }


    public PaginatedList<WireMockServerMapping>? Maps { get; set; }

    [BindProperty] public string MapJsonContent { get; set; } = string.Empty;

    private readonly HttpClient _client = clientFactory.CreateClient();

    public async Task<IActionResult> OnGet(int serviceId, string sortOrder, int? pageIndex)
    {
        GuidSort = string.IsNullOrEmpty(sortOrder) ? "guid_desc" : "";
        TitleSort = sortOrder == "title" ? "title_desc" : "title";
        DateSort = sortOrder == "date" ? "date_desc" : "date";

        var mappings = await Repository.GetMappingsAsync(serviceId);

        var maps = SetMapsOrdered(mappings, sortOrder).ToList();
        var pageSize = Config.GetValue("PageSize", 4);
        Maps = PaginatedList<WireMockServerMapping>.CreatePage(maps, pageIndex ?? 1, pageSize);

        return Page();
    }


    private static IEnumerable<WireMockServerMapping> SetMapsOrdered(IEnumerable<WireMockServerMapping> mappings,
        string sortOrder)
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

    public async Task<IActionResult> OnPostSaveAndUpdate(int serviceId, string guid, string raw)
    {
        // send mapping to service, redirect to error page if update failes
        if (serviceOrchestrator.IsRunning(serviceId) && !await SaveAndUpdateToService(serviceId, guid, raw)) 
            return RedirectToPage("../Error");
        
        await SaveAndUpdateToDatabase(guid, raw);

        return RedirectToPage(new { serviceId });
    }

    private async Task SaveAndUpdateToDatabase(string guid, string raw)
    {
        if (Guid.TryParse(guid, out var guidObj))
        {
            var updatedMapping = new WireMockServerMapping()
            {
                Guid = guidObj,
                Raw = raw
            };
            await Repository.UpdateMappingAsync(updatedMapping);
        }
    }

    private async Task<bool> SaveAndUpdateToService(int id, string guid, string raw)
    {
        var model = JsonSerializer.Deserialize<WireMockMappingModel>(raw);
        var wireMockServerModel = await Repository.GetModelAsync(id);
        var content = JsonContent.Create(model);
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri($"http://localhost:{wireMockServerModel.Port}/__admin/mappings/{guid}"),
            Content = content
        };
        var response = await _client.SendAsync(request);
        return response.IsSuccessStatusCode;
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