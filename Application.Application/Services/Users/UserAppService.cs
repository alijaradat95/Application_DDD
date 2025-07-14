using Application.Application.Services.Auth;
using Application.Contracts.Dtos.Auth;
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
