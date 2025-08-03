using Application.Contracts.Dtos.User;
using Application.Contracts.Interfaces.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Web.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly IUserAppService _userAppService;

        public List<UserDto> Users { get; set; } = new();

        public IndexModel(IUserAppService userAppService)
        {
            _userAppService = userAppService;
        }

        public async Task OnGetAsync()
        {
            var result = await _userAppService.GetAllAsync();

            Users = result.ToList();
        }
    }
}
