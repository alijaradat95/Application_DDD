using Application.Domain.Shared.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Application.Services.Common
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId =>
            TryGetGuidClaim(ClaimTypes.NameIdentifier);

        public Guid? TenantId =>
            TryGetGuidClaim("tenant_id");

        public string? UserName =>
            GetClaim(ClaimTypes.Name);

        private string? GetClaim(string type)
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(type)?.Value;
        }

        private Guid? TryGetGuidClaim(string claimType)
        {
            var value = GetClaim(claimType);
            return Guid.TryParse(value, out var result) ? result : null;
        }

    }

    internal static class StringExtensions
    {
        public static Guid? Let(this string? input, Func<string, (bool Success, Guid Value)> tryParse)
        {
            if (input == null) return null;
            var (success, value) = tryParse(input);
            return success ? value : null;
        }

        public static (bool, Guid) TryParse(this Func<string, Guid> _, string input)
        {
            var success = Guid.TryParse(input, out var value);
            return (success, value);
        }
    }

}
