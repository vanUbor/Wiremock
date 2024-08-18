using Microsoft.EntityFrameworkCore;

namespace WireMock.Data;

public class WireMockServerContext(DbContextOptions<WireMockServerContext> options) 
    : DbContext(options)
{
    public DbSet<WireMockServiceModel> WireMockServerModel { get; init; } = default!;
    public DbSet<WireMockServerMapping> WireMockServerMapping { get; init; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WireMockServerMapping>()
            .HasIndex(e => e.Guid)
            .IsUnique();

        modelBuilder.Entity<WireMockServerMapping>()
            .HasOne<WireMockServiceModel>()
            .WithMany(m => m.Mappings)
            .HasForeignKey(m => m.WireMockServerModelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}