
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Booking_Application.Interfaces;
using Booking_Domain.Models;
using Booking_Application.DTO_s.Booking;

namespace Booking_API_CleanArchitecture.Controllers
{
 
    [Route("api/[controller]")]
    [ApiController]

    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;


        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost("addBooking")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult<ServiceResponse<BookingDTO>>> CreateBooking(BookingDTO bookingDto)
        {
            var response = await _bookingService.CreateBooking(bookingDto);

            if (response.Success)
            {
                return CreatedAtAction(nameof(GetBookingById), new { bookingId = response.Data.Id }, response);
            }

            return Ok(response); 
        }


        // GET: api/Booking
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<BookingWithImageDTO>>>> GetUserBookings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new ServiceResponse<List<BookingWithImageDTO>> { Success = false, Message = "User ID not found." });
            }

            var response = await _bookingService.GetBookings(userId);

            if (response.Success)
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}/";

                foreach (var room in response.Data)
                {
                    if (room.RoomImage != null && room.RoomImage.Any())
                    {
                        room.RoomImage = baseUrl + room.RoomImage;
                    }
                }
                return Ok(response);
            }

            return NotFound(response);
        }

        // GET: api/Booking/{id}   
        [HttpGet("{bookingId}")]
        public async Task<ActionResult<ServiceResponse<BookingDTO>>> GetBookingById(int bookingId)
        {
            var response = await _bookingService.GetBookingById(bookingId);

            if (response.Success)
            {
                return Ok(response);
            }

            return NotFound(response);
        }

        // DELETE: api/Booking/{id}
        [Authorize(Roles = "User,Admin")]
        [HttpDelete("{id}")] 
        public async Task<ActionResult<ServiceResponse<bool>>> DeleteBooking(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new ServiceResponse<bool> { Success = false, Message = "User ID not found." });
            }

            var response = await _bookingService.DeleteBooking(id, userId);

            if (response.Success)
            {
                return Ok(response);
            }

            return NotFound(response);
        }
    }
}
