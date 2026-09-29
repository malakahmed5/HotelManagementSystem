using HMS.Core.Entiites.AuthModule.Enums;
using HMS.Core.Entiites.BookingModule;
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
    public sealed class Admin:HotelUser
    {

    }
    public sealed class Staff : HotelUser
    {
        public StaffSpecialities Specialities { get; set; }
    }
    public sealed class Guest : HotelUser
    {
        public ICollection<Booking> Bookings { get; set; } = [];
    }
}
