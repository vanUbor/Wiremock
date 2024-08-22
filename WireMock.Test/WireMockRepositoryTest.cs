using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WireMock.Data;

namespace WireMock.Test;

[TestClass]
public class WireMockRepositoryTest
{
    private DbContextFactory? _contextFactory;
    private WireMockRepository? _repo;

    [TestInitialize]
    public void Setup()
    {
        _contextFactory = CreateInMemoryContext();
        InitializeContextWithTestData();
        _repo = new WireMockRepository(_contextFactory);
    }

    private static DbContextFactory CreateInMemoryContext()
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

        context.WireMockServerModel.Add(new WireMockServiceModel { Id = 42, Name = "InitModel"});
        context.WireMockServerMapping.Add(new WireMockServerMapping
        {
            WireMockServerModelId = 42,
            Guid = new Guid([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16]),
            Raw = "InitMapping",
            Title = "Init Unit Test Title"
        });
        context.SaveChanges();
    }

    [TestMethod]
    public async Task AddModelAsync()
    {
        //Arrange
        var model = new WireMockServiceModel { Name = "AddModelAsyncModel_Test", Id = 43 };

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
        Assert.AreEqual(42, models[0].Id);
    }

    [TestMethod]
    public async Task UpdateModelAsync()
    {
        //Arrange
        var model = new WireMockServiceModel { Name = "InitName", Id = 42 };
        var context = await _contextFactory!.CreateDbContextAsync();
        context.WireMockServerModel.Add(model);

        //Act
        model.Name = "newName";
        await _repo!.UpdateModelAsync(model);

        //Assert
        var updatedContext = await _contextFactory.CreateDbContextAsync();
        var updatedModel = await updatedContext.WireMockServerModel.SingleAsync(m => m.Id == 42);
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
        var updatedMapping = new WireMockServerMapping
        {
            Guid = new Guid([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16]),
            Raw = "UpdatedRaw",
            Title = "Unit Test Title"
        };
        await _repo!.UpdateMappingAsync(updatedMapping);

        // Assert
        var updatedContext = await _contextFactory!.CreateDbContextAsync();
        Assert.AreEqual("UpdatedRaw", (await updatedContext.WireMockServerMapping.FirstAsync()).Raw);
    }
    
    [TestMethod]
    public async Task UpdateMapping_FailedAsync()
    {
        // Arrange
        var updatedMapping = new WireMockServerMapping
        {
            Guid = new Guid([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 17]),
            Raw = "UpdatedRaw",
            Title = "Unit Test Title"
        };
        
        // Act & Assert
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () 
            => await _repo!.UpdateMappingAsync(updatedMapping));
    }

    [TestMethod]
    public async Task AddExistingMappingsTest()
    {
        // Arrange
        const int id = 42;
        var guid = new Guid([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16]);
        const string raw = "FakeJsonMapping";

        // Act
        await _repo!.AddMappingsAsync(new List<WireMockServerMapping>
        {
            new()
            {
                Guid = guid,
                Raw = raw
            }
        });

        // Assert
        var updatedContext = await _contextFactory!.CreateDbContextAsync();
        var service = await updatedContext.WireMockServerModel
            .Include(wireMockServiceModel => wireMockServiceModel.Mappings)
            .SingleAsync(m => m.Id == id);
        Assert.AreEqual(1, service.Mappings.Count);
    }

    [TestMethod]
    public async Task AddNewMappingsTest()
    {
        // Arrange
        const int id = 42;
        var guid = new Guid([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 17]);
        const string raw = "FakeJsonMapping";

        // Act
        await _repo!.AddMappingsAsync(new[]
        {
            new WireMockServerMapping
            {
                Guid = guid,
                Raw = raw,
                Title = "Unit Test Title",
                WireMockServerModelId = id
            }
        });

        // Assert
        var updatedContext = await _contextFactory!.CreateDbContextAsync();
        var service = await updatedContext.WireMockServerModel.Include(wireMockServiceModel
                => wireMockServiceModel.Mappings)
            .SingleAsync(m => m.Id == id);
        Assert.AreEqual(2, service.Mappings.Count);
        Assert.AreEqual(guid, service.Mappings[^1].Guid);
        Assert.AreEqual(raw, service.Mappings[^1].Raw);
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
        Assert.IsFalse(await updatedContext.WireMockServerMapping.AnyAsync(m => m.Guid == guid));
    }

    [TestMethod]
    public async Task UpdateMappingsTest()
    {
        // Arrange
        var guid = new Guid([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16]);
        const string raw = "UpdatedFakeMapping";

        // Act
        await _repo!.UpdateMappingsAsync(new[] { new Tuple<Guid, string>(guid, raw) });

        // Assert
        var context = await _contextFactory!.CreateDbContextAsync();
        Assert.AreEqual(raw, (await context.WireMockServerMapping.SingleAsync(mapping => mapping.Guid == guid)).Raw);
    }

    [TestMethod]
    public async Task UpdateNotExistingMappingsTest()
    {
        // Arrange
        var guid = new Guid([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 17]);
        const string raw = "UpdatedFakeMapping";

        // Act
        await _repo!.UpdateMappingsAsync(new[] { new Tuple<Guid, string>(guid, raw) });

        // Assert
        var context = await _contextFactory!.CreateDbContextAsync();
        Assert.AreEqual(1, await context.WireMockServerMapping.CountAsync());
        Assert.AreEqual("InitMapping", (await context.WireMockServerMapping.FirstAsync()).Raw);
    }

    [TestMethod]
    public async Task GetMappingsTest()
    {
        //Arrange
        const int serviceId = 42;
        //Act
        var mappings = (await _repo!.GetMappingsAsync(serviceId))
            .ToList();
        
        //Arrange
        Assert.AreEqual(1, mappings.Count);
        Assert.AreEqual("InitMapping", mappings[0].Raw);
    }

    [TestMethod]
    public async Task RemoveMappingTest()
    {
        //Arrange
        var guid = new Guid([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16]);
        
        //Act
        await _repo!.RemoveMappingAsync(guid);
        
        //Assert
        var updatedContext = await _contextFactory!.CreateDbContextAsync();
        Assert.IsFalse(await updatedContext.WireMockServerMapping.AnyAsync(m => m.Guid == guid));
    }
}