using Application.Contracts.Dtos.Tenant;
using Application.Contracts.Interfaces;
using Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Application.Services.Tenant
{
    public class TenantAppService : ITenantAppService
    {
        private readonly ApplicationDbContext _context;

        public TenantAppService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TenantDto>> GetAllAsync()
        {
            return await _context.Tenants
                .Where(t => !t.IsDeleted)
                .Select(t => new TenantDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    ConnectionString = t.ConnectionString
                })
                .ToListAsync();
        }

        public async Task<TenantDto> GetByIdAsync(Guid id)
        {
            var tenant = await _context.Tenants.FindAsync(id);
            if (tenant == null || tenant.IsDeleted) throw new Exception("Tenant not found");

            return new TenantDto
            {
                Id = tenant.Id,
                Name = tenant.Name,
                ConnectionString = tenant.ConnectionString
            };
        }

        public async Task<Guid> CreateAsync(CreateTenantDto input)
        {
            var tenant = new Domain.Entities.Tenant()
            {
                Id = Guid.NewGuid(),
                Name = input.Name,
                ConnectionString = input.ConnectionString
            };

            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();
            return tenant.Id;
        }

        public async Task UpdateAsync(UpdateTenantDto input)
        {
            var tenant = await _context.Tenants.FindAsync(input.Id);
            if (tenant == null || tenant.IsDeleted) throw new Exception("Tenant not found");

            tenant.Name = input.Name;
            tenant.ConnectionString = input.ConnectionString;
            tenant.LastModifiedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var tenant = await _context.Tenants.FindAsync(id);
            if (tenant == null || tenant.IsDeleted) throw new Exception("Tenant not found");

            tenant.IsDeleted = true;
            tenant.DeletedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }

}
