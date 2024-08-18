using Microsoft.EntityFrameworkCore;

namespace WireMock.Data;

public class DbContextFactory(DbContextOptions<WireMockServerContext> options)
    : IDbContextFactory<WireMockServerContext>
{
    public WireMockServerContext CreateDbContext()
        => new WireMockServerContext(options);


    public async Task<WireMockServerContext> CreateDbContextAsync()
        => await Task.FromResult(CreateDbContext());
}