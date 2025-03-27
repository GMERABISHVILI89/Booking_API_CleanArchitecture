

using Booking_Application.DTO_s.Hotel;
using Booking_Application.DTO_s.Room;
using Booking_Application.DTO_s.RoomType;
using Booking_Domain.Models;

namespace Booking_Application.Interfaces
{
    public interface IFilterService
    {
        Task<ServiceResponse<List<FilteredRoomDTO>>> GetFilteredRooms(FilterDTO filter);
        Task<ServiceResponse<List<FilteredRoomDTO>>> GetAvailableRooms(DateTime startDate, DateTime endDate);
        Task<ServiceResponse<List<RoomTypeGetAllDTO>>> GetRoomTypes();
        Task<ServiceResponse<List<FilterByCityDTO>>> GetHotelsByCity(string city);

    }
}
