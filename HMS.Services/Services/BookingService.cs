using HMS.Core.Contracts;
using HMS.Core.Entiites.AuthModule;
using HMS.Core.Entiites.BookingModule;
using HMS.Core.Entiites.BookingModule.Enums;
using HMS.Core.Entiites.RoomModuleEntities;
using HMS.Core.Entiites.RoomModuleEntities.Enums;
using HMS.Services.Abstraction;
using HMS.Shared.DTOs.BookingDTOs;
using HMS.Shared.DTOs.MessagesDTOs;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Services.Services
{
    public class BookingService : IBookingService
    {
        private readonly ILogger<BookingService> _logger;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;

        public BookingService(IUnitOfWork unitOfWork , ILogger<BookingService> logger, IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
        }

        public async Task<GenericResponse<Guid>> CreateBookingAsync(string userId, CreateBookingDTO createBooking) //return string => return id booking
        {
            //1-createBookingDTO is null ?
            //2-User exists?
            //3-Room available?
            //4-User allowed to book?
            //5-No overlapping booking?
            //6-Prevent Race Condition
            //6-Room status allows booking ?
            var genericResponse = new GenericResponse<Guid>();
            try
            {
                if (createBooking is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "No Booking Data Provided";
                    return genericResponse;
                }

                var room = await _unitOfWork.GetRepository<int, Room>().GetByIdAsync(createBooking.RoomId,
                    query => query.Where(r => r.RoomStatus == RoomStatus.Available || r.RoomStatus == RoomStatus.Reserved)
                    ,r => r.Bookings);
                if (room is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;
                    genericResponse.Message = $"This Room Is Not Avaliable At This Date {createBooking.CheckInDate.ToShortDateString()}";
                    return genericResponse;
                }

                var hasConflict = room.Bookings.Any((b =>
                (b.Status == BookingStatus.PendingPayment || b.Status == BookingStatus.Paid) ||
                    //To Prevent overlapping booking:
                    (b.CheckInDate < createBooking.CheckOutDate) &&
                    (b.CheckOutDate > createBooking.CheckInDate)));

                if (hasConflict)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = $"This Room Is Not Avaliable At This Date {createBooking.CheckInDate.ToShortDateString()}";
                    return genericResponse;
                }

                var booking = new Booking()
                {
                    Id = Guid.NewGuid(),
                    CheckInDate = createBooking.CheckInDate,
                    CheckOutDate = createBooking.CheckOutDate,
                    CreatedAt = DateTime.UtcNow,
                    GuestId = userId,
                    RoomId = room.Id,
                    Status = BookingStatus.PendingPayment,
                    TotalAmount = (createBooking.CheckOutDate - createBooking.CheckInDate).Days * room.PricePerNight,
                };

                await _unitOfWork.GetRepository<Guid, Booking>().AddAsync(booking);

                room.RoomStatus = RoomStatus.Reserved;
                _unitOfWork.GetRepository<int, Room>().Update(room);

                var isCreated = await _unitOfWork.SaveChangesAsync() > 0;

                if (isCreated)
                {
                    genericResponse.StatusCode = StatusCodes.Status201Created;
                    genericResponse.Message = "Booking Created Successfully";
                    genericResponse.Data = booking.Id;
                }
                else
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = "Failed To Create Booking On This Room";
                }
                return genericResponse;
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case DbUpdateConcurrencyException:
                        // ده التعديل: لما حد تاني يحجز نفس الأوضة في نفس اللحظة (RowVersion اختلف)
                        _logger.LogWarning(ex, $"Concurrency conflict while booking room {createBooking.RoomId}", createBooking?.RoomId);
                        genericResponse.StatusCode = StatusCodes.Status409Conflict;
                        genericResponse.Message = "This Room Was Just Booked By Someone Else, Please Try Again";
                        break;

                    default:
                        _logger.LogError(ex, "Failed To Create Booking");
                        genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                        genericResponse.Message = "Faild To Create Booking On This Room";
                        break;
                }
                return genericResponse;
            }
        }
        
        public async Task<GenericResponse<IEnumerable<BookingDTO>>> GetAllBookingsForAdminAsync()
        {
            var genericResponse = new GenericResponse<IEnumerable<BookingDTO>>();
            var bookings = await _unitOfWork.GetRepository<Guid,Booking>()
                .GetAllAsync(q => q.OrderByDescending(b => b.CreatedAt) , b => b.GuestUser);
            if(bookings is null || !bookings.Any())
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "No Booking To Show";
                return genericResponse;
            }

            var bookingsToReturn = bookings.Select(b => new BookingDTO()
            {
                Id = b.Id,
                GuestFullName = b.GuestUser.FullName,
                GuestEmail = b.GuestUser.Email!,
                BookingStatus = b.Status.ToString(),
                TotalAmount = b.TotalAmount,
                RoomId = b.RoomId,
            }).ToList();

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Booking Retrieved Successfully";
            genericResponse.Data = bookingsToReturn;
            return genericResponse;
        }

        public async Task<GenericResponse<bool>> CancelBookingAsync(Guid id)
        {
            var genericResponse = new GenericResponse<bool>();  

            var booking = await _unitOfWork.GetRepository<Guid, Booking>()
                .GetByIdAsync(id , null , b => b.GuestUser);

            if(booking is null)
            {
                genericResponse.StatusCode= StatusCodes.Status404NotFound;
                genericResponse.Message = "No Booking By This Id Found To Cancel";
                return genericResponse;
            }
            if(booking.Status == BookingStatus.Paid)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "You Can Not Cancel This Booking It's Already Paid";
                return genericResponse;
            }

            try
            {
                booking.Status = BookingStatus.Cancelled;
                booking.UpdatedAt = DateTime.Now;
                _unitOfWork.GetRepository<Guid, Booking>().Update(booking);
                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (result)
                {
                    genericResponse.StatusCode = StatusCodes.Status200OK;
                    genericResponse.Message = "Booking Cancelled Successfully";
                    await _emailService.SendEmailAsync(new EmailDTO()
                    {
                        EmailTo = booking.GuestUser.Email!,
                        Subject = "Your Booking Has Been Cancelled",
                        Body = "Sorry, We Have To Cancel Your Booking Because Emergency Reasons Please Contact Us For More Details",
                    });
                }
                else
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = "Failed To Cancel This Booking";
                }

                genericResponse.Data = result;
                return genericResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed To Cancel This Booking");

                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Failed To Cancel This Booking";
                return genericResponse;
            }
        }

        public async Task<GenericResponse<IEnumerable<MyBookingDTO>>> GetAllBookingsForGuestAsync(string guestId)
        {
            var genericResponse = new GenericResponse<IEnumerable<MyBookingDTO>>();

            var bookings = await _unitOfWork.GetRepository<Guid, Booking>()
                .GetAllAsync(q => q.Where(b => b.GuestId == guestId).OrderByDescending(b => b.CreatedAt));
            if(bookings is null || !bookings.Any())
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "No Booking To Show";
                return genericResponse;
            }

            var bookingsToReturn = bookings.Select(b => new MyBookingDTO()
            {
                RoomId = b.RoomId,
                CheckInDate = b.CheckInDate.ToShortDateString(),
                CheckOutDate = b.CheckOutDate.ToShortDateString(),
                BookingStatus = b.Status.ToString(),
                TotalAmount = b.TotalAmount,
            });

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Booking Retrieved Successfully";
            genericResponse.Data = bookingsToReturn;
            return genericResponse;
        }
    }
    
}
