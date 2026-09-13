using HMS.Core.Entiites.RoomModuleEntities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Core.Entiites.RoomModuleEntities
{
    public class Room : BaseEntity<int>
    {
        public RoomType RoomType { get; set; }
        public string Description { get; set; } = null!;
        public decimal PricePerNight { get; set; }
        public string Amenities { get; set; } = null!;
        public ICollection<RoomImage> RoomImages { get; set; } = [];
        public RoomStatus RoomStatus { get; set; } = RoomStatus.Available;
    }
}
