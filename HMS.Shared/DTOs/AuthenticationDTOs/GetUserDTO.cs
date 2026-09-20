using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Shared.DTOs.AuthenticationDTOs
{
    public class GetUserDTO
    {
        public string Id { get; set; } = default!;
        public string Email { get; set; } = default!;
        public List<string> Role { get; set; } = default!;
        public bool IsActive { get; set; }
    }
}
