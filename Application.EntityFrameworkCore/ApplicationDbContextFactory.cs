using Application.Domain.Entities;
using Application.Domain.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Application.EntityFrameworkCore
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Application.DbMigrator"))
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(configuration.GetConnectionString("Default"));

            var fakeUserService = new DesignTimeCurrentUserService();

            return new ApplicationDbContext(builder.Options, fakeUserService);
        }
    }

    public class DesignTimeCurrentUserService : ICurrentUserService
    {
        public Guid? UserId => Guid.Empty;
        public Guid? TenantId => Guid.Empty;
        public string? UserName => "DesignTime";
    }
}
