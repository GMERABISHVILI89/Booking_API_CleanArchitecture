
using Booking_Application.DTO_s.RoomType;
using Booking_Domain.Models;

namespace Booking_Application.Interfaces
{
    public interface IRoomTypeService
    {
        Task<ServiceResponse<RoomTypeGetDTO>> AddRoomType(CreateRoomTypeDTO roomType);
        Task<ServiceResponse<RoomTypeGetDTO>> EditRoomType(int id, UpdateRoomTypeDTO roomType);
        Task<ServiceResponse<List<RoomTypeGetAllDTO>>> GetAllRoomTypes();
        Task<ServiceResponse<bool>> DeleteRoomType(int id);

    }
}
