using System.ComponentModel.DataAnnotations;

namespace Booking_Application.DTO_s.RoomType
{
    public class CreateRoomTypeDTO
    {
        [Required(ErrorMessage = "TypeName is required.")]
        public string TypeName { get; set; } = string.Empty;
    }
}
