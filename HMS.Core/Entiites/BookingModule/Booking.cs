using HMS.Core.Entiites.AuthModule;
using HMS.Core.Entiites.BookingModule.Enums;
using HMS.Core.Entiites.RoomModuleEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HMS.Core.Entiites.BookingModule
{
    public class Booking:BaseEntity<Guid>
    {
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalAmount { get; set; } // PricePerNight * No.Dayes
        public string Currency { get; set; } = "EGP";
        public BookingStatus Status { get; set; } = BookingStatus.PendingPayment;
        public string? PayMobOrderId { get; set; }
        public string? PayMobPaymentKey {  get; set; }
        public DateTime? PaidDate {  get; set; }

        public Room Room { get; set; }
        public int RoomId { get; set; }

        public Guest GuestUser { get; set; }
        public string GuestId { get; set; } = default!;


    }
}
