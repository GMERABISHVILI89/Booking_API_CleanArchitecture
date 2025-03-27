using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace Booking_Domain.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; } // Primary Key

        public string Name { get; set; } = string.Empty;

        [ForeignKey("Hotel")]
        public int HotelId { get; set; } // Foreign Key for Hotel

        public Hotel Hotel { get; set; } // Navigation Property for Hotel

        public decimal PricePerNight { get; set; }

        public bool Available { get; set; } = true;

        public int MaximumGuests { get; set; }

        [ForeignKey("RoomType")] // Foreign Key for RoomType
        public int RoomTypeId { get; set; }

        public RoomType RoomType { get; set; } // Navigation Property for RoomType

        public List<BookedDate> BookedDates { get; set; } = new List<BookedDate>(); // List of Booked Dates

        public List<Image> Images { get; set; } = new List<Image>(); // List of Room Images
    }

}
