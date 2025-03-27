using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Booking_Domain.Models
{
    public class BookedDate
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Room")]
        public int RoomId { get; set; } // Link to the room

        public Room Room { get; set; }  // Navigation property

        public DateTime StartDate { get; set; } // Booking start date
        public DateTime EndDate { get; set; }   // Booking end date
    }
}
