using Application.Domain.Entities;
using Application.Domain.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using Application.EntityFrameworkCore;

namespace Application.Application.Authorization
{
    public class PermissionChecker : IPermissionChecker
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ApplicationDbContext _dbContext;

        public PermissionChecker(ICurrentUserService currentUser, ApplicationDbContext dbContext)
        {
            _currentUser = currentUser;
            _dbContext = dbContext;
        }

        public async Task<bool> HasPermissionAsync(string permission)
        {
            if (!_currentUser.UserId.HasValue)
                return false;

            var roles = await _dbContext.UserRoles
                .Where(ur => ur.UserId == _currentUser.UserId)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            return await _dbContext.RolePermissions
                .AnyAsync(rp => roles.Contains(rp.RoleId) && rp.Permission == permission);
        }
    }
}
