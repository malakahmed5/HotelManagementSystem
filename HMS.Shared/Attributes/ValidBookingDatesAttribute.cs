using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.Shared.DTOs.BookingDTOs;
using System.ComponentModel.DataAnnotations;
namespace HMS.Shared.Attributes
{
    public class ValidBookingDatesAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext)
        {
            var dto = (CreateBookingDTO)validationContext.ObjectInstance;

            if (dto.RoomId <= 0)
                return new ValidationResult(
                    "RoomId must be greater than 0.",
                    new[] { nameof(dto.RoomId) });

            if (dto.CheckInDate < DateTime.Now)
                return new ValidationResult(
                    "Check-in date cannot be in the past.",
                    new[] { nameof(dto.CheckInDate) });

            if (dto.CheckOutDate <= dto.CheckInDate)
                return new ValidationResult(
                    "Check-out date must be after check-in date.",
                    new[] { nameof(dto.CheckOutDate) });

            return ValidationResult.Success;
        }
    }
}
