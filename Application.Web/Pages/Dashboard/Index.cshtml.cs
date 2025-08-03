using Application.Contracts.Interfaces;
using Application.Contracts.Interfaces.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Web.Pages.Dashboard
{
    public class IndexModel : PageModel
    {
        private readonly IUserAppService _userAppService;
        private readonly IRoleAppService _roleAppService;
        private readonly ITenantAppService _tenantAppService;

        public int UserCount { get; set; }
        public int RoleCount { get; set; }
        public int TenantCount { get; set; }

        public IndexModel(
            IUserAppService userAppService,
            IRoleAppService roleAppService,
            ITenantAppService tenantAppService)
        {
            _userAppService = userAppService;
            _roleAppService = roleAppService;
            _tenantAppService = tenantAppService;
        }

        public async Task OnGetAsync()
        {
            var users = await _userAppService.GetAllAsync();
            var roles = await _roleAppService.GetAllAsync();
            var tenants = await _tenantAppService.GetAllAsync();

            UserCount = users.Count;
            RoleCount = roles.Count;
            TenantCount = tenants.Count;
        }
    }
}
