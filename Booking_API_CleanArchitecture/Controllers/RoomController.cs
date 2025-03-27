
using Microsoft.AspNetCore.Mvc;
using Booking_Application.Interfaces;
using Booking_Domain.Models;
using Booking_Application.DTO_s.Room;

namespace Booking_API_CleanArchitecture.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        // POST: api/room/add
        [HttpPost("add-room")]
        public async Task<ActionResult<ServiceResponse<RoomDTO>>> AddRoom([FromForm] CreateRoomDTO roomDTO, [FromForm] List<IFormFile> roomImages)
        {
            if (roomImages == null || roomImages.Count == 0)
            {
                return BadRequest(new ServiceResponse<RoomDTO>
                {
                    Success = false,
                    Message = "Please upload at least one image for the room."
                });
            }

            var response = await _roomService.AddRoom(roomDTO, roomImages);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        // PUT: api/room/update/{roomId}
        [HttpPut("update/{roomId}")]
        public async Task<ActionResult<ServiceResponse<RoomDTO>>> UpdateRoom(int roomId, [FromForm] CreateRoomDTO roomDTO, [FromForm] List<IFormFile> roomImages)
        {
            var response = await _roomService.UpdateRoom(roomId, roomDTO, roomImages);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        // GET: api/room/{roomId}
        [HttpGet("{roomId}")]
        public async Task<ActionResult<ServiceResponse<RoomDTO>>> GetRoomById(int roomId)
        {
            var response = await _roomService.GetRoomById(roomId);
            if (response.Success)
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}/";

             
                    if (response.Data.ImageUrls != null && response.Data.ImageUrls.Any())
                    {
                       response.Data.ImageUrls = response.Data.ImageUrls.Select(imageUrl => baseUrl + imageUrl).ToList();
                    }
                return Ok(response);
            }
            return NotFound(response);
        }

        // GET: api/room/hotel/{hotelId}
        [HttpGet("RoomsByHotelId/{hotelId}")]
        public async Task<ActionResult<ServiceResponse<List<RoomDTO>>>> GetRoomsByHotelId(int hotelId)
        {
            var response = await _roomService.GetRoomsByHotelId(hotelId);
            if (response.Success)
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}/";

                foreach (var room in response.Data)
                {
                    if (room.ImageUrls != null && room.ImageUrls.Any())
                    {
                  
                        room.ImageUrls = room.ImageUrls.Select(imageUrl => baseUrl + imageUrl).ToList();
                    }
                }
                return Ok(response);
            }
            return NotFound(response);
        }

        // DELETE: api/room/delete/{roomId}
        [HttpDelete("delete/{roomId}")]
        public async Task<ActionResult<ServiceResponse<bool>>> DeleteRoom(int roomId)
        {
            var response = await _roomService.DeleteRoom(roomId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        // GET: api/room/all
        [HttpGet("all")]
        public async Task<ActionResult<ServiceResponse<List<RoomDTO>>>> GetAllRooms()
        {
            var response = await _roomService.GetAllRooms();
            if (response.Success)
            {
             
                var baseUrl = $"{Request.Scheme}://{Request.Host}/";

                foreach (var room in response.Data)
                {
                    if (room.ImageUrls != null && room.ImageUrls.Any())
                    {
                       
                        room.ImageUrls = room.ImageUrls.Select(imageUrl => baseUrl + imageUrl).ToList();
                    }
                }
                return Ok(response);
            }
            return NotFound(response);
        }

    }
}
