namespace Booking_Domain.Models
{
    public class Hotel
    {
        public int Id { get; set; } // Primary Key

        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string hotelImage { get; set; } = string.Empty; 

  
        public List<Room> Rooms { get; set; } = new List<Room>(); // Navigation property
    }
}
