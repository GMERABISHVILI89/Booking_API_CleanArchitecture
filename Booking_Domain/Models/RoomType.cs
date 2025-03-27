using System.ComponentModel.DataAnnotations;

namespace Booking_Domain.Models
{
    public class RoomType
    {
        [Key]
        public int Id { get; set; }
        public string TypeName { get; set; } = string.Empty;  // E.g., "Single", "Double", etc.
    }
}
