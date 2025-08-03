using Application.Contracts.Dtos.Tenant;
using Application.Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Web.Pages.Tenants
{
    public class EditModel : PageModel
    {
        private readonly ITenantAppService _tenantAppService;

        public EditModel(ITenantAppService tenantAppService)
        {
            _tenantAppService = tenantAppService;
        }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public UpdateTenantDto Input { get; set; } = new();

        public async Task OnGetAsync()
        {
            var tenant = await _tenantAppService.GetByIdAsync(Id);
            Input = new UpdateTenantDto
            {
                Name = tenant.Name,
                ConnectionString = tenant.ConnectionString
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            Input.Id = Id;
            await _tenantAppService.UpdateAsync(Input);
            return RedirectToPage("Index");
        }
    }
}
