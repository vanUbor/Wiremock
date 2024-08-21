using System.ComponentModel.DataAnnotations;

namespace WireMock.Data;

/// <summary>
/// The Mapping Model as it is used in the local Database
/// </summary>
public class WireMockServerMapping
{
    public int Id { get; init; }

    [MaxLength(255)]
    public string Title { get; set; }
    /// <summary>
    /// Represents a globally unique identifier.
    /// </summary>
    public Guid Guid { get; init; }

    public DateTime UpdatedAt { get; set; }
    
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