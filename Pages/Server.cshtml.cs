using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WireMock.Server;

namespace WireMock.Pages;

public class Server : PageModel
{

    [BindProperty] public IList<WireMockServer> Servers { get; set; } = default!;
    private readonly WireMockServerContext _context;

    public Server(WireMockServerContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> OnGet()
    {
        var models = await _context.WireMockServerModel.ToListAsync();
        Servers = models.Select(s => new WireMockServer()
        {
            Id = s.Id.ToString(),
            Name = s.Name,
        }).ToList();
        
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var wiremockservermodel = await _context.WireMockServerModel.FindAsync(id);
        if (wiremockservermodel != null)
        {
            _context.WireMockServerModel.Remove(wiremockservermodel);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage();
    }
}