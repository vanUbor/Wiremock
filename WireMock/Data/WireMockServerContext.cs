using Microsoft.EntityFrameworkCore;
using WireMock.Server;

namespace WireMock.Data
{
    public class WireMockServerContext : DbContext
    {
        public WireMockServerContext (DbContextOptions<WireMockServerContext> options)
            : base(options)
        {
        }

        public DbSet<WireMockServiceModel> WireMockServerModel { get; set; } = default!;
        public DbSet<WireMockServerMapping> WireMockServerMapping { get; set; } = default!;

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
}
