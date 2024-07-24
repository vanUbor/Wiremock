using Microsoft.EntityFrameworkCore;

namespace WireMock.Server
{
    public class WireMockServerContext : DbContext
    {
        public WireMockServerContext (DbContextOptions<WireMockServerContext> options)
            : base(options)
        {
        }

        public DbSet<WireMockServerModel> WireMockServerModel { get; set; } = default!;
        public DbSet<WireMockServerMapping> WireMockServerMapping { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WireMockServerMapping>()
                .HasIndex(e => e.Guid)
                .IsUnique();

            modelBuilder.Entity<WireMockServerMapping>()
                .HasOne<WireMockServerModel>()
                .WithMany(m => m.Mappings)
                .HasForeignKey(m => m.WireMockServerModelId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
