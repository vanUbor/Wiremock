using NSubstitute;
using WireMock.Data;
using WireMock.Server;
using WireMock.Server.Interfaces;
using Index = WireMock.Pages.Index;

namespace WireMock.Test.Pages;

[TestClass]
public class IndexTest
{
    [TestMethod]
    public async Task OnGetTest()
    {
        // Arrange
        var repo = Substitute.For<IWireMockRepository>();
        var orchestrator = Substitute.For<IOrchestrator>();
        var page = new Index(repo, orchestrator);
        
        // Act
        var result = await page.OnGet();
        
        // Assert
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task OnPostStartTest()
    {
        // Arrange
        var repo = Substitute.For<IWireMockRepository>();
        var orchestrator = Substitute.For<IOrchestrator>();
        var page = new Index(repo, orchestrator);

        int? serviceIdToStart = 123; // Use suitable ID value

        // Act
        var result = await page.OnPostStartAsync(serviceIdToStart);

        // Assert
        Assert.IsNotNull(result);
        await orchestrator.Received().StartServiceAsync(serviceIdToStart.Value);
    }

    [TestMethod]
    public async Task OnPostStopTest()
    {
        // Arrange
        var repo = Substitute.For<IWireMockRepository>();
        var orchestrator = Substitute.For<IOrchestrator>();
        var page = new Index(repo, orchestrator);
        int? serviceIdToStop = 123; // Use suitable ID value
    
        // Act
        var result = await page.OnPostStopAsync(serviceIdToStop);

        // Assert
        Assert.IsNotNull(result);
        orchestrator.Received().Stop(serviceIdToStop.Value);
    }

    [TestMethod]
    public async Task OnPostDeleteTest()
    {
        // Arrange
        var repo = Substitute.For<IWireMockRepository>();
        var orchestrator = Substitute.For<IOrchestrator>();
        var page = new Index(repo, orchestrator);
        const int serviceIdToDelete = 123; // Use suitable ID value

        // Act
        var result = await page.OnPostDeleteAsync(serviceIdToDelete);

        // Assert
        Assert.IsNotNull(result);
        orchestrator.Received().RemoveService(serviceIdToDelete);
        await repo.Received().RemoveModelAsync(serviceIdToDelete);
    }
}