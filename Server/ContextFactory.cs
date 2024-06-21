using Microsoft.EntityFrameworkCore;

namespace WireMock.Server;

public interface IDbContextFactory
{
    WireMockServerContext CreateDbContext();
}

public class DbContextFactory : IDbContextFactory
{
    private readonly DbContextOptions<WireMockServerContext> _options;

    public DbContextFactory(DbContextOptions<WireMockServerContext> options)
    {
        _options = options;
    }

    public WireMockServerContext CreateDbContext()
    {
        return new WireMockServerContext(_options);
    }
}