using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using WireMock.Data;
using WireMock.Server;

namespace WireMock.Test;

[TestClass]
public class WireMockRepositoryTest
{
    private IDbContextFactory<WireMockServerContext> _contextFactory;
    private ILogger<IWireMockRepository> _logger;
    private WireMockRepository _repo;
    
    [TestInitialize]
    public void Setup()
    {
        _contextFactory = CreateInMemoryContext();
        InitializeContextWithTestData();
        _logger = Substitute.For<ILogger<IWireMockRepository>>();
        _repo = new WireMockRepository(_logger, _contextFactory);
    }
    
    private IDbContextFactory<WireMockServerContext> CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<WireMockServerContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new DbContextFactory(options);
    }

    private void InitializeContextWithTestData()
    {
        var context = _contextFactory.CreateDbContext();
        context.WireMockServerModel.Add(new WireMockServiceModel() { Name = "InitModel"});
        context.WireMockServerMapping.Add(new WireMockServerMapping() {  });
        context.SaveChanges();
    }
    
    [TestMethod]
    public async Task AddModelAsync_Successful()
    {
        //Arrange
        var model = new WireMockServiceModel() { Name = "AddModelAsyncModel_Test", Id = 42 };
        
        //Act
        await _repo.AddModelAsync(model);
        
        //Assert
        Assert.IsTrue(await _repo.CheckModelExistsAsync(model.Id), "Context does not contain model");
    }
}