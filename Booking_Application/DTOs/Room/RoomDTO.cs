namespace Booking_Application.DTO_s.Room
{
    public class RoomDTO
    {
        public int Id { get; set; } // Room ID

        public string Name { get; set; } = string.Empty;

        public int HotelId { get; set; } // Associated Hotel ID

        public string HotelName { get; set; } // Optional: Include Hotel Name

        public decimal PricePerNight { get; set; }

        public bool Available { get; set; }

        public int MaximumGuests { get; set; }

        public int RoomTypeId { get; set; }

        public string RoomTypeName { get; set; } // Optional: Include Room Type Name

        public List<string> ImageUrls { get; set; } = new List<string>(); // Room Images (URLs)

        public List<DateTime> BookedDates { get; set; } = new List<DateTime>(); // List of booked dates
    }
}
