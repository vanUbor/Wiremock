﻿using Moq;
using NSubstitute;
using WireMock.Admin.Mappings;
using WireMock.Data;
using WireMock.Server;
using WireMock.Server.Interfaces;

namespace WireMock.Test;

[TestClass]
public class ServerOrchestratorTest
{
    private ServiceOrchestrator? _orchestrator;
    private IWireMockRepository? _repo;
    private WireMockServiceList? _serviceList;
    
    [TestInitialize]
    public void Setup()
    {
        _repo = Substitute.For<IWireMockRepository>();
        _repo.GetModelsAsync().ReturnsForAnyArgs(
            new List<WireMockServiceModel>
            {
                new ()
                {
                    Name = "FirstInitService",
                    Id = 1
                },
                new ()
                {
                    Name = "SecondInitService",
                    Id = 2
                }
            });
        
        _serviceList = new WireMockServiceList();
        _orchestrator = new ServiceOrchestrator(_serviceList, _repo);
    }
    
    /// <summary>
    /// Tests if all services where a model can be found
    /// in the repo getting created and returned
    /// where models that can be found but do already exist,
    /// mus tnot be created again
    /// </summary>
    [TestMethod]
    public async Task GetOrCreateServices_GetExistingAndSkipServices_Test()
    {
        // Arrange
        _serviceList!.Add(new WireMockService(new WireMockServiceModel
        {
            Id = 1,
            Name = "UnitTestService"
        }));
        
        // Act
        var services = await _orchestrator!.GetOrCreateServicesAsync();
        
        // Assert
        Assert.AreEqual(2, _serviceList.Count);
        Assert.IsTrue(services.Any(s => s.Id == 1));
        Assert.IsTrue(services.Any(s => s.Id == 2));
    }
    
    /// <summary>
    /// Tests if all services where a model can be found
    /// in the repo getting created as well as all already existing
    /// services are returned
    /// </summary>
    [TestMethod]
    public async Task GetOrCreateServices_GetExistingAndNewServices_Test()
    {
        // Arrange
        _serviceList!.Add(new WireMockService(new WireMockServiceModel
        {
            Id = 3,
            Name = "UnitTestService"
        }));
        
        // Act
        var services = await _orchestrator!.GetOrCreateServicesAsync();
        
        // Assert
        Assert.AreEqual(3, _serviceList.Count);
        Assert.IsTrue(services.Any(s => s.Id == 1));
        Assert.IsTrue(services.Any(s => s.Id == 2));
        Assert.IsTrue(services.Any(s => s.Id == 3));
    }

    /// <summary>
    /// Tests if all services where a model can be found
    /// in the repo getting created and returned
    /// </summary>
    [TestMethod]
    public async Task GetOrCreateServices_CreateNewService_Test()
    {
        // Act
        var services = await _orchestrator!.GetOrCreateServicesAsync();

        // Assert
        Assert.AreEqual(2, _serviceList!.Count);
        Assert.IsTrue(services.Any(s => s.Id == 1));
        Assert.IsTrue(services.Any(s => s.Id == 2));
    }

    [TestMethod]
    public async Task CreateServiceTest()
    {
        // Act
        await _orchestrator!.CreateServiceAsync(1);

        // Assert
        Assert.AreEqual(1, _serviceList!.Count);
    }

    [TestMethod]
    public async Task RemoveServiceTest()
    {
        // Arrange
        await _orchestrator!.GetOrCreateServicesAsync();
        
        // Act
        _orchestrator!.RemoveService(1);
        
        // Assert
        Assert.AreEqual(1, _serviceList!.Count);
        Assert.IsTrue(_serviceList!.Any(s => s.Id == 2));
    }
    
    [TestMethod]
    public async Task RemoveService_NotExistingServiceTest()
    {
        // Arrange
        await _orchestrator!.GetOrCreateServicesAsync();
        
        // Act
        _orchestrator!.RemoveService(3);
        
        // Assert
        Assert.AreEqual(2, _serviceList!.Count);
        Assert.IsTrue(_serviceList!.Any(s => s.Id == 2));
        Assert.IsTrue(_serviceList!.Any(s => s.Id == 1));
    }

    [TestMethod]
    public async Task StopServiceTest()
    {
        // Arrange
        await _orchestrator!.GetOrCreateServicesAsync();
        await _orchestrator!.StartServiceAsync(1);
        
        // Act
        _orchestrator!.Stop(1);
        
        // Assert
        var service = _serviceList!.Single(s => s.Id == 1);
        Assert.IsFalse(service.IsRunning);
    }
    
    [TestMethod]
    public async Task StopService_NotExistingTest()
    {
        // Arrange
        await _orchestrator!.GetOrCreateServicesAsync();
        await _orchestrator!.StartServiceAsync(1);
        
        // Act
        _orchestrator!.Stop(3);
        
        // Assert
        var service = _serviceList!.Single(s => s.Id == 1);
        Assert.IsTrue(service.IsRunning);
    }

    [TestMethod]
    public async Task StartServiceTest()
    {
        // Arrange
        await _orchestrator!.GetOrCreateServicesAsync();
        
        // Act
        await _orchestrator!.StartServiceAsync(1);
        
        // Assert
        var service = _serviceList!.Single(s => s.Id == 1);
        Assert.IsTrue(service.IsRunning);
    }

    [TestMethod]
    [DataRow(42, true)]
    [DataRow(69, false)]
    public void IsRunning_Successful(int serviceId, bool isRunning)
    {
        var model = new WireMockServiceModel
        {
            Id = 42,
            Name = "UnitTestModel"
        };
        var serviceMock = new Mock<WireMockService>(model);
        serviceMock.SetupGet(s => s.IsRunning).Returns(isRunning);
        _serviceList!.Add(serviceMock.Object);
        var orchestrator = new ServiceOrchestrator(_serviceList!, _repo!);
        
        // Act
        var runns = orchestrator.IsRunning(serviceId);
        
        // Assert
        Assert.AreEqual(isRunning, runns);
    }

    [TestMethod]
    public void MappingChangedOnMappingsAddedRaisedTest()
    {
        // Arrange
        var repo = Mock.Of<IWireMockRepository>();
        var serviceList = new ServiceListMock();
        var orchestrator = new ServiceOrchestrator(serviceList, repo);
        var mappingsChangedRaised = false;
        
        orchestrator.MappingsChanged += (sender, args) =>
        {
            mappingsChangedRaised = true;
        };
        
        // Act
        serviceList.RaiseMappingAdded();

        // Assert
        Assert.IsTrue(mappingsChangedRaised);
    }
    
    [TestMethod]
    public void MappingChangedOnMappingsRemovedRaisedTest()
    {
        // Arrange
        var repo = Mock.Of<IWireMockRepository>();
        var serviceList = new ServiceListMock();
        var orchestrator = new ServiceOrchestrator(serviceList, repo);
        var mappingsChangedRaised = false;
        
        orchestrator.MappingsChanged += (_, _) =>
        {
            mappingsChangedRaised = true;
        };
        
        // Act
        serviceList.RaiseMappingRemoved();

        // Assert
        Assert.IsTrue(mappingsChangedRaised);
    }
    
    private class ServiceListMock : WireMockServiceList
    {
        public void RaiseMappingAdded()
            => OnMappingAdded(null, new ChangedMappingsEventArgs(
                new List<MappingModel>()){ServiceId = 42});

        public void RaiseMappingRemoved()
            => OnMappingRemoved(null, new ChangedMappingsEventArgs(
                new List<MappingModel>()){ServiceId = 42});
    }
}