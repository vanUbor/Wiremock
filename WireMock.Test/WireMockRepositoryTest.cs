using Microsoft.Data.Sqlite;
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
        var connection = new SqliteConnection("Data Source=InMemorySample;Mode=Memory;");
        connection.Open();

        var options = new DbContextOptionsBuilder<WireMockServerContext>()
            .UseSqlite(connection)
            .Options;

        return new DbContextFactory(options);
    }

    private void InitializeContextWithTestData()
    {
        var context = _contextFactory!.CreateDbContext();
        context.Database.EnsureCreated();

        context.WireMockServerModel.Add(new WireMockServiceModel() { Id = 42, Name = "InitModel"});
        context.WireMockServerMapping.Add(new WireMockServerMapping()
        {
            WireMockServerModelId = 42,
            Guid = new Guid([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16]),
            Raw = "InitMapping"
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
    public async Task GetModelAsync()
    {
        //Act
        var model = await _repo!.GetModelAsync(42);

        //Assert
        Assert.AreEqual("InitModel", model.Name);
    }

    [TestMethod]
    public async Task GetModelsAsync()
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
        await _repo!.UpdateMappingAsync(new Guid([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16]),
            "UpdatedRaw");

        // Assert
        var updatedContext = await _contextFactory!.CreateDbContextAsync();
        Assert.AreEqual("UpdatedRaw", updatedContext.WireMockServerMapping.First().Raw);
    }

    [TestMethod]
    public async Task AddExistingMappingsTest()
    {
        // Arrange
        var id = 42;
        var guid = new Guid([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16]);
        var raw = "FakeJsonMapping";

        // Act
        await _repo!.AddMappingsAsync(id, new[] { new Tuple<Guid, string>(guid, raw) });

        // Assert
        var updatedContext = await _contextFactory!.CreateDbContextAsync();
        var service = updatedContext.WireMockServerModel.Include(wireMockServiceModel => wireMockServiceModel.Mappings)
            .Single(m => m.Id == id);
        Assert.AreEqual(1, service.Mappings.Count);
    }

    [TestMethod]
    public async Task AddNewMappingsTest()
    {
        // Arrange
        var id = 42;
        var guid = new Guid([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 17]);
        var raw = "FakeJsonMapping";

        // Act
        await _repo!.AddMappingsAsync(id, new[] { new Tuple<Guid, string>(guid, raw) });

        // Assert
        var updatedContext = await _contextFactory!.CreateDbContextAsync();
        var service = updatedContext.WireMockServerModel.Include(wireMockServiceModel
                => wireMockServiceModel.Mappings)
            .Single(m => m.Id == id);
        Assert.AreEqual(2, service.Mappings.Count);
        Assert.AreEqual(guid, service.Mappings.Last().Guid);
        Assert.AreEqual(raw, service.Mappings.Last().Raw);
    }

    [TestMethod]
    public async Task RemoveMappingsTest()
    {
        //Arrange
        var guid = new Guid([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16]);

        //Act
        await _repo!.RemoveMappingsAsync(new List<Guid> { guid });

        //Assert
        var updatedContext = await _contextFactory!.CreateDbContextAsync();
        Assert.IsFalse(updatedContext.WireMockServerMapping.Any(m => m.Guid == guid));
    }

    [TestMethod]
    public async Task UpdateMappingTest()
    {
        // Arrange
        var guid = new Guid([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16]);
        var raw = "UpdatedFakeMapping";

        // Act
        await _repo!.UpdateMappingsAsync(new[] { new Tuple<Guid, string>(guid, raw) });

        // Assert
        var context = await _contextFactory!.CreateDbContextAsync();
        Assert.AreEqual(raw, context.WireMockServerMapping.Single(mapping => mapping.Guid == guid).Raw);
    }

    [TestMethod]
    public async Task UpdateNotExistingMappingTest()
    {
        // Arrange
        var guid = new Guid([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 17]);
        var raw = "UpdatedFakeMapping";

        // Act
        await _repo!.UpdateMappingsAsync(new[] { new Tuple<Guid, string>(guid, raw) });

        // Assert
        var context = await _contextFactory!.CreateDbContextAsync();
        Assert.AreEqual(1, context.WireMockServerMapping.Count());
        Assert.AreEqual("InitMapping", context.WireMockServerMapping.First().Raw);
    }
}