
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Booking_Domain.Entities;
using Booking_Application.Interfaces;
using Booking_Domain.Models;
using Booking_Application.DTO_s.Auth;
using Booking_Application.DTO_s.UserProfile;

namespace Booking_API_CleanArchitecture.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<ServiceResponse<int>> Register(UserRegisterDTO userRegisterDTO)
        {
            return await _authService.Register(userRegisterDTO);
        }

        [HttpPost("Login")]
        public async Task<ServiceResponse<string>> Login(UserLoginDTO userLoginDTO)
        {
            return await _authService.Login(userLoginDTO);
        }

        [HttpGet("Profile")]
        [Authorize]
        public async Task<ActionResult<ServiceResponse<UserProfileDTO>>> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ServiceResponse<UserProfileDTO> { Success = false, Message = "User not authenticated" });
            }

            var response = await _authService.GetProfile(int.Parse(userId));
            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

    }
}
