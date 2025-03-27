
using Booking_Application.DTO_s.Auth;
using Booking_Application.DTO_s.UserProfile;
using Booking_Domain.Entities;
using Booking_Domain.Models;

namespace Booking_Application.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<int>> Register(UserRegisterDTO registerDTO);
        Task<ServiceResponse<string>> Login(UserLoginDTO loginDTO);
        Task<ServiceResponse<UserProfileDTO>> GetProfile(int userId);
         
    }
}
