using Booking_Application.DTO_s.Booking;
using Booking_Domain.Models;

namespace Booking_Application.Interfaces
{
    public interface IBookingService
    {
        Task<ServiceResponse<BookingDTO>> CreateBooking(BookingDTO bookingDto);
        Task<ServiceResponse<BookingDTO>> GetBookingById(int bookingId);
        Task<ServiceResponse<bool>> DeleteBooking(int id, string userId);
        Task<ServiceResponse<List<BookingWithImageDTO>>> GetBookings(string userId);
    }
}
