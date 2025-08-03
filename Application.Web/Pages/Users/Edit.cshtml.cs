using Application.Contracts.Dtos.User;
using Application.Contracts.Interfaces.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Web.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly IUserAppService _userAppService;

        public EditModel(IUserAppService userAppService)
        {
            _userAppService = userAppService;
        }

        [BindProperty]
        public UpdateUserDto Input { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userAppService.GetByIdAsync(Id);
            Input = new UpdateUserDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await _userAppService.GetByIdAsync(Id);
            Input.Id = user.Id;
            try
            { 
            await _userAppService.UpdateAsync(Input);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.InnerException?.Message ?? ex.Message);
                return Page();
            }

            return RedirectToPage("Index");
        }
    }
}
