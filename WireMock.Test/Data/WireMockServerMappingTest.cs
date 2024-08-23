using System.Text.Json;
using WireMock.Data;

namespace WireMock.Test.Data;

[TestClass]
public class WireMockServerMappingTest
{
    [TestMethod]
    public void Title_NoRawTest()
    {
        // Arrange
        var mapping = new WireMockServerMapping();
        
        // Act
        var title = mapping.Title;
        
        // Assert
        Assert.AreEqual(string.Empty, title);
    }
    
    [TestMethod]
    public void Title_WithTest()
    {
        // Arrange
        const string titleSet = "UnitTestTitle";
        var mapping = new WireMockServerMapping();
        var model = new WireMockMappingModel
        {
            Title = titleSet,
            Request = new Request(),
            Response = new Response()
        };

        mapping.Raw = JsonSerializer.Serialize(model);
        
        // Act
        var title = mapping.Title;
        
        // Assert
        Assert.AreEqual(titleSet, title);
    }
    
    [TestMethod] public void Title_MalformedRawTest() { 
        // Arrange
        var mapping = new WireMockServerMapping(); 
        mapping.Raw = "{ malformed json }"; 
        
        // Act
        var title = mapping.Title; 
        
        // Assert
        Assert.AreEqual("Title not readable", title); }
}