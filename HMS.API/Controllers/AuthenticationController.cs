using HMS.Infrastructure.Repository;
using HMS.Services.Abstraction;
using HMS.Shared.DTOs.AuthenticationDTOs;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers
{
    public class AuthenticationController : ApiBaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<GenericResponse<UserDTO>>>Register([FromBody] RegisterDTO registerData)
        {
            var result = await _authenticationService.RegisterAsync(registerData);
            return HandelResponse(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<GenericResponse<UserDTO>>>Login([FromBody] LoginDTO loginData)
        {
            var result = await _authenticationService.LoginAsync(loginData);
            return HandelResponse(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create-staff")]
        public async Task<ActionResult<GenericResponse<bool>>> CreateStaffUser([FromBody] StaffUserDTO staffUserData)
        {
            var result = await _authenticationService.CreateStaffUserAsync(staffUserData);
            return HandelResponse(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public async Task<ActionResult<GenericResponse<GetUserDTO>>> GetAllUsersForAdmin()
        {
            var result = await _authenticationService.GetAllUsersForAdminAsync();
            return HandelResponse(result);
        }

        //PUT /api/auth/users/{id}/toggle-status
        [Authorize(Roles = "Admin")]
        [HttpPut("users/{id}/toggle-status")]
        public async Task<ActionResult<GenericResponse<bool>>> ToggleStaffOrUserStatusAcount(string id)
        {
            var result = await _authenticationService.ToggleStaffOrUserStatusAccountAsync(id);
            return HandelResponse(result);
        }

        [HttpGet("email-exist")]
        public async Task<ActionResult<GenericResponse<bool>>> IsEmailExist(string email)
        {
            var result = await _authenticationService.IsEmailExistAsync(email);
            return HandelResponse(result);
        }

        [HttpGet("current-user-profile")]
        public async Task<ActionResult<GenericResponse<UserProfileDTO>>> GetCurrentUserProfile()
        {
            var result = await _authenticationService.GetUserProfileAsync(GetUserEmailFromToken());
            return HandelResponse(result);
        }
    }

}
