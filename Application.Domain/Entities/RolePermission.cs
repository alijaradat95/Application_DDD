using Application.Domain.Shared.Base;

namespace Application.Domain.Entities
{
    public class RolePermission : FullAuditedEntity
    {
        public Guid RoleId { get; set; }
        public string Permission { get; set; }

        public Role Role { get; set; }
    }
}