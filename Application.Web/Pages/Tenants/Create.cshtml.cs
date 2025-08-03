using Application.Contracts.Dtos.Tenant;
using Application.Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Web.Pages.Tenants
{
    public class CreateModel : PageModel
    {
        private readonly ITenantAppService _tenantAppService;

        public CreateModel(ITenantAppService tenantAppService)
        {
            _tenantAppService = tenantAppService;
        }

        [BindProperty]
        public CreateTenantDto Input { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            await _tenantAppService.CreateAsync(Input);
            return RedirectToPage("Index");
        }
    }
}
