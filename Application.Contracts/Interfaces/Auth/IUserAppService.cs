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
       
        Task<AuthResponseDto> LoginAsync(LoginDto input);
        Task<Response<Guid>> RegisterAsync(RegisterDto input);
    }
}
