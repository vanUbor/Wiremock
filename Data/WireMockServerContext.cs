using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WireMock.Server
{
    public class WireMockServerContext : DbContext
    {
        public WireMockServerContext (DbContextOptions<WireMockServerContext> options)
            : base(options)
        {
        }

        public DbSet<WireMock.Server.WireMockServerModel> WireMockServerModel { get; set; } = default!;
    }
}
