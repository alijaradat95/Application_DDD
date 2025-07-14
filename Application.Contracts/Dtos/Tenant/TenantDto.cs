using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Dtos.Tenant
{
    public class TenantDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? ConnectionString { get; set; }
    }
}
