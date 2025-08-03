using Application.Contracts.Dtos.User;
using Application.Contracts.Interfaces.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Web.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IUserAppService _userAppService;

        public LoginModel(IUserAppService userAppService)
        {
            _userAppService = userAppService;
        }

        [BindProperty]
        public LoginDto LoginDto { get; set; } = new();

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            _userAppService.LoginAsync(LoginDto);


            return RedirectToPage("/Index");
        }
    }
}
