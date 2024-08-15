using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using NSubstitute;
using WireMock.Data;
using WireMock.Pages.WireMockService;
using WireMock.Server;

namespace WireMock.Test.Pages;

[TestClass]
public class MappingsTest
{
    private IHttpClientFactory? _clientFactory;
    private IWireMockRepository? _repository;
    private Mock<HttpMessageHandler>? _handlerMock;
    private ServiceOrchestrator? _orchestrator;

    [TestInitialize]
    public void Setup()
    {
        _handlerMock = new Mock<HttpMessageHandler>();
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("[{" +
                                        "\"Title\" : \"UnitTestTitle\"," +
                                        "\"Response\" : null," +
                                        "\"Request\" : null}]"),
        };

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(response)
            .Verifiable();

        var httpClient = new HttpClient(_handlerMock.Object);
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);
        _clientFactory = httpClientFactoryMock.Object;

        _repository = Substitute.For<IWireMockRepository>();
        _repository.GetModelAsync(Arg.Any<int>()).ReturnsForAnyArgs(
            new WireMockServiceModel()
            {
                Name = "UnitTestServiceModel",
                Port = 8081
            });
        var list = new Mock<WireMockServiceList>();
        var orchestratorMock = new Mock<ServiceOrchestrator>(list.Object, _repository);
        orchestratorMock.Setup(o => o.IsRunning(42))
            .Returns(true);
        _orchestrator = orchestratorMock.Object;
    }


    [TestMethod]
    [DataRow("date")]
    [DataRow("date_desc")]
    [DataRow("title")]
    [DataRow("title_desc")]
    [DataRow("guid")]
    [DataRow("guid_desc")]
    public async Task OnGetTest(string sortOrder)
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        var configSectionMock = new Mock<IConfigurationSection>();
        configSectionMock.SetupGet(m => m.Value).Returns("42");
        configMock.Setup(m => m.GetSection("PageSize"))
            .Returns(configSectionMock.Object);

        var mappings = new Mappings(_clientFactory, _orchestrator, _repository, configMock.Object);

        var serviceId = 42;
        var pageIndex = 1;

        // Act
        var actionResult = await mappings.OnGet(serviceId, sortOrder, pageIndex);

        // Assert
        Assert.IsNotNull(actionResult);
    }

    [TestMethod]
    public async Task OnGet_WithNoServiceRunningTest()
    {
        // Arrange
        const string sortOrder = "date";
        var configMock = new Mock<IConfiguration>();
        var configSectionMock = new Mock<IConfigurationSection>();
        configSectionMock.SetupGet(m => m.Value).Returns("42");
        configMock.Setup(m => m.GetSection("PageSize"))
            .Returns(configSectionMock.Object);

        var mappings = new Mappings(_clientFactory, _orchestrator, _repository, configMock.Object);

        const int serviceId = 69;
        const int pageIndex = 1;

        // Act
        var actionResult = await mappings.OnGet(serviceId, sortOrder, pageIndex);

        // Assert
        Assert.IsNotNull(actionResult);
        Assert.IsInstanceOfType(actionResult, typeof(RedirectToPageResult));
        var redirectResult = actionResult as RedirectToPageResult;
        Assert.AreEqual("../Error", redirectResult?.PageName);
    }
    
    [TestMethod]
    public async Task OnPostSaveAndUpdateTest()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        var configSectionMock = new Mock<IConfigurationSection>();
        configSectionMock.SetupGet(m => m.Value).Returns("1");
        configMock.Setup(m => m.GetSection("PageSize")).Returns(configSectionMock.Object);

        var repositoryMock = new Mock<IWireMockRepository>();
        repositoryMock.Setup(x => x.GetModelAsync(It.IsAny<int>()))
            .ReturnsAsync(new WireMockServiceModel
            {
                Name = "ExistingTestServiceModel",
                Port = 8081
            });

        repositoryMock.Setup(x => x.UpdateMappingAsync(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var mappings = new Mappings(_clientFactory, _orchestrator, repositoryMock.Object, configMock.Object);

        string serviceId = "42";
        string guid = Guid.NewGuid().ToString();
        string raw = "{" +
                     "\"Title\" : \"UnitTestTitle\"," +
                     "\"Response\" : null," +
                     "\"Request\" : null}";

        // Act
        var actionResult = await mappings.OnPostSaveAndUpdate(serviceId, guid, raw);

        // Assert
        Assert.IsNotNull(actionResult);
        Assert.IsInstanceOfType(actionResult, typeof(RedirectToPageResult));
    }

    [TestMethod]
    public async Task OnPostResetMappingTest()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        var configSectionMock = new Mock<IConfigurationSection>();
        configSectionMock.SetupGet(m => m.Value).Returns("1");
        configMock.Setup(m => m.GetSection("PageSize")).Returns(configSectionMock.Object);

        var repositoryMock = new Mock<IWireMockRepository>();
        repositoryMock.Setup(x => x.GetModelAsync(It.IsAny<int>()))
            .ReturnsAsync(new WireMockServiceModel
            {
                Name = "ExistingTestServiceModel",
                Port = 8081
            });

        var mappings = new Mappings(_clientFactory, _orchestrator, repositoryMock.Object, configMock.Object);

        var serviceId = "42";
        var guid = Guid.NewGuid().ToString();

        // Act
        var actionResult = await mappings.OnPostResetMapping(serviceId, guid);

        // Assert
        Assert.IsNotNull(actionResult);
        Assert.IsInstanceOfType(actionResult, typeof(RedirectToPageResult));
    }

    [TestMethod]
    public async Task OnPostResetAllMappingsTest()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        var configSectionMock = new Mock<IConfigurationSection>();
        configSectionMock.SetupGet(m => m.Value).Returns("1");
        configMock.Setup(m => m.GetSection("PageSize")).Returns(configSectionMock.Object);

        var repositoryMock = new Mock<IWireMockRepository>();
        repositoryMock.Setup(x => x.GetModelAsync(It.IsAny<int>()))
            .ReturnsAsync(new WireMockServiceModel
            {
                Name = "ExistingTestServiceModel",
                Port = 8081
            });

        var mappings = new Mappings(_clientFactory, _orchestrator, repositoryMock.Object, configMock.Object);
        var serviceId = "42";

        // Act
        var actionResult = await mappings.OnPostResetAllMappings(serviceId);

        // Assert
        Assert.IsNotNull(actionResult);
        Assert.IsInstanceOfType(actionResult, typeof(RedirectToPageResult));
    }
}