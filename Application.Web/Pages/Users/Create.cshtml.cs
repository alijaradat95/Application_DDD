using Application.Contracts.Dtos.User;
using Application.Contracts.Interfaces.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Web.Pages.Users
{
    public class CreateModel : PageModel
    {
        private readonly IUserAppService _userAppService;

        public CreateModel(IUserAppService userAppService)
        {
            _userAppService = userAppService;
        }

        [BindProperty]
        public RegisterDto Input { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            await _userAppService.RegisterAsync(Input);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Content("success");

            return RedirectToPage("Index");
        }
    }
}
