using Newtonsoft.Json;
using WireMock.Admin.Mappings;
using WireMock.Data;
using WireMock.Server;

namespace WireMock.Test;

[TestClass]
public class WireMockServiceTest
{
    [TestMethod]
    public void CreateAndStart_WithoutMappingsTest()
    {
        // Arrange
        var model = new WireMockServiceModel
        {
            Name = "UnitTestModel"
        };
        var service = new WireMockService(model);

        // Act
        service.CreateAndStart();
    }

    [TestMethod]
    public void CreateAndStart_WithMappingsTest()
    {
        // Arrange
        var model = new WireMockServiceModel
        {
            Name = "UnitTestModel"
        };
        var service = new WireMockService(model);
        var mappings = new List<WireMockServerMapping>()
        {
            new ()
            {
                Id = 1,
                Raw = JsonConvert.SerializeObject(new MappingModel()
                {
                    Request = new RequestModel(),
                    Response = new ResponseModel()
                }), 
            },
            new ()
            {
                Id = 2,
                Raw = JsonConvert.SerializeObject(new MappingModel()
                {
                    
                    Request = new RequestModel(),
                    Response = new ResponseModel()
                }),
                
            }
        };

        // Act
        service.CreateAndStart(mappings);
    }
    
    [TestMethod]
    public void CreateAndStart_WithNullRawMappingsTest()
    {
        // Arrange
        var model = new WireMockServiceModel
        {
            Name = "UnitTestModel"
        };
        var service = new WireMockService(model);
        var mappings = new List<WireMockServerMapping>()
        {
            new ()
            {
                Id = 1
            },
            new ()
            {
                Id = 2
                
            }
        };

        // Act
        service.CreateAndStart(mappings);
    }
    
    [TestMethod]
    public async Task CheckMappings_WithoutRaisingEventsTest()
    {
        // Arrange
        var model = new WireMockServiceModel
        {
            Name = "UnitTestModel"
        };
        var service = new WireMockService(model);
        var mappings = new List<WireMockServerMapping>()
        {
            new ()
            {
                Id = 1,
                Raw = JsonConvert.SerializeObject(new MappingModel()
                {
                    Request = new RequestModel(),
                    Response = new ResponseModel()
                }), 
            },
            new ()
            {
                Id = 2,
                Raw = JsonConvert.SerializeObject(new MappingModel()
                {
                    
                    Request = new RequestModel(),
                    Response = new ResponseModel()
                }),
                
            }
        };

        // Act
        service.CreateAndStart(mappings);
        await Task.Delay(3000); // wait to let the CheckMappings-Timer trigger
    }
}