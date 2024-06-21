using Microsoft.EntityFrameworkCore;
using WireMock.Server;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<ServerOrchestrator>();
builder.Services.AddDbContext<WireMockServerContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("WireMockServerContext") 
                      ?? throw new InvalidOperationException("Connection string 'WireMockServerContext' not found.")));

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
