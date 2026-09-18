using HMS.Shared.SharedEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace HMS.Shared.DTOs.RoomDTOs
{

    public class CreateRoomDTO
    {
        [Required(ErrorMessage = "Room type is required.")]
        [EnumDataType(typeof(RoomType), ErrorMessage = "Invalid room type. Allowed values are: Single, Double, Triple, Suite.")]
        public string RoomType { get; set; } = null!;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description must not exceed 500 characters.")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Amenities are required.")]
        public string Amenities { get; set; } = null!;

        [Required(ErrorMessage = "Price per night must be greater than zero.")]
        public decimal PricePerNight { get; set; }
    }
}
