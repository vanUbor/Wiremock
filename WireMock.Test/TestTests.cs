namespace WireMock.Test;

[TestClass]
public class TestTests
{
    [TestMethod]
    public void AlwaysTrue()
    {
        Assert.IsTrue(true);
    }
    
    [TestMethod]
    public void AlwaysFails()
    {
        Assert.IsTrue(false);
    }
}