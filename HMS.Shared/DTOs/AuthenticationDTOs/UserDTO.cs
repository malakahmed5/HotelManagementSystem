using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Shared.DTOs.AuthenticationDTOs   
{
    public record UserDTO(string FullName = default!, string Email = default!, string Token = default!);
}