using WireMock.Pages.WireMockService;

namespace WireMock.Test.Pages;

[TestClass]
public class PaginatedListTest
{
    [TestMethod]
    public void CreatePaginatedListTest()
    {
        // Arrange
        var page = 2;
        var pageSize = 2;
        var items = new List<int> { 1, 2, 3, 4, 5 };
        
        // Act
        var pl = new PaginatedList<int>(items, 
            items.Count,
            page, 
            pageSize);
        
        // Assert
        Assert.IsNotNull(pl);
        Assert.AreEqual(page, pl.Page);
        Assert.AreEqual(3, pl.TotalPages);
        Assert.IsTrue(pl.HasPreviousPage);
        Assert.IsTrue(pl.HasNextPage);
        Assert.AreEqual(5, pl.Count);
        Assert.AreEqual(1, pl[0]);
        Assert.AreEqual(2, pl[1]);
        Assert.AreEqual(3, pl[2]);
        Assert.AreEqual(4, pl[3]);
        Assert.AreEqual(5, pl[4]);
    }

    [TestMethod]
    public void CreateTest()
    {
        // Arrange
        var source = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var page = 3;
        var pageSize = 3;

        // Act
        var pl = PaginatedList<int>.CreatePage(source, page, pageSize);

        // Assert
        Assert.IsNotNull(pl);
        Assert.AreEqual(page, pl.Page);
        Assert.AreEqual(4, pl.TotalPages);
        Assert.IsTrue(pl.HasPreviousPage);
        Assert.IsTrue(pl.HasNextPage);
        Assert.AreEqual(3, pl.Count);
        Assert.AreEqual(7, pl[0]);
        Assert.AreEqual(8, pl[1]);
        Assert.AreEqual(9, pl[2]);
    }
}