using Application.Domain.Shared.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain.Entities
{
    public class AuditLog : FullAuditedEntity
    {
        public DateTime ActionDate { get; set; }
        public string Action { get; set; }
        public string EntityName { get; set; }
        public Guid? EntityId { get; set; }
        public string? Changes { get; set; }
        public Guid? UserId { get; set; }
    }
}
