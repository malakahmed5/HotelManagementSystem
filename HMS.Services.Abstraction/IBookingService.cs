using HMS.Shared.DTOs.BookingDTOs;
using HMS.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Services.Abstraction
{
    public interface IBookingService
    {
        Task<GenericResponse<Guid>> CreateBookingAsync(string userId , CreateBookingDTO createBooking);  //userId From Token
        Task<GenericResponse<IEnumerable<BookingDTO>>> GetAllBookingsForAdminAsync();
        Task<GenericResponse<bool>> CancelBookingAsync(Guid id);
        Task<GenericResponse<IEnumerable<MyBookingDTO>>> GetAllBookingsForGuestAsync(string guestId);
    }
}
