
using Booking_Application.DTO_s.Hotel;

namespace Booking_Application.DTO_s.Room
{
    public class FilteredRoomDTO
    {
        public int Id { get; set; }  // Room ID
        public string Name { get; set; } = string.Empty;
        public int HotelId { get; set; }
        public decimal PricePerNight { get; set; }
        public bool Available { get; set; }
        public int MaximumGuests { get; set; }
        public int RoomTypeId { get; set; }

        public List<BookedDateDTO> BookedDates { get; set; } = new List<BookedDateDTO>();
        public List<ImageDTO> imageUrls { get; set; } = new List<ImageDTO>();
    }
}
