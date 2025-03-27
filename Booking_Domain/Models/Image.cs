using System.ComponentModel.DataAnnotations;

namespace Booking_Domain.Models 
{ 
    public class Image
    {
        [Key]
        public int Id { get; set; }

        public int RoomId { get; set; } // Foreign Key for Room

        public string roomImage { get; set; } = string.Empty; // The image URL/path

        public Room Room { get; set; } // Navigation Property for Room
    }
}
