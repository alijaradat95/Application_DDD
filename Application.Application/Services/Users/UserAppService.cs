using Application.Application.Services.Auth;
using Application.Contracts.Dtos.Auth;
using Application.Contracts.Dtos.Tenant;
using Application.Contracts.Dtos.User;
using Application.Contracts.Interfaces.Auth;
using Application.Domain.Entities;
using Application.Domain.Shared.Wrapper;
using Application.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Application.Services.Users
{
    public class UserAppService : IUserAppService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UserAppService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            return await _context.Users
                .Where(t => !t.IsDeleted)
                .Select(t => new UserDto
                {
                    Id = t.Id,
                    Email = t.Email,
                    UserName = t.UserName,
                    TenantId = t.TenantId
                })
                .ToListAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || user.IsDeleted)
                throw new Exception("User not found");

            user.IsDeleted = true;
            user.DeletedOn = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task SetActivationAsync(Guid userId, bool isActive)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
            if (user == null)
                throw new Exception("User not found");

            user.IsActive = isActive;
            user.LastModifiedOn = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task<List<string>> GetUserRolesAsync(Guid userId)
        {
            return await _context.UserRoles
                .Where(x => x.UserId == userId)
                .Select(x => x.Role.Name)
                .ToListAsync();
        }


        public async Task ChangePasswordAsync(ChangePasswordDto input)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == input.UserId && !u.IsDeleted);
            if (user == null)
                throw new Exception("User not found");

            var oldHash = PasswordHasher.HashPassword(input.OldPassword);
            if (user.PasswordHash != oldHash)
                throw new Exception("Old password is incorrect");

            user.PasswordHash = PasswordHasher.HashPassword(input.NewPassword);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(UpdateUserDto input)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == input.Id && !u.IsDeleted);
                if (user == null)
                    throw new Exception("User not found");

                user.UserName = input.UserName;
                user.Email = input.Email;
                user.LastModifiedOn = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<UserDto> GetByIdAsync(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
            if (user == null)
                throw new Exception("User not found");

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                TenantId = user.TenantId
            };
        }


        public async Task<Response<Guid>> RegisterAsync(RegisterDto input)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = input.UserName,
                Email = input.Email,
                PasswordHash = PasswordHasher.HashPassword(input.Password),
                TenantId = input.TenantId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Response<Guid>.Success(user.Id, "User created successfully");
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto input)
        {
            var hash = PasswordHasher.HashPassword(input.Password);

            var user = _context.Users
                .FirstOrDefault(u =>
                    (u.UserName == input.UserName || u.Email == input.Email) &&
                    u.PasswordHash == hash &&
                    !u.IsDeleted);

            if (user == null)
                throw new Exception("Invalid credentials");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim("tenant", user.TenantId?.ToString() ?? "")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthResponseDto
            {
                AccessToken = tokenHandler.WriteToken(token),
                ExpireAt = tokenDescriptor.Expires.Value
            };
        }
    }
}
