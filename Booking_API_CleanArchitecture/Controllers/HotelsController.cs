
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Booking_Application.DTO_s.Hotel;
using Booking_Application.Interfaces;
using Booking_Domain.Models;

namespace Booking_API_CleanArchitecture.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class HotelsController : ControllerBase
    {
        private readonly IHotelService _hotelService;
        public HotelsController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        // Add Hotel
        [HttpPost("AddHotel")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceResponse<Hotel>>> AddHotel([FromForm] CreateHotelDTO hotelDTO, [FromForm] IFormFile hotelImage)
        {
            if (hotelDTO == null)
                return BadRequest(new ServiceResponse<Hotel> { Success = false, Message = "Invalid data." });

            var response = await _hotelService.AddHotel(hotelDTO, hotelImage);

            if (!response.Success)
                return Conflict(response);
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            if (!string.IsNullOrEmpty(response.Data?.hotelImage))
            {
                response.Data.hotelImage = baseUrl + response.Data.hotelImage;
            }
            return Ok(response);
        }

        // ✅ Get All Hotels
        [HttpGet("all")]
        public async Task<ActionResult<ServiceResponse<List<Hotel>>>> GetAllHotels()
        {
            var response = await _hotelService.GetAllHotels();

            if (response.Success)
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}";

                foreach (var hotel in response.Data)
                {
                    if (!string.IsNullOrEmpty(hotel.hotelImage))
                    {

                        hotel.hotelImage = baseUrl + hotel.hotelImage;
                    }
                }
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        // ✅ Get Hotel By ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<Hotel>>> GetHotelById(int id)
        {
            var response = await _hotelService.GetHotelById(id);

            if (response.Success)
            {

                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var baseUrlRoom = $"{Request.Scheme}://{Request.Host}/";


                if (!string.IsNullOrEmpty(response.Data?.hotelImage))
                {

                    response.Data.hotelImage = baseUrl + response.Data.hotelImage;
                }

                foreach (var room in response.Data!.Rooms)
                {
                    if (room.Images != null && room.Images.Any())
                    {

                        foreach (var image in room.Images)
                        {
                            image.roomImage = baseUrlRoom + image.roomImage;
                        }
                    }
                }

                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{hotelId}")]
        public async Task<ActionResult<ServiceResponse<bool>>> DeleteHotel(int hotelId)
        {
            var response = await _hotelService.DeleteHotel(hotelId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [Authorize(Roles = "Admin")]
        // ✅ Update Hotel
        [HttpPut("update/{hotelId}")]
        public async Task<ActionResult<ServiceResponse<Hotel>>> UpdateHotel(int hotelId,
            [FromForm] string name, [FromForm] string address,
            [FromForm] string city,
            [FromForm] IFormFile hotelImage)
        {
            var hotelDTO = new UpdateHotelDTO
            {
                name = name,
                address = address,
                city = city
            };
            if (hotelDTO == null)
                return BadRequest(new ServiceResponse<Hotel> { Success = false, Message = "Invalid data." });

            var response = await _hotelService.UpdateHotel(hotelId, hotelDTO, hotelImage);
            if (!response.Success)
                return Conflict(response);

            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            if (!string.IsNullOrEmpty(response.Data?.hotelImage))
            {
                response.Data.hotelImage = baseUrl + response.Data.hotelImage;
            }

            return Ok(response);
        }
    }
}
