using HMS.Shared.DTOs.PaymentDTOs.PayMobDTOs.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Shared.DTOs.PaymentDTOs.PaymobDTOs
{
    public class PaymobSettings
    {
        public string BaseUrl { get; set; } = default!;
        public string ApiKey { get; set; } = default!;
        public string SecretKey { get; set; } = default!;
        public string HMAC { get; set; } = default!;
        public string IFrameId { get; set; } = default!;
        public Dictionary<PaymentMethodType, string> IntegrationIds { get; set; } = new();
    }
}
