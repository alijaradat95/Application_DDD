using Application.Contracts.Dtos.Roles;
using Application.Contracts.Interfaces;
using Application.Domain.Entities;
using Application.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Application.Services.Roles
{
    public class RoleAppService : IRoleAppService
    {
        private readonly ApplicationDbContext _context;

        public RoleAppService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateAsync(CreateRoleDto input)
        {
            var role = new Role
            {
                Id = Guid.NewGuid(),
                Name = input.Name,
                IsDefault = input.IsDefault
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return role.Id;
        }

        public async Task<List<RoleDto>> GetAllAsync()
        {
            return await _context.Roles
                .Where(r => !r.IsDeleted)
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    IsDefault = r.IsDefault
                }).ToListAsync();
        }

        public async Task<RoleDto> GetByIdAsync(Guid id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null || role.IsDeleted) throw new Exception("Role not found");

            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                IsDefault = role.IsDefault
            };
        }

        public async Task UpdateAsync(UpdateRoleDto input)
        {
            var role = await _context.Roles.FindAsync(input.Id);
            if (role == null || role.IsDeleted) throw new Exception("Role not found");

            role.Name = input.Name;
            role.IsDefault = input.IsDefault;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null || role.IsDeleted) throw new Exception("Role not found");

            role.IsDeleted = true;
            role.DeletedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
