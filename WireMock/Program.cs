using Microsoft.EntityFrameworkCore;
using WireMock.Data;
using WireMock.Server;
using WireMock.Server.Interfaces;
using WireMock.SignalR;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IWireMockRepository, WireMockRepository>();
builder.Services.AddSingleton<IDbContextFactory<WireMockServerContext>, DbContextFactory>();
builder.Services.AddSingleton<IOrchestrator, ServiceOrchestrator>();
builder.Services.AddSingleton<WireMockServiceList>();

builder.Services.AddSingleton<DbContextOptions<WireMockServerContext>>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

    var optionsBuilder = new DbContextOptionsBuilder<WireMockServerContext>();
    optionsBuilder.UseSqlite(configuration.GetConnectionString("WireMockServerContext")
                             ?? throw new InvalidOperationException("Connection string 'WireMockServerContext' not found."));

    return optionsBuilder.Options;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.MapHub<MappingHub>("/mappinghub");

await app.RunAsync();
