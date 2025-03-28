using Booking_Application.DTO_s.Auth;
using Booking_Application.DTO_s.UserProfile;
using Booking_Application.Interfaces;
using Booking_Domain.Entities;
using Booking_Domain.Models;
using Booking_Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Booking_Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        private readonly IEmailService _emailService;

        public AuthService(ApplicationDbContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }
        public async Task<ServiceResponse<UserProfileDTO>> GetProfile(int userId)
        {
            var response = new ServiceResponse<UserProfileDTO>();

            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserProfileDTO
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            response.Data = user;
            response.Message = "User Info fethed";
            return response;
        }

        public async Task<ServiceResponse<int>> Register(UserRegisterDTO registerDTO)
        {
            var response = new ServiceResponse<int>();

            if (await UserExists(registerDTO.UserName))
            {
                response.Success = false;
                response.Message = "User already exists";
                return response;
            }
            CreatePasswordHash(registerDTO.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User()
            {
                UserName = registerDTO.UserName,
                Email = registerDTO.Email,
                Role = registerDTO.Role,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // SMTP Email  - was blocked !

            //try
            //{

            //MailRequest mail = new MailRequest();

            //mail.ToEmail = user.Email;
            //mail.Subject = "Hello from Booking";
            //mail.Body = $@"
            //    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
            //        <div style='background-color: #4CAF50; color: white; padding: 10px; text-align: center; font-size: 20px; border-radius: 10px 10px 0 0;'>
            //            Welcome to BookingHotels!
            //        </div>
            //        <div style='padding: 20px;'>
            //            <p>Dear <strong>{user.UserName}</strong>,</p>
            //            <p>Thank you for registering on <strong>BookingHotels</strong>. Your account has been successfully created, and you can now start booking your favorite hotels with ease.</p>
            //            <p>Here are your account details:</p>
            //            <table border='0' cellpadding='5'>
            //                <tr><td><strong>Name:</strong></td><td>{user.UserName}</td></tr>
            //                <tr><td><strong>Email:</strong></td><td>{user.Email}</td></tr>
            //            </table>
            //            <p>You can now log in and explore exclusive deals and comfortable stays at the best hotels.</p>
            //            <p>If you have any questions or need assistance, feel free to contact our support team.</p>
            //            <p>We’re excited to have you on board!</p>
            //        </div>
            //        <div style='text-align: center; font-size: 14px; margin-top: 20px; color: #555;'>
            //            Best regards,<br>
            //            <strong>BookingHotels Team</strong><br>
            //            <a href='mailto:support@bookinghotels.com'>support@bookinghotels.com</a> | <a href='tel:+1234567890'>+1234567890</a>
            //        </div>
            //    </div>";

            //await _emailService.SendEmailAsync(mail);

            //}
            //catch (Exception ex)
            //{

            //    Console.WriteLine($"Error sending email: {ex.Message}");
            //}
            response.Data = user.Id;
            response.Message = "User Registered Successfully !";
            return response;
        }
        public async Task<ServiceResponse<string>> Login(UserLoginDTO loginDTO)
        {
            var response = new ServiceResponse<string>();

            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName.ToLower() == loginDTO.UserName.ToLower());

            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }
            else if (!VerifyPasswordHash(loginDTO.Password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Incorrect password";
                return response;
            }
            else
            {
                var result = GenerateTokens(user, loginDTO.StaySignedIn);
                response.Data = result.AccessToken;
            }

            if (loginDTO.StaySignedIn)
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            return response;
        }

        #region PrivateMethods

        private async Task<bool> UserExists(string userName)
        {
            if (await _context.Users.AnyAsync(x => x.UserName.ToLower() == userName.ToLower()))
                return true;
            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordsalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordsalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private TokenDTO GenerateTokens(User user, bool staySignedIn)
        {
            string refreshToken = string.Empty;

            if (staySignedIn)
            {
                refreshToken = GenerateRefreshToken(user);
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpirationDate = DateTime.Now.AddDays(2);
            }

            var accessToken = GenerateAccessToken(user);

            return new TokenDTO { AccessToken = accessToken, RefreshToken = refreshToken };
        }

        private string GenerateAccessToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role),

            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JwtSettings:Secret").Value!));

            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials,
                Issuer = "BookingApi",
                Audience = "BookingApiClient"
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            SecurityToken token = handler.CreateToken(tokenDescriptor);

            return handler.WriteToken(token);
        }

        private string GenerateRefreshToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JwtSettings:Secret").Value!));

            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(2),
                SigningCredentials = credentials,
                Issuer = "BookingApi",
                Audience = "BookingApiClient"
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            SecurityToken token = handler.CreateToken(tokenDescriptor);

            return handler.WriteToken(token);
        }

        #endregion
    }
}
