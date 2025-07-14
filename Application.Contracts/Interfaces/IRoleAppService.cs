using Application.Contracts.Dtos.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Interfaces
{
    public interface IRoleAppService
    {
        Task<Guid> CreateAsync(CreateRoleDto input);
        Task<List<RoleDto>> GetAllAsync();
        Task<RoleDto> GetByIdAsync(Guid id);
        Task UpdateAsync(UpdateRoleDto input);
        Task DeleteAsync(Guid id);
    }
}
