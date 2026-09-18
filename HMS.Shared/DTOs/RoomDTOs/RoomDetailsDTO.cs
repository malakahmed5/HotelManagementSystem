using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Shared.DTOs.RoomDTOs
{
    public class RoomDetailsDTO
    {
        public int Id { get; set; }
        public string RoomType { get; set; } = default!;
        public string RoomStatus { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Amenities { get; set; } = default!;
        public decimal PricePerNight { get; set; }
        public List<string> RoomImagesUrls { get; set; } = [];
    }
}
