using HMS.Shared.DTOs.AuthenticationDTOs;
using HMS.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Services.Abstraction
{
    public interface IAuthenticationService
    {
        Task<GenericResponse<UserDTO>> RegisterAsync(RegisterDTO registerDTO);
        Task<GenericResponse<UserDTO>> LoginAsync(LoginDTO loginDTO);

        Task<GenericResponse<bool>> CreateStaffUserAsync(StaffUserDTO staffUserData);
        Task<GenericResponse<IEnumerable<GetUserDTO>>> GetAllUsersForAdminAsync();
        Task<GenericResponse<bool>> ToggleStaffOrUserStatusAccountAsync(string id);
        Task<GenericResponse<bool>> IsEmailExistAsync(string email);

        Task<GenericResponse<UserProfileDTO>> GetUserProfileAsync(string email);
    }
}
