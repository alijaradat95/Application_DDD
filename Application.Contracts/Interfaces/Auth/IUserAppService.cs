using Application.Contracts.Dtos.Auth;
using Application.Contracts.Dtos.User;
using Application.Domain.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Interfaces.Auth
{
    public interface IUserAppService
    {
        Task ChangePasswordAsync(ChangePasswordDto input);
        Task DeleteAsync(Guid id);
        Task<List<UserDto>> GetAllAsync();
        Task<UserDto> GetByIdAsync(Guid id);
        Task<List<string>> GetUserRolesAsync(Guid userId);
        Task<AuthResponseDto> LoginAsync(LoginDto input);
        Task<Response<Guid>> RegisterAsync(RegisterDto input);
        Task SetActivationAsync(Guid userId, bool isActive);
        Task UpdateAsync(UpdateUserDto input);
    }
}
