

using Booking_Application.DTO_s.Hotel;
using Booking_Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Booking_Application.Interfaces
{
    public interface IHotelService
    {

        Task<ServiceResponse<Hotel>> AddHotel(CreateHotelDTO hotelDTO, IFormFile hotelImage);
        Task<ServiceResponse<List<Hotel>>> GetAllHotels();
        Task<ServiceResponse<Hotel>> GetHotelById(int hotelId);
        Task<ServiceResponse<Hotel>> UpdateHotel(int hotelId, UpdateHotelDTO hotelDTO, IFormFile hotelImage);
        Task<ServiceResponse<bool>> DeleteHotel(int hotelId);

    }
}
