using System.ComponentModel.DataAnnotations;
using WireMock.Settings;

namespace WireMock.Server;

public class WireMockServerModel
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime Created { get; set; }

    [MaxLength(100)]public string Contact { get; set; } = string.Empty;

    [MaxLength(255)]public string Description { get; set; } = string.Empty;
    
    public int Port { get; set; }
    public bool StartAdminInterface { get; set; }
    [MaxLength(100)] public string ProxyUrl { get; set; } = string.Empty;
    public bool SaveMapping { get; set; }
    public bool SaveMappingToFile { get; set; }
    [MaxLength(3)] public string SaveMappingForStatusCodePattern { get; set; } = "4xx";

    /// <summary>
    /// AList of all Mappings
    /// </summary>
    public IList<WireMockServerMapping> Mappings { get; set; } = new List<WireMockServerMapping>();
    
    internal WireMockServerSettings ToSettings()
    {
        return new WireMockServerSettings()
        {
            ProxyAndRecordSettings = new ProxyAndRecordSettings()
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