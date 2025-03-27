
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Security.Claims;
using Booking_Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Booking_Domain.Models;
using Booking_Application.DTO_s.Booking;
using Booking_Infrastructure.Persistence;


namespace Booking_Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;

    

        public BookingService(ApplicationDbContext context, IEmailService emailService, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
        }   

        public async Task<ServiceResponse<BookingDTO>> CreateBooking(BookingDTO bookingDto)
        {
            var response = new ServiceResponse<BookingDTO>();

            try
            {

                var loggedInUserId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var loggedInUserName = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value;



                if (loggedInUserId == null)
                {
                    response.Success = false;
                    response.Message = "You are not authorized to make this booking.";
                    return response;
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == int.Parse(loggedInUserId));


                // Check if the room is available for the requested dates
                var room = await _context.Rooms
                                          .Include(r => r.BookedDates)
                                          .FirstOrDefaultAsync(r => r.Id == bookingDto.RoomId);

                if (room == null)
                {
                    response.Success = false;
                    response.Message = "Room not found.";
                    return response;
                }

                // Check if the room is available during the requested dates
                var conflictingBooking = room.BookedDates
                                              .Any(b => (b.StartDate < bookingDto.CheckOutDate && b.EndDate > bookingDto.CheckInDate));

                if (conflictingBooking)
                {
                    response.Success = false;
                    response.Message = "The room is already booked for the selected dates.";
                    return response;
                }

                // Proceed with booking the room
                var booking = new Booking
                {
                    RoomId = bookingDto.RoomId,
                    CheckInDate = bookingDto.CheckInDate,
                    CheckOutDate = bookingDto.CheckOutDate,
                    TotalPrice = bookingDto.TotalPrice,
                    CustomerName = loggedInUserName!,
                    CustomerId = loggedInUserId,
                    CustomerPhone = bookingDto.CustomerPhone
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                // Update the Room's BookedDates
                var bookedDate = new BookedDate
                {
                    RoomId = bookingDto.RoomId,
                    StartDate = bookingDto.CheckInDate,
                    EndDate = bookingDto.CheckOutDate
                };

                // Add the new booked date to the Room
                room.BookedDates.Add(bookedDate);
                await _context.SaveChangesAsync(); // Save the changes to the Room's BookedDates


                response.Data = _mapper.Map<BookingDTO>(booking);

                response.Message = "Booking created successfully.";
                response.Success = true;

                try
                {
                    var roomfromDb = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == booking.RoomId);
                    var roomName = "";
                    var hotelName = "";

                    if (roomfromDb != null)
                    {
                        roomName = roomfromDb.Name;
              
                        var hotel = await _context.Hotels
                            .Include(r => r.Rooms)
                            .FirstOrDefaultAsync(r => r.Rooms.Any(r => r.Id == roomfromDb.Id));

                        if(hotel != null)
                        {
                            hotelName = hotel.Name;
                        }
                    }
                    MailRequest mail = new MailRequest();

                    mail.ToEmail = user!.Email;
                    mail.Subject = "Booking Room";
                    mail.Body = $@"
                        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                        <div style='background-color: #4CAF50; color: white; padding: 10px; text-align: center; font-size: 20px; border-radius: 10px 10px 0 0;'>
                        Room Booking Confirmation
                         </div>
                        <div style='padding: 20px;'>
                        <p>Dear <strong>{booking.CustomerName}</strong>,</p>
                        <p>We are pleased to confirm your room booking. Here are your booking details:</p>
                        <table border='0' cellpadding='5'>
                        <tr><td><strong>Hotel :</strong></td><td>{hotelName}</td></tr>
                        <tr><td><strong>Room :</strong></td><td>{roomName}</td></tr>
                        <tr><td><strong>Check-in Date:</strong></td><td>{booking.CheckInDate:yyyy-MM-dd}</td></tr>
                        <tr><td><strong>Check-out Date:</strong></td><td>{booking.CheckOutDate:yyyy-MM-dd}</td></tr>
                        <tr><td><strong>Total Price:</strong></td><td>${booking.TotalPrice} GEL</td></tr>
                        <tr><td><strong>Customer Name:</strong></td><td>{booking.CustomerName}</td></tr>
                        <tr><td><strong>Customer Phone:</strong></td><td>{booking.CustomerPhone}</td></tr>
                        </table>
                            <p>If you have any special requests or need assistance, feel free to contact us.</p>
                            <p>We look forward to welcoming you!</p>
                        </div>
                        <div style='text-align: center; font-size: 14px; margin-top: 20px; color: #555;'>
                            Best regards,<br>
                            <strong>Booking Hotels Georgia</strong><br>
                            <a href='mailto:BookignHotels@email.com'>BookignHotels@email.com</a> | <a href='tel:+1234567890'>+1234567890</a>
                        </div>
                        </div>";

                    await _emailService.SendEmailAsync(mail);
                }
                catch (Exception ex)
                {
                    response.Success = true;
                    response.Message = $"Error sending email: {ex.Message}";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error creating booking: {ex.Message}";
            }

            return response;
        }


        public async Task<ServiceResponse<List<BookingWithImageDTO>>> GetBookings(string userId)
        {
            var response = new ServiceResponse<List<BookingWithImageDTO>>();
            try
            {
                var bookings = await _context.Bookings
                    .Where(b => b.CustomerId ==userId) // Filter bookings by user ID
                    .ToListAsync();

                var bookingDTOs = new List<BookingWithImageDTO>();

                foreach (var booking in bookings)
                {
                    var room = await _context.Rooms
          .Include(r => r.Images) 
          .FirstOrDefaultAsync(r => r.Id == booking.RoomId);
                    if (room != null)
                    {
                        var bookingDTO = _mapper.Map<BookingWithImageDTO>(booking);

                     
                        if (room.Images != null && room.Images.Any())
                        {
                            bookingDTO.RoomImage = room.Images[0].roomImage.ToString();
                        }
                        else
                        {
                            bookingDTO.RoomImage = null; 
                        }

                        bookingDTOs.Add(bookingDTO);
                    }
                    else
                    {
                        Console.WriteLine($"Room with ID {booking.RoomId} not found for booking ID {booking.Id}");
                        var bookingDTO = _mapper.Map<BookingWithImageDTO>(booking); 
                        bookingDTO.RoomImage = null;
                        bookingDTOs.Add(bookingDTO);
                    }
                }
                response.Data = bookingDTOs;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving user bookings: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<BookingDTO>> GetBookingById(int bookingId)
        {
            var response = new ServiceResponse<BookingDTO>();
            try
            {
                var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
                if (booking == null)
                {
                    response.Success = false;
                    response.Message = "Booking not found.";
                }
                else
                {
                    response.Data = _mapper.Map<BookingDTO>(booking);
                    response.Message = "Booking retrieved successfully.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving booking: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<BookingDTO>> UpdateBooking(int bookingId, BookingDTO bookingDto)
        {
            var response = new ServiceResponse<BookingDTO>();
            try
            {
                var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
                if (booking == null)
                {
                    response.Success = false;
                    response.Message = "Booking not found.";
                }
                else
                {
                    _mapper.Map(bookingDto, booking);
                    _context.Bookings.Update(booking);
                    await _context.SaveChangesAsync();

                    response.Data = _mapper.Map<BookingDTO>(booking);
                    response.Message = "Booking updated successfully.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error updating booking: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteBooking(int id, string userId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var booking = await _context.Bookings.FindAsync(id);

                if (booking == null)
                {
                    response.Success = false;
                    response.Message = "Booking not found.";
                    return response;
                }


                var existingBooking = await _context.BookedDates.FirstOrDefaultAsync(b =>
                    b.RoomId == booking.RoomId &&
                    b.StartDate == booking.CheckInDate &&
                    b.EndDate == booking.CheckOutDate);

                if (existingBooking != null)
                {
                    if (existingBooking.StartDate == booking.CheckInDate && existingBooking.EndDate == booking.CheckOutDate)
                    {
                        _context.BookedDates.Remove(existingBooking);
                        await _context.SaveChangesAsync();
                    }
                }


                if (booking.Id != id)
                {
                    response.Success = false;
                    response.Message = "You are not authorized to delete this booking.";
                    return response;
                }

                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();

                response.Success = true;
                response.Data = true; 
                response.Message = "Booking deleted successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error deleting booking: {ex.Message}";
            }

            return response;
        }
    }

}
