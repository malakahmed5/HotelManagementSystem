using HMS.Core.Contracts;
using HMS.Core.Entiites.AuthModule;
using HMS.Core.Entiites.AuthModule.Enums;
using HMS.Services.Abstraction;
using HMS.Shared.DTOs.AuthenticationDTOs;
using HMS.Shared.DTOs.MessagesDTOs;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HMS.Services.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<HotelUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthenticationService(UserManager<HotelUser> userManager,
            RoleManager<IdentityRole> roleManager ,IConfiguration configuration,
            IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<GenericResponse<UserDTO>> RegisterAsync(RegisterDTO registerDTO)
        {
            var genericResponse = new GenericResponse<UserDTO>();

            if (registerDTO is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Registration failed, please check your data and fill all required data.";
                return genericResponse;
            }
            var user = await _userManager.FindByEmailAsync(registerDTO.Email);
            if (user is not null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = $"This Email '{registerDTO.Email}' Is Already Exist";
                return genericResponse;
            }

            var newUser = new HotelUser()
            {
                FullName = registerDTO.FullName,
                UserName = registerDTO.Email.Split('@')[0],
                Email = registerDTO.Email,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                PhoneNumber = registerDTO.PhoneNumber,
            };


            var result = await _userManager.CreateAsync(newUser, registerDTO.Password);
            if (!result.Succeeded)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Registration failed, please check your data";
                return genericResponse;
            }

            var email = new EmailDTO()
            {
                EmailTo = registerDTO.Email,
                Subject = $"Welcome '{registerDTO.FullName}', To Our Hotel System Application.",
                Body = "This Is A Welcome Message From Our Application, Please Go And Login To Our Applicaton And Enjoy Our Services"
            };
            await _emailService.SendEmailAsync(email);

            result = await _userManager.AddToRoleAsync(newUser, "Guest");
            var token = await CreateTokenAsync(newUser);

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "User registered successfully";
            genericResponse.Data = new UserDTO(newUser.FullName, newUser.Email, token);

            return genericResponse;
        }
        public async Task<GenericResponse<UserDTO>> LoginAsync(LoginDTO loginDTO)
        {
            var genericResponse = new GenericResponse<UserDTO>();
            if (loginDTO is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "No Login Data Provided.";
                return genericResponse;
            }

            //check email exist or not 
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if(user is null)
            {
                genericResponse.StatusCode = StatusCodes.Status401Unauthorized;
                genericResponse.Message = "InvalidCredentials";
                return genericResponse;
            }

            if (!user.IsActive)
            {
                genericResponse.StatusCode = StatusCodes.Status403Forbidden;
                genericResponse.Message = "You Account Has Been Deactivated, Please Contact With Adminstration.";
                return genericResponse;
            }

            //check password exist or not 
            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
            if (!isPasswordCorrect)
            {
                genericResponse.StatusCode = StatusCodes.Status401Unauthorized;
                genericResponse.Message = "InvalidCredentials";
                return genericResponse;
            }

            var token = await CreateTokenAsync(user);
            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Login Successfully";
            genericResponse.Data = new UserDTO(user.FullName!,loginDTO.Email, token);
            return genericResponse;
        }

        public async Task<GenericResponse<IEnumerable<GetUserDTO>>> GetAllUsersForAdminAsync()
        {
            var genericResponse = new GenericResponse<IEnumerable<GetUserDTO>>();
            var users = await _userManager.Users.ToListAsync();
            if (users is null || users.Count == 0)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "No Users To Show";
                return genericResponse;
            }

            var listOfUsersToReturn = new List<GetUserDTO>();
            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                    continue;

                var userRole = await _userManager.GetRolesAsync(user);
                var UserToReturn = new GetUserDTO()
                {
                    Id = user.Id,
                    Email = user.Email!,
                    IsActive = user.IsActive,
                    Role = userRole.ToList(),
                };
                listOfUsersToReturn.Add(UserToReturn);
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Users Data Retreived Successfully";
            genericResponse.Data = listOfUsersToReturn;
            return genericResponse;
        }

        public async Task<GenericResponse<bool>> CreateStaffUserAsync(StaffUserDTO staffUserData)
        {
            var genericResponse = new GenericResponse<bool>();

            if (staffUserData is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Create New Staff User failed, please check your data and fill all required data.";
                return genericResponse;
            }

            var staffUser = await _userManager.FindByEmailAsync(staffUserData.Email);
            if (staffUser is not null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "This email is already registered. Please use a different email.";
                return genericResponse;
            }

            var isStaffSpeciality = Enum.TryParse(staffUserData.Specialities, out StaffSpecialities staffSpecialities);
            if (!isStaffSpeciality)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Invalid Staff Speciality";
                return genericResponse;
            }
            var newStaffUser = new Staff()
            {
                FullName = staffUserData.FullName,
                UserName = staffUserData.Email.Split('@')[0],
                Email = staffUserData.Email,
                PhoneNumber = staffUserData.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Specialities = staffSpecialities
            };


            var result = await _userManager.CreateAsync(newStaffUser, staffUserData.Password);
            if (!result.Succeeded)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = string.Join("| ", result.Errors.Select(e => e.Description));
                return genericResponse;
            }

            await _userManager.AddToRoleAsync(newStaffUser, "Staff");

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Staff User Created Successfully";
            genericResponse.Data = true;
            return genericResponse;
        }
        public async Task<GenericResponse<bool>> ToggleStaffOrUserStatusAccountAsync(string id)
        {
            var genericResponse = new GenericResponse<bool>();
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "User Not Found.";
                return genericResponse;
            }

            user.IsActive = !user.IsActive;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = string.Join("| ", result.Errors.Select(e => e.Description));
                return genericResponse;
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = user.IsActive
                ? "User Activated Successfully."
                : "User Deactivated Successfully.";
            genericResponse.Data = user.IsActive;
            return genericResponse;
        }
        public async Task<GenericResponse<bool>> IsEmailExistAsync(string email)
        {
            var genericResponse = new GenericResponse<bool>();

            if (!IsValidEmailFormat(email))
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Invalid Email format.";
                return genericResponse;
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user is not null)
            {
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "Email Exists.";
                genericResponse.Data = true;
                return genericResponse;
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = " Email Does't Exists.";
            genericResponse.Data = false;
            return genericResponse;
        }
        public async Task<GenericResponse<UserProfileDTO>> GetUserProfileAsync(string email)
        {
            var genericResponse = new GenericResponse<UserProfileDTO>();

            var user = await _userManager.FindByEmailAsync(email);
            if(user is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "Profile User Not Found";
                return genericResponse;
            }

            var userProfile = new UserProfileDTO()
            {
                Email = user.Email!,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber!,
                UserName = user.UserName!,
            };
            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "User Profile Retrieve Successfully";
            genericResponse.Data = userProfile;
            return genericResponse;
        }

        #region HelperMethod 
        private async Task<string> CreateTokenAsync(HotelUser user)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.NameId , user.Id),
                new Claim(JwtRegisteredClaimNames.Email , user.Email!),
                new Claim("Activity" , user.IsActive.ToString())
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var secretKey = _configuration["JWTOptions:SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var credentials = new SigningCredentials(key , SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWTOptions:Issuer"],
                audience: _configuration["JWTOptions:Audience"],
                claims: claims,
                signingCredentials: credentials,
                expires: DateTime.UtcNow.AddHours(1)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private static bool IsValidEmailFormat(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            const string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }


        #endregion
    }
}
