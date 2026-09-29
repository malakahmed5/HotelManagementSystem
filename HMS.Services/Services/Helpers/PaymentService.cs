using HMS.Core.Contracts;
using HMS.Core.Entiites.BookingModule;
using HMS.Services.Abstraction;
using HMS.Shared.DTOs.PaymentDTOs.PaymobDTOs;
using HMS.Shared.DTOs.PaymentDTOs.PayMobDTOs.Enums;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Services.Services.Helpers
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentServiceGateway _paymentServiceGateway;
        private readonly PaymobSettings _paymobSettings;

        public PaymentService(IUnitOfWork unitOfWork , IPaymentServiceGateway paymentServiceGateway,
            IOptions<PaymobSettings> paymobSettings)
        {
            _unitOfWork = unitOfWork;
            _paymentServiceGateway = paymentServiceGateway;
            _paymobSettings = paymobSettings.Value;
        }
        public async Task<GenericResponse<string>> CreatePaymentUrlAsync(Guid bookingId)
        {
            var genericResponse = new GenericResponse<string>();

            //1-Get Booking Check if Exist Book By This Id Or not ?
            var booking = await _unitOfWork.GetRepository<Guid,Booking>()
                .GetByIdAsync(bookingId,null,b => b.GuestUser);
            if(booking is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "Booking Not Found To Pay ";
                return genericResponse;
            }

            //ExternalServices:
            //2-Get Auth Token 
            var authToken = await _paymentServiceGateway.AuthenticateAsync();

            //3-Create Order 
            var orderId = await _paymentServiceGateway.CreateOrderAsync(authToken, booking.TotalAmount, booking.Currency);
            if (string.IsNullOrEmpty(orderId))
            {
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Failed To Create Intent On PayMob";
                return genericResponse;
            }
            booking.PayMobOrderId = orderId;

            //4-CreatePaymentKey
            var paymentKey = await _paymentServiceGateway.CreatePaymentKeyAsync(authToken,orderId, booking.TotalAmount, booking.Currency, PaymentMethodType.OnlineCard, booking.GuestUser.Email!, booking.GuestUser.FullName, booking.GuestUser.PhoneNumber!);
            if (string.IsNullOrEmpty(paymentKey))
            {
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Failed To Create Intent On PayMob";
                return genericResponse;
            }
            booking.PayMobPaymentKey = paymentKey;
            booking.UpdatedAt = DateTime.Now;
            _unitOfWork.GetRepository<Guid,Booking>().Update(booking);
            var result = await _unitOfWork.SaveChangesAsync() > 0;

            if (result)
            {
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "Payment Intent On Paymob Created Successfully";
                genericResponse.Data = $"{_paymobSettings.BaseUrl}/acceptance/iframes/{_paymobSettings.IFrameId}?payment_token={paymentKey}";
            }
            else 
            {
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Failed To Create Intent On PayMob";
            }
            return genericResponse;
        }
    }
}
