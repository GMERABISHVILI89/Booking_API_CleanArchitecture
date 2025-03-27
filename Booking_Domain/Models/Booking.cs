
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Booking_Domain.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Room")]
        public int RoomId { get; set; } // Foreign Key for Room
        public Room Room { get; set; }  // Navigation property to Room

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsConfirmed { get; set; }

        public string CustomerName { get; set; }
        public string CustomerId { get; set; }
        public string CustomerPhone { get; set; }
    }
}
