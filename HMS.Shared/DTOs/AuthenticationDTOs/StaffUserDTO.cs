using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Shared.DTOs.AuthenticationDTOs
{
    public class StaffUserDTO
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 100 characters.")]
        public string FullName { get; set; } = default!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string Password { get; set; } = default!;

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^01[0125][0-9]{8}$",
            ErrorMessage = "Phone number must be a valid Egyptian mobile number.")]
        public string PhoneNumber { get; set; } = default!;

        [Required(ErrorMessage = "Specialty is required.")]
        public string Specialities { get; set; } = default!;
    }
}
