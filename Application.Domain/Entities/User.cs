using Application.Domain.Shared.Base;
using Application.Domain.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain.Entities
{
    public class User : FullAuditedEntity , IMustHaveTenant
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }

        public Guid? TenantId { get; set; }
        public Tenant Tenant { get; set; }

        public ICollection<UserRole> Roles { get; set; } = new List<UserRole>();
    }
}
