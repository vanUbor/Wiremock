using Microsoft.EntityFrameworkCore;
using WireMock.Server;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<IDbContextFactory, DbContextFactory>();
builder.Services.AddSingleton<ServerOrchestrator>();

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

app.Run();
