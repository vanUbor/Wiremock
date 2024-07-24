namespace WireMock.Server;

public class WireMockServerMapping
{
    public int Id { get; set; }

    /// <summary>
    /// Represents a globally unique identifier.
    /// </summary>
    public Guid Guid { get; set; }


    /// <summary>
    /// Raw JSON of the Mapping
    /// </summary>
    public string? Raw { get; set; }

    /// <summary>
    /// Represents the identification of a WireMock server mapping.
    /// It serves as a foreign key to the corresponding WireMockService
    /// </summary>
    public int WireMockServerModelId { get; set; }
}