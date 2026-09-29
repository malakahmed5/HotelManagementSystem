using HMS.Shared.DTOs.PaymentDTOs.PayMobDTOs.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Services.Abstraction
{
    public interface IPaymentServiceGateway
    {
        //PayMob:
        Task<string> AuthenticateAsync();
        Task<string> CreateOrderAsync(string authToken, decimal amount,string currency);
        Task<string> CreatePaymentKeyAsync(string authToken, string orderId, decimal amount, string currency, PaymentMethodType paymentMethodType, string userEmail , string fullName , string phoneNumber);
    }
}
