
using Booking_Application.DTO_s.Room;
using Booking_Application.DTO_s.RoomType;
using Booking_Application.Interfaces;
using Booking_Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Booking_API_CleanArchitecture.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomTypeController : ControllerBase
    {
        private readonly IRoomTypeService _roomTypeService;
        public RoomTypeController(IRoomTypeService roomTypeService)
        {
            _roomTypeService = roomTypeService;
        }


        [HttpPost("add-roomType")]
        public async Task<ActionResult<ServiceResponse<RoomTypeDTO>>> AddRoomType(CreateRoomTypeDTO roomTypeDTO)
        {
            if (roomTypeDTO == null )
            {
                return BadRequest(new ServiceResponse<RoomTypeDTO>
                {
                    Success = false,
                    Message = "Please Insert RoomType."
                });
            }

            var response = await _roomTypeService.AddRoomType(roomTypeDTO);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("edit-roomType/{id}")] 
        public async Task<ActionResult<ServiceResponse<RoomTypeDTO>>> EditRoomType(int id,  UpdateRoomTypeDTO roomTypeDTO)
        {
            if (roomTypeDTO == null)
            {
                return BadRequest(new ServiceResponse<RoomTypeDTO>
                {
                    Success = false,
                    Message = "Please Insert RoomType."
                });
            }

            var response = await _roomTypeService.EditRoomType(id, roomTypeDTO);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("delete-roomType/{id}")] 
        public async Task<ActionResult<ServiceResponse<bool>>> DeleteRoomType(int id)
        {
            var response = await _roomTypeService.DeleteRoomType(id);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("get-all-roomTypes")] 
        public async Task<ActionResult<ServiceResponse<List<RoomTypeDTO>>>> GetAllRoomTypes()
        {
            var response = await _roomTypeService.GetAllRoomTypes();
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }





    }
}
