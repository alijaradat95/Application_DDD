using Application.Domain.Shared.Base;
using Application.Domain.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application.Domain.Entities
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUser;

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }


        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ICurrentUserService currentUser)
            : base(options)
        {
            _currentUser = currentUser;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });

            builder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.Roles)
                .HasForeignKey(ur => ur.UserId);

            builder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(ur => ur.RoleId);

            builder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.Permissions)
            .HasForeignKey(rp => rp.RoleId);

            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // Global Query Filters
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                {
                    builder.Entity(entityType.ClrType).HasQueryFilter(
                        CreateIsDeletedFilter(entityType.ClrType));
                }

                if (typeof(IMustHaveTenant).IsAssignableFrom(entityType.ClrType))
                {
                    builder.Entity(entityType.ClrType).HasQueryFilter(
                        CreateTenantFilter(entityType.ClrType));
                }
            }
        }

        private static LambdaExpression CreateIsDeletedFilter(Type entityType)
        {
            var parameter = Expression.Parameter(entityType, "e");
            var property = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
            var condition = Expression.Equal(property, Expression.Constant(false));
            return Expression.Lambda(condition, parameter);
        }

        private LambdaExpression CreateTenantFilter(Type entityType)
        {
            var parameter = Expression.Parameter(entityType, "e");
            var property = Expression.Property(parameter, "TenantId");

            var tenantIdValue = Expression.Constant(_currentUser.TenantId, typeof(Guid?));
            var condition = Expression.Equal(property, tenantIdValue); 

            return Expression.Lambda(condition, parameter);
        }


        public override int SaveChanges()
        {
            ApplyAuditInfo();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInfo();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditInfo()
        {
            var entries = ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added &&
                    entry.Entity is IHasCreationTime hasCreation)
                {
                    hasCreation.CreatedOn = DateTime.UtcNow;

                    if (entry.Entity is FullAuditedEntity e)
                        e.CreatedBy = _currentUser.UserId;
                }

                if (entry.State == EntityState.Modified &&
                    entry.Entity is IHasModificationTime hasMod)
                {
                    hasMod.LastModifiedOn = DateTime.UtcNow;

                    if (entry.Entity is FullAuditedEntity e)
                        e.LastModifiedBy = _currentUser.UserId;
                }

                if (entry.State == EntityState.Deleted &&
                    entry.Entity is ISoftDelete softDelete)
                {
                    softDelete.IsDeleted = true;
                    softDelete.DeletedOn = DateTime.UtcNow;
                    softDelete.DeletedBy = _currentUser.UserId;

                    entry.State = EntityState.Modified;
                }
            }
        }
    }
}
