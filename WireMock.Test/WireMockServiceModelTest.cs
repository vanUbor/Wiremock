using WireMock.Data;

namespace WireMock.Test;

[TestClass]
public class WireMockServiceModelTest
{
    [TestMethod]
    public void ToSettingsTest()
    {
        // Arrange
        const string url = "ProxyUrl";
        const string saveMappingsForStatusCodePattern = "xxx";
        const bool saveMappingToFile = true;
        const bool saveMapping = true;
        const int port = 1234;
        
        var model = new WireMockServiceModel
        {
            Name = "TestModel",
            ProxyUrl = url,
            SaveMappingForStatusCodePattern = saveMappingsForStatusCodePattern,
            SaveMappingToFile = saveMappingToFile,
            SaveMapping = saveMapping,
            Port = port
        };
        
        // Act
        var settings = model.ToSettings();
        
        // Assert
        Assert.AreEqual("TestModel", settings.ProxyAndRecordSettings!.PrefixForSavedMappingFile);
        Assert.AreEqual(url, settings.ProxyAndRecordSettings.Url);
        Assert.AreEqual(saveMappingToFile, settings.ProxyAndRecordSettings.SaveMappingToFile);
        Assert.AreEqual(port, settings.Port);
    }
}