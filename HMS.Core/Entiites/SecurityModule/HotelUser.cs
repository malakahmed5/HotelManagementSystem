using HMS.Core.Entiites.AuthModule.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Core.Entiites.AuthModule
{
    public class HotelUser:IdentityUser
    {
        public string FullName { get; set; } = default!;
        public bool IsActive { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = default!;
        public DateTime? UpdatedAt { get; set; } = default!;
    }
    public class Staff : HotelUser
    {
        public StaffSpecialities Specialities { get; set; }
    }
}
