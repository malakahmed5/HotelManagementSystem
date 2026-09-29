using HMS.Services.Abstraction;
using HMS.Shared.DTOs.PaymentDTOs.PaymobDTOs;
using HMS.Shared.DTOs.PaymentDTOs.PayMobDTOs.Enums;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HMS.Infrastructure.ExternalServices
{
    public class PaymobPaymentGateway : IPaymentServiceGateway
    {
        private readonly PaymobSettings _paymobSettings;
        private readonly HttpClient _httpClient;

        public PaymobPaymentGateway(IOptions<PaymobSettings> paymobSettings , HttpClient httpClient)
        {
            _paymobSettings = paymobSettings.Value;
            _httpClient = httpClient;
        }
        //Call External EndPoint:
        //1-HTTP Verb ?
        //2-Url ?
        //3-Parameter Needed? [الحاجات الي بتحتاجها غالبا بحطها في ال app settings]
        //4-Return ?
        public async Task<string> AuthenticateAsync()
        {
            //Call External EndPoint:
            //1-HTTP => post 
            //2-Url => BaseUrl/auth/token
            //3-Parameter Needed => api_key [بشكل الي انا كتباه متالفش]
            //4-Return => json as string => throught it will take the => auth_token 

            var response = await _httpClient.PostAsJsonAsync(
                requestUri: $"{_paymobSettings.BaseUrl}/auth/tokens",
                new { api_key = _paymobSettings.ApiKey },
                cancellationToken: default);

            var json = await response.Content.ReadFromJsonAsync<JsonElement>(default);

            var token = json.GetProperty(propertyName:"token").GetString()!;

            return token;
        }  

        public async Task<string> CreateOrderAsync(string authToken, decimal amount, string currency)
        {
            //Call External EndPoint:
            //1-HTTP => post 
            //2-Url => BaseUrl/ecommerce/orders
            //3-Parameter Needed => authToken , amount , currency
            //4-Return => response json as string => throught it will take the => orderId 
            var response = await _httpClient.PostAsJsonAsync(
                $"{_paymobSettings.BaseUrl}/ecommerce/orders",
                new
                {
                    auth_token = authToken,
                    delivery_needed = false,
                    amount_cents = (int)(amount * 100),
                    currency = currency,
                    items = Array.Empty<object>()
                },
                cancellationToken: default);

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();

            var merchant_order_Id = json.GetProperty(propertyName: "id").GetInt32().ToString()!;
            return merchant_order_Id;
        }

        public async Task<string> CreatePaymentKeyAsync(string authToken, string orderId, decimal amount , string currency , PaymentMethodType paymentMethodType,string userEmail , string fullName , string phoneNumber)
        {
            //Call External EndPoint:
            //1-HTTP => http post
            //2-Url => baseUrl/payment_keys
            //3-Parameter Needed => authToken , orderId , integrationId , amount
            //4-Return => paymentKey 

            string selectedIntegrationId = _paymobSettings.IntegrationIds[paymentMethodType]!;

            var response = await _httpClient.PostAsJsonAsync(
                requestUri: $"{_paymobSettings.BaseUrl}/acceptance/payment_keys",
                new
                {
                    auth_token = authToken,
                    order_id = orderId,
                    amount_cents = (int)(amount * 100),
                    currency=currency,
                    expiration = 3600,
                    integration_id = int.Parse(selectedIntegrationId),
                    billing_data = new
                    {
                        first_name = fullName.Split(' ')[0],
                        last_name = fullName.Split(' ')[1].Length > 1 ? fullName.Split(' ')[1] : "NA",
                        email = userEmail,
                        phone_number = phoneNumber,
                        country = "EG",
                        apartment = "NA",
                        floor = "NA",
                        street = "NA",
                        building = "NA",
                        shipping_method = "NA",
                        postal_code = "NA",
                        city = "NA"
                    }
                },
                cancellationToken: default);

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();

            var paymentKey = json.GetProperty(propertyName: "token").GetString()!;
            return paymentKey;
        }
    }
}
