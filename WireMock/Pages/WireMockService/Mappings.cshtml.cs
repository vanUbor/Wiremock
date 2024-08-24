using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using WireMock.Data;
using WireMock.Server.Interfaces;
using WireMock.SignalR;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WireMock.Pages.WireMockService;

public class Mappings(
    IHttpClientFactory clientFactory,
    IOrchestrator serviceOrchestrator,
    IWireMockRepository Repository,
    IHubContext<MappingHub> HubContext,
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
        ServiceId = serviceId;
        GuidSort = string.IsNullOrEmpty(sortOrder) ? "guid_desc" : "";
        TitleSort = sortOrder == "title" ? "title_desc" : "title";
        DateSort = sortOrder == "date" ? "date_desc" : "date";

        serviceOrchestrator.MappingsChanged += (sender, args) =>
        {
            HubContext.Clients.All.SendAsync("ReceivedMappingUpdate", args);
        };
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
            "date" => mappings.OrderBy(m => m.LastChange),
            "date_desc" => mappings.OrderByDescending(m => m.LastChange),
            "title" => mappings.OrderBy(m => m.Title),
            "title_desc" => mappings.OrderByDescending(m => m.Title),
            "guid_desc" => mappings.OrderByDescending(m => m.Guid),
            _ => mappings.OrderBy(m => m.Guid)
        };
    }

    /// <summary>
    /// Saves and updates a mapping to the service.
    /// If a service is running, the mapping will only get updated on the service,
    /// the orchestrator will recognize the updated mapping and will updated it than on the db as well.
    /// </summary>
    public async Task<IActionResult> OnPostSaveAndUpdate(int serviceId, string guid, string raw)
    {
        if (serviceOrchestrator.IsRunning(serviceId) && !await SaveAndUpdateToService(serviceId, guid, raw))
            return RedirectToPage("../Error");

        await SaveAndUpdateToDatabase(guid, raw);

        return RedirectToPage(new { serviceId });
    }

    private async Task SaveAndUpdateToDatabase(string guid, string raw)
    {
        if (Guid.TryParse(guid, out var guidObj))
        {
            var updatedMapping = new WireMockServerMapping
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

    /// <summary>
    /// Removes the mapping with the given guid
    /// If the service is running it will remove the mapping it from the service, the orchestrator will recognize the removed mapping
    /// and will remove it than from the db as well.
    /// If a service is not running the mapping will only be removed from the database
    /// </summary>
    public async Task<IActionResult> OnPostResetMapping(int serviceId, string guid)
    {
        if (serviceOrchestrator.IsRunning(serviceId))
        {
            var success = await ResetMappingOnService(serviceId, guid);
            return success
                ? RedirectToPage(new { serviceId })
                : RedirectToPage("../Error");
        }

        // if service is not running, it needs to get deleted manually from the db,
        // otherwise the "mapping removed" event handler of Orchestrator will take care of it
        await Repository.RemoveMappingAsync(Guid.Parse(guid));
        return RedirectToPage(new { serviceId });
    }

    /// <summary>
    /// Removes a mapping from the service
    /// </summary>
    private async Task<bool> ResetMappingOnService(int serviceId, string guid)
    {
        var wireMockServerModel = await Repository.GetModelAsync(serviceId);
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"http://localhost:{wireMockServerModel.Port}/__admin/mappings/{guid}")
        };
        var response = await _client.SendAsync(request);
        return response.IsSuccessStatusCode;
    }


    /// <summary>
    /// Removes all mappings
    /// If a service is running it will remove it from the service, the orchestrator will recognize removed mappings
    /// and will remove than from the db as well.
    /// If a service is not running the mappings will only be delete from the database
    /// </summary>
    /// <param name="serviceId">The id of the service from which all mappings get removed</param>
    public async Task<IActionResult> OnPostResetAllMappings(int serviceId)
    {
        var wireMockServerModel = await Repository.GetModelAsync(serviceId);
        if (serviceOrchestrator.IsRunning(serviceId))
        {
            var context = new StringContent(string.Empty);
            await _client.PostAsync($"http://localhost:{wireMockServerModel.Port}/__admin/mappings/reset",
                context);

            return RedirectToPage(new { serviceId });
        }

        // if service is not running, it needs to get deleted manually from the db,
        // otherwise the "mapping removed" event handler of Orchestrator will take care of it
        await Repository.RemoveMappingsAsync(wireMockServerModel.Mappings.Select(m => m.Guid));
        return RedirectToPage(new { serviceId });
    }
}