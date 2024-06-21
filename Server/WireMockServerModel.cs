using System.ComponentModel.DataAnnotations;
using WireMock.Settings;

namespace WireMock.Server;

public class WireMockServerModel
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime Created { get; set; }
    
    public string Contact { get; set; }
    
    public string[] Urls { get; set; }
    public bool StartAdminInterface { get; set; }
    public string ProxyUrl { get; set; }
    public bool SaveMapping { get; set; }
    public bool SaveMappingToFile { get; set; }
    public string SaveMappingForStatusCodePattern { get; set; }

    internal WireMockServerSettings ToSettings()
    {
        return new WireMockServerSettings()
        {
            Urls = Urls,
            StartAdminInterface = StartAdminInterface,
            ProxyAndRecordSettings = new ProxyAndRecordSettings()
            {
                Url = ProxyUrl,
                SaveMapping = SaveMapping,
                SaveMappingToFile = SaveMappingToFile,
                SaveMappingForStatusCodePattern = SaveMappingForStatusCodePattern
            }
        };
    }
}