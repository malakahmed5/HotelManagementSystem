using HMS.Infrastructure.Repository;
using HMS.Services.Abstraction;
using HMS.Shared.DTOs.BookingDTOs;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers
{
    public class BookingsController : ApiBaseController
    {
        private readonly IBookingService _bookingService;
        private readonly IPaymentService _paymentService;

        public BookingsController(IBookingService bookingService , IPaymentService paymentService)
        {
            _bookingService = bookingService;
            _paymentService = paymentService;
        }

        [Authorize(Roles ="Guest")]
        [HttpPost]
        public async Task<ActionResult<GenericResponse<string>>> CreateBooking(CreateBookingDTO createBooking)
        {
            var userId = GetUserIdFromToken();
            var result = await _bookingService.CreateBookingAsync(userId, createBooking);
            return HandelResponse(result);
        }

        [Authorize(Roles = "Guest")]
        [HttpPost("{id}/pay")]
        public async Task<ActionResult<GenericResponse<string>>> CreatePaymentUrl(Guid id)
        {
            var result = await _paymentService.CreatePaymentUrlAsync(id);
            return HandelResponse(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public async Task<ActionResult<GenericResponse<IEnumerable<BookingDTO>>>> GetAllBookingsForAdmin()
        {
            var result = await _bookingService.GetAllBookingsForAdminAsync();
            return HandelResponse(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/cancel")]
        public async Task<ActionResult<GenericResponse<bool>>> CancelBooking(Guid id)
        {
            var result = await _bookingService.CancelBookingAsync(id);
            return HandelResponse(result);
        }

        [Authorize(Roles = "Guest")]
        [HttpGet("my")]
        public async Task<ActionResult<GenericResponse<IEnumerable<MyBookingDTO>>>> GetAllBookingsForGuest()
        {
            var result = await _bookingService.GetAllBookingsForGuestAsync(GetUserIdFromToken());
            return HandelResponse(result);
        }
    }
}
