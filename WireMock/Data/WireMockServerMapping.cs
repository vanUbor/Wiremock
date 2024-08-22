using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace WireMock.Data;

/// <summary>
/// The Mapping Model as it is used in the local Database
/// </summary>
public class WireMockServerMapping
{
    public int Id { get; init; }

    private string? _title;

    [MaxLength(255)]
    public string? Title
    {
        get
        {
            if (!string.IsNullOrEmpty(_title))
                return _title;
            if (string.IsNullOrEmpty(Raw))
                return string.Empty;
            
            var map = JsonSerializer.Deserialize<WireMockMappingModel>(Raw);
            return map?.Title ?? string.Empty;
        }
        set => _title = value;
    }

    /// <summary>
    /// Represents a globally unique identifier.
    /// </summary>
    public Guid Guid { get; init; }

    /// <summary>
    /// When the mapping was changed, either by creating or changing
    /// </summary>
    public DateTime LastChange { get; set; }
    
    /// <summary>
    /// Raw JSON of the Mapping
    /// </summary>
    [MaxLength(1024 * 1024)]
    public string? Raw { get; set; }

    /// <summary>
    /// Represents the identification of a WireMock server mapping.
    /// It serves as a foreign key to the corresponding WireMockService
    /// </summary>
    public int WireMockServerModelId { get; init; }
}