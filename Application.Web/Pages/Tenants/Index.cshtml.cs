using Application.Contracts.Dtos.Tenant;
using Application.Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Web.Pages.Tenants
{
    public class IndexModel : PageModel
    {
        private readonly ITenantAppService _tenantAppService;

        public List<TenantDto> Tenants { get; set; } = new();

        public IndexModel(ITenantAppService tenantAppService)
        {
            _tenantAppService = tenantAppService;
        }

        public async Task OnGetAsync()
        {
            var result = await _tenantAppService.GetAllAsync();

            Tenants = result.ToList();
        }
    }
}
