using WireMock.Data;

namespace WireMock.Server;

public interface IDbContextFactory
{
    WireMockServerContext CreateDbContext();
}