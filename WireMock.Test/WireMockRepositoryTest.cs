using Microsoft.EntityFrameworkCore;
using WireMock.Data;
using WireMock.Server;

namespace WireMock.Test;

[TestClass]
public class WireMockRepositoryTest
{
    private IDbContextFactory<WireMockServerContext>? _contextFactory;
    private WireMockRepository? _repo;
    
    [TestInitialize]
    public void Setup()
    {
        _contextFactory = CreateInMemoryContext();
        InitializeContextWithTestData();
        _repo = new WireMockRepository(_contextFactory);
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
        var context = _contextFactory!.CreateDbContext();
        context.WireMockServerModel.Add(new WireMockServiceModel() { Id = 42, Name = "InitModel"});
        context.WireMockServerMapping.Add(new WireMockServerMapping()
        {
            Guid = new Guid([1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16])
        });
        context.SaveChanges();
    }

    [TestMethod]
    public async Task AddModelAsync()
    {
        //Arrange
        var model = new WireMockServiceModel() { Name = "AddModelAsyncModel_Test", Id = 43 };
        
        //Act
        await _repo!.AddModelAsync(model);
        
        //Assert
        Assert.IsTrue(await _repo.CheckModelExistsAsync(model.Id), "Context does not contain model");
    }

    [TestMethod]
    public async Task GetModelsAsync()
    {
        //Arrange
        var model = new WireMockServiceModel() { Name = "AddModelAsyncModel_Test", Id = 43 };
        
        //Act
        await _repo!.AddModelAsync(model);
        
        //Assert
        Assert.IsTrue(await _repo.CheckModelExistsAsync(model.Id), "Context does not contain model");
    }

    [TestMethod]
    public async Task GetModelAsync()
    {
        //Act
        var models = await _repo!.GetModelsAsync();
        
        //Assert
        Assert.AreEqual(1, models.Count);
        Assert.AreEqual(42, models.First().Id);
    }

    [TestMethod]
    public async Task UpdateModelAsync()
    {
        //Arrange
        var model = new WireMockServiceModel() { Name = "InitName", Id = 42 };
        var context = await _contextFactory!.CreateDbContextAsync();
        context.WireMockServerModel.Add(model);
        
        //Act
        model.Name = "newName";
        await _repo!.UpdateModelAsync(model);
        
        //Assert
        var updatedContext = await _contextFactory.CreateDbContextAsync();
        var updatedModel = updatedContext.WireMockServerModel.Single(m => m.Id == 42);
        Assert.AreEqual("newName", updatedModel.Name, "Name not as expected");
    }

    [TestMethod]
    [DataRow(42)]
    [DataRow(43)]
    public async Task RemoveModelAsync(int id)
    {
        // Act
        await _repo!.RemoveModelAsync(42);
        
        // Assert
        var updatedContext = await _contextFactory!.CreateDbContextAsync();
        Assert.IsFalse(await updatedContext.WireMockServerModel.AnyAsync(m => m.Id == id));
    }

    [TestMethod]
    public async Task UpdateMappingAsync()
    {
        // Act
        await _repo!.UpdateMappingAsync(new Guid([1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16]), "UpdatedRaw");
        
        // Assert
        var updatedContext = await _contextFactory!.CreateDbContextAsync();
        Assert.AreEqual("UpdatedRaw", updatedContext.WireMockServerMapping.First().Raw);
    }
}