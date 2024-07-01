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
    }
}
