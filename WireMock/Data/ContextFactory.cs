using Microsoft.EntityFrameworkCore;

namespace WireMock.Data;

public class DbContextFactory(DbContextOptions<WireMockServerContext> options)
    : IDbContextFactory<WireMockServerContext>
{
    public WireMockServerContext CreateDbContext()
    {
        return new WireMockServerContext(options);
    }
}