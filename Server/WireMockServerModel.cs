using System.ComponentModel.DataAnnotations;

namespace WireMock.Server;

public class WireMockServerModel
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime Created { get; set; }
    
    public string Contact { get; set; }
}