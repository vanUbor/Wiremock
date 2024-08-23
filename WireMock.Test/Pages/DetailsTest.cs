using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using WireMock.Data;
using WireMock.Pages.WireMockService;
using WireMock.Server.Interfaces;

namespace WireMock.Test.Pages;

[TestClass]
public class DetailsTest
{
    [TestMethod]
    public async Task OnGetTest()
    {
        // Arrange
        const int serviceId = 42;
        var model = new WireMockServiceModel { Name = "Unit Test Model" };
        var repoMock = new Mock<IWireMockRepository>();
        repoMock.Setup(r 
            => r.GetModelAsync(serviceId)).ReturnsAsync(model);

        var page = new DetailsModel(repoMock.Object);
        
        // Act
        var result = await page.OnGetAsync(serviceId);
        
        // Assert
        Assert.AreEqual(model, page.WireMockServiceModel);
        Assert.IsInstanceOfType<PageResult>(result);
    }
}