using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Dtos.User
{
    public class LoginDto
    {
        [Required]
        public string Email { get; set; }
        public string? UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public Guid? TenantId { get; set; }
    }

}
