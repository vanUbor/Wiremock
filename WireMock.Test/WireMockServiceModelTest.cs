﻿using WireMock.Data;

namespace WireMock.Test;

[TestClass]
public class WireMockServiceModelTest
{
    [TestMethod]
    public void ToSettingsTest()
    {
        // Arrange
        var url = "ProxyUrl";
        var saveMappingsForStatusCodePattern = "xxx";
        var saveMappingToFile = true;
        var saveMapping = true;
        var port = 1234;
        
        var model = new WireMockServiceModel()
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