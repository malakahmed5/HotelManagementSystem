using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Shared.DTOs.BookingDTOs
{
    public class MyBookingDTO
    {
        public int RoomId { get; set; }
        public string CheckInDate { get; set; } = default!;
        public string CheckOutDate { get; set; } = default!;
        public string BookingStatus { get; set; } = default!;
        public decimal TotalAmount { get; set; }
    }
}
