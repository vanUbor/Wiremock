using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace WireMock.Data;


[ExcludeFromCodeCoverage]
public class WireMockMappingModel
{
    public string? Guid { get; init; }
    public DateTime UpdatedAt { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public int Priority { get; init; }
    public required Request Request { get; init; }
    public required Response Response { get; init; }

    [JsonIgnore] public string? Raw { get; set; }
}