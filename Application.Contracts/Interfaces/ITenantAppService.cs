using Application.Contracts.Dtos.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Interfaces
{
    public interface ITenantAppService
    {
        Task<Guid> CreateAsync(CreateTenantDto input);
        Task DeleteAsync(Guid id);
        Task<List<TenantDto>> GetAllAsync();
        Task<TenantDto> GetByIdAsync(Guid id);
        Task UpdateAsync(UpdateTenantDto input);
    }
}
