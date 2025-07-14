using Application.Domain.Shared.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain.Entities
{
    public class Tenant : FullAuditedEntity
    {
        public string Name { get; set; }
        public string? ConnectionString { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }


}
