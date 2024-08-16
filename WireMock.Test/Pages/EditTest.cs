using Moq;
using WireMock.Data;
using WireMock.Pages.WireMockService;
using WireMock.Server.Interfaces;

namespace WireMock.Test.Pages;

[TestClass]
public class EditTest
{
    [TestMethod]
    public async Task OnGetTest()
    {
        // Arrange
        var repoMock = new Mock<IWireMockRepository>();
        var orchestratorMock = new Mock<IOrchestrator>();
        var page = new EditModel(repoMock.Object, orchestratorMock.Object);
        const int id = 42;
        
        // Act
        var result = await page.OnGetAsync(id);

        // Assert
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task OnPostTest()
    {
        // Arrange
        var repoMock = new Mock<IWireMockRepository>();
        var orchestrator = new Mock<IOrchestrator>();
        
        var page = new EditModel(repoMock.Object, orchestrator.Object);
        page.WireMockServiceModel = new () {Name = "UnitTest Service Model", Id = 42};
        
        // Act
        var returnPage = await page.OnPostAsync();
        
        // Assert
        Assert.IsNotNull(returnPage);
    }
}