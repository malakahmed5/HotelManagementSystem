using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Shared.DTOs.BookingDTOs
{
    public class BookingDTO
    {
        public Guid Id { get; set; }
        public string GuestFullName { get; set; } = default!;
        public string GuestEmail { get; set; } = default!;
        public string BookingStatus { get; set; } = default!;
        public decimal TotalAmount { get; set; } 
        public int RoomId { get; set; }
    }
}
