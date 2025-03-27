using Microsoft.AspNetCore.Http;

namespace Booking_Application.DTO_s.Hotel
{
    public class UpdateHotelDTO
    {
        public string name { get; set; } = string.Empty;
        public string address { get; set; } = string.Empty;
        public string city { get; set; } = string.Empty;
        public IFormFile? hotelImage { get; set; }  // Accept file upload
    }
}
