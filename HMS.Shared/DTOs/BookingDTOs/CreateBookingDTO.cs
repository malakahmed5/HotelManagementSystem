using HMS.Shared.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Shared.DTOs.BookingDTOs
{
    [ValidBookingDates]
    public class CreateBookingDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Room Id Must Be Greater Than 0.")]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Check-In Date Is Required.")]
        [DataType(DataType.DateTime)]
        public DateTime CheckInDate { get; set; }

        [Required(ErrorMessage = "Check-Out Date Is Required.")]
        [DataType(DataType.DateTime)]
        public DateTime CheckOutDate { get; set; }
    }
}
