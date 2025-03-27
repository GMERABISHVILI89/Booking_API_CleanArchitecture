using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Booking_Application.DTO_s.Booking
{
    public class BookingDTO
    {
        [Key]

        public int Id { get; set; }
        public int RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
    }

    public class BookingWithImageDTO : BookingDTO
    {
        public string RoomImage { get; set; }
    }
}
