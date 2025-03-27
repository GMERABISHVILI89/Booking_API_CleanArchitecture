namespace Booking_Application.DTO_s.Room
{
    public class FilterDTO
    {
            public int? RoomTypeId { get; set; }  // Optional filter for Room Type
            public decimal? PriceFrom { get; set; }  // Minimum price filter
            public decimal? PriceTo { get; set; }  // Maximum price filter
            public int? MaximumGuests { get; set; }  // Filter rooms that can accommodate at least this number of guests
            public DateTime? CheckIn { get; set; }  // Check-in date for availability filtering
            public DateTime? CheckOut { get; set; }  // Check-out date for availability filtering
    }
}
