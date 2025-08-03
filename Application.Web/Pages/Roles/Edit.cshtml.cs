using Application.Contracts.Dtos.Roles;
using Application.Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Web.Pages.Roles
{
    public class EditModel : PageModel
    {
        private readonly IRoleAppService _roleAppService;

        public EditModel(IRoleAppService roleAppService)
        {
            _roleAppService = roleAppService;
        }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public UpdateRoleDto Input { get; set; } = new();

        public async Task OnGetAsync()
        {
            var role = await _roleAppService.GetByIdAsync(Id);
            Input = new UpdateRoleDto
            {
                Id = role.Id,
                Name = role.Name
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            await _roleAppService.UpdateAsync(Input);
            return RedirectToPage("Index");
        }
    }
}
