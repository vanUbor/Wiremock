using WireMock.Data;
using WireMock.Server;

namespace WireMock.Test;

[TestClass]
public class WireMockServerListTest
{
    private WireMockServiceList? _list;
    private WireMockService? _service;
    
    
    [TestInitialize]
    public void Setup()
    {
        _service = new WireMockService(new WireMockServiceModel
        {
            Name = "FirstService"
        });
        _list = new WireMockServiceList { _service };
    }
    
    
    [TestMethod]
    public void CountTest()
    {
        // Act
        var count = _list!.Count;
        
        //Assert
        Assert.AreEqual(1, count);
    }

    [TestMethod]
    public void IsReadOnlyTest()
    {
        //Act
        var isReadonly = _list!.IsReadOnly;
        
        //Assert
        Assert.AreEqual(false, isReadonly);
    }

   
    [TestMethod]
    public void AddTest()
    {
        //Arrange
        var service = new WireMockService(new WireMockServiceModel
        {
            Name = "SecondService"
        });
        
        // Act
        _list!.Add(service);
        
        // Assert
        Assert.AreEqual(2, _list.Count);
        Assert.IsTrue(_list!.Any(s => s.Name.Equals("FirstService")));
        Assert.IsTrue(_list!.Any(s => s.Name.Equals("SecondService")));
    }

    [TestMethod]
    public void RemoveTest()
    {
        // Make sure list contains items before removing
        Assert.IsTrue(_list!.Count == 1);
        // Act
        _list!.Remove(_service!);
        
        // Assert
        Assert.AreEqual(0, _list.Count);
    }
    
    [TestMethod]
    public void ClearTest()
    {
        // Make sure list contains items before clearing
        Assert.IsTrue(_list!.Count > 0);
        
        // Act
        _list!.Clear();
        
        // Assert
        Assert.AreEqual(0, _list.Count);
    }
}