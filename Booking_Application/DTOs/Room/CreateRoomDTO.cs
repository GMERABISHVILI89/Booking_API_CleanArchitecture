using System.Text.Json.Serialization;

namespace Booking_Application.DTO_s.Room
{
    public class CreateRoomDTO
    {
        public int HotelId { get; set; }  // Foreign Key for Hotel
        public string Name { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public int MaximumGuests { get; set; }
        public int RoomTypeId { get; set; }
    }
}
