using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Domain.Shared.Base;

namespace Application.Domain.Entities
{
    public class Role : FullAuditedEntity
    {
        public string Name { get; set; }
        public bool IsDefault { get; set; } = false;

        public ICollection<UserRole> Users { get; set; } = new List<UserRole>();
    }

}
