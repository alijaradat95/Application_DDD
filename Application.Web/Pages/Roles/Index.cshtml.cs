using Application.Contracts.Dtos.Roles;
using Application.Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Web.Pages.Roles
{
    public class IndexModel : PageModel
    {
        private readonly IRoleAppService _roleAppService;

        public List<RoleDto> Roles { get; set; } = new();

        public IndexModel(IRoleAppService roleAppService)
        {
            _roleAppService = roleAppService;
        }

        public async Task OnGetAsync()
        {
            var result = await _roleAppService.GetAllAsync();

            Roles = result.ToList();
        }
    }
}
