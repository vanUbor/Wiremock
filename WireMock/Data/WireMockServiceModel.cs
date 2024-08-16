using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WireMock.Settings;

namespace WireMock.Data;

public class WireMockServiceModel
{
    public int Id { get; init; }
    
    [MaxLength(255)]
    public required string Name { get; set; }
    
    [ExcludeFromCodeCoverage]
    [DataType(DataType.Date)]
    public DateTime Created { get; init; }

    [ExcludeFromCodeCoverage]
    [MaxLength(100)]public string Contact { get; init; } = string.Empty;

    [ExcludeFromCodeCoverage]
    [MaxLength(255)]public string Description { get; init; } = string.Empty;
    
    public int Port { get; init; }
    public bool StartAdminInterface => true;
    [MaxLength(100)] public string ProxyUrl { get; init; } = string.Empty;
    public bool SaveMapping { get; init; }
    public bool SaveMappingToFile { get; init; }
    [MaxLength(3)] public string SaveMappingForStatusCodePattern { get; init; } = "4xx";

    /// <summary>
    /// AList of all Mappings
    /// </summary>
    public IList<WireMockServerMapping> Mappings { get; init; } = new List<WireMockServerMapping>();
    
    internal WireMockServerSettings ToSettings()
    {
        return new WireMockServerSettings
        {
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                Url = ProxyUrl,
                SaveMappingForStatusCodePattern = this.SaveMappingForStatusCodePattern,
                SaveMappingToFile = this.SaveMappingToFile,
                SaveMapping = this.SaveMapping,
                PrefixForSavedMappingFile = this.Name
            },
            StartAdminInterface = this.StartAdminInterface,
            Port = Port,
        };
    }
}