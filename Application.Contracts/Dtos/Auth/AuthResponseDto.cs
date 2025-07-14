using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Dtos.Auth
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; }
        public DateTime ExpireAt { get; set; }
    }

}
