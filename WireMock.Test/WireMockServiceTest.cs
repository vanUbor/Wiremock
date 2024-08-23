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
        try
        {
            service.CreateAndStart();
        }
        catch (Exception e)
        {
            // Assert
            Assert.Fail(e.ToString());
        }
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
        var mappings = new List<WireMockServerMapping>
        {
            new()
            {
                Id = 1,
                Raw = JsonConvert.SerializeObject(new MappingModel
                {
                    Request = new RequestModel(),
                    Response = new ResponseModel()
                })
            },
            new()
            {
                Id = 2,
                Raw = JsonConvert.SerializeObject(new MappingModel
                {
                    Request = new RequestModel(),
                    Response = new ResponseModel()
                })
            }
        };

        // Act
        try
        {
            service.CreateAndStart(mappings);
        }
        catch (Exception e)
        {
            // Assert
            Assert.Fail(e.ToString());
        }
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
        var mappings = new List<WireMockServerMapping>
        {
            new()
            {
                Id = 1
            },
            new()
            {
                Id = 2
            }
        };

        // Act
        try
        {
            service.CreateAndStart(mappings);
        }
        catch (Exception e)
        {
            // Assert
            Assert.Fail(e.ToString());
        }
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
        var mappings = new List<WireMockServerMapping>
        {
            new()
            {
                Id = 1,
                Raw = JsonConvert.SerializeObject(new MappingModel
                {
                    Request = new RequestModel(),
                    Response = new ResponseModel()
                })
            },
            new()
            {
                Id = 2,
                Raw = JsonConvert.SerializeObject(new MappingModel
                {
                    Request = new RequestModel(),
                    Response = new ResponseModel()
                })
            }
        };

        // Act
        try
        {
            service.CreateAndStart(mappings);
            await Task.Delay(3000); // wait to let the CheckMappings-Timer trigger
        }
        catch (Exception e)
        {
            // Assert
            Assert.Fail(e.ToString());
        }
    }

    [TestMethod]
    public void RaiseNewMappingTest()
    {
        // Arrange
        var model = new WireMockServiceModel { Name = "UnitTest Model", Id = 42 };
        var service = new WireMockService(model);
        var mappingsAdded = false;
        var serviceId = 0;
        service.MappingsAdded += (_, args) =>
        {
            mappingsAdded = true;
            serviceId = args.ServiceId;
        };

        // Act
        service.RaiseNewMappings([new MappingModel()]);

        // Assert
        Assert.IsTrue(mappingsAdded);
        Assert.IsNotNull(service.MappingsAdded);
        Assert.AreEqual(42, serviceId);
    }

    [TestMethod]
    public void RaiseMappingRemoved()
    {
        // Arrange
        var model = new WireMockServiceModel { Name = "UnitTest Model", Id = 42 };
        var service = new WireMockService(model);
        var mappingsAdded = false;
        var serviceId = 0;
        service.MappingsRemoved += (_, args) =>
        {
            mappingsAdded = true;
            serviceId = args.ServiceId;
        };

        // Act
        service.RaiseMappingRemoved([new MappingModel()]);

        // Assert
        Assert.IsTrue(mappingsAdded);
        Assert.IsNotNull(service.MappingsRemoved);
        Assert.AreEqual(42, serviceId);
    }
}