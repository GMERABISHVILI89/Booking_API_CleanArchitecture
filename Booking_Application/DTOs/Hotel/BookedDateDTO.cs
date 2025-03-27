using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace Booking_Application.DTO_s.Hotel
{
    public class BookedDateDTO
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Room")]
        public int RoomId { get; set; } // Link to the room

        public Booking_Domain.Models.Room Room { get; set; }  // Navigation property

        public DateTime StartDate { get; set; } // Booking start date
        public DateTime EndDate { get; set; }   // Booking end date
    }
}
