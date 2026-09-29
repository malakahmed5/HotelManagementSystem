using HMS.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Services.Abstraction
{
    public interface IPaymentService
    {
        //Paymob:
        Task<GenericResponse<string>> CreatePaymentUrlAsync(Guid bookingId);

        //Task PaymentSucceededAsync(string paymobOrderId);
        //Task PaymentFailedAsync(string paymobOrderId);
    }
}
