using Application.Contracts.Dtos.Roles;
using Application.Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Web.Pages.Roles
{
    public class CreateModel : PageModel
    {
        private readonly IRoleAppService _roleAppService;

        public CreateModel(IRoleAppService roleAppService)
        {
            _roleAppService = roleAppService;
        }

        [BindProperty]
        public CreateRoleDto Input { get; set; } = new();

        public async Task<IActionResult>
    OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            await _roleAppService.CreateAsync(Input);
            return RedirectToPage("Index");
        }
    }
}
