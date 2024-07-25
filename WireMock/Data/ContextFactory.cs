using Microsoft.EntityFrameworkCore;
using WireMock.Data;

namespace WireMock.Server;

public class DbContextFactory : IDbContextFactory<WireMockServerContext>
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