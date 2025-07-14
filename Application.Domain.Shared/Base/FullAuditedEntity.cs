using Application.Domain.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain.Shared.Base
{
    public abstract class FullAuditedEntity : FullAuditedEntity<Guid>
    {
    }

    public abstract class FullAuditedEntity<TPrimaryKey> : BaseEntity<TPrimaryKey>,
    IHasCreationTime,
    IHasModificationTime,
    ISoftDelete
    {
        public DateTime CreatedOn { get; set; }
        public Guid? CreatedBy { get; set; }

        public DateTime? LastModifiedOn { get; set; }
        public Guid? LastModifiedBy { get; set; }

        public DateTime? DeletedOn { get; set; }
        public Guid? DeletedBy { get; set; }

        public bool IsDeleted { get; set; }
    }
}
