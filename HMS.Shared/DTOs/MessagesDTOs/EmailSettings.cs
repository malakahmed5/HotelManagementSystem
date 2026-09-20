using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Shared.DTOs.MessagesDTOs
{
    public class EmailSettings
    {
        public string SenderEmail { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string Host {  get; set; } = default!;
        public int Port { get; set; } 
    }
}
