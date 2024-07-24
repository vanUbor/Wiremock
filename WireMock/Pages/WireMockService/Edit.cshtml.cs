using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WireMock.Data;
using WireMock.Server;

namespace WireMock.Pages_WireMockServers
{
    public class EditModel : PageModel
    {
        private readonly ServiceOrchestrator _serviceOrchestrator;
        private IWireMockRepository _repository;

        public EditModel(IWireMockRepository repository, ServiceOrchestrator serviceOrchestrator)
        {
            _serviceOrchestrator = serviceOrchestrator;
            _repository = repository;
        }

        [BindProperty] public WireMockServiceModel WireMockServiceModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            WireMockServiceModel = await _repository.GetModelAsync(id);
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

            try
            {
                await _repository.UpdateModelAsync(WireMockServiceModel);
                _serviceOrchestrator.Stop(WireMockServiceModel.Id);
                _serviceOrchestrator.RemoveService(WireMockServiceModel.Id);
                _serviceOrchestrator.CreateService(WireMockServiceModel.Id);
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return RedirectToPage("../Server");
        }
    }
}