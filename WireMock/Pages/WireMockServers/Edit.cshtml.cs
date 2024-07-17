using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WireMock.Server;

namespace WireMock.Pages_WireMockServers
{
    public class EditModel : PageModel
    {
        private readonly ServerOrchestrator _serverOrchestrator;
        private WireMockServerContext _context;

        public EditModel(IDbContextFactory contextFactory, ServerOrchestrator serverOrchestrator)
        {
            _serverOrchestrator = serverOrchestrator;
            _context = contextFactory.CreateDbContext();
        }

        [BindProperty] public WireMockServerModel WireMockServerModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

           
            WireMockServerModel = await _context.WireMockServerModel.FirstOrDefaultAsync(m => m.Id == id);
            if (WireMockServerModel == null)
            {
                return NotFound();
            }

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            _context.Attach(WireMockServerModel).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                _serverOrchestrator.Stop(WireMockServerModel.Id);
                _serverOrchestrator.RemoveService(WireMockServerModel.Id);
                _serverOrchestrator.CreateService(WireMockServerModel.Id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.WireMockServerModel.Any(e => e.Id == WireMockServerModel.Id))
                {
                    return NotFound();
                }
                throw;
            }

            return RedirectToPage("../Server");
        }
    }
}