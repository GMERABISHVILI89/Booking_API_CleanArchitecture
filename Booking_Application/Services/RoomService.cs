using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Booking_Application.Interfaces;
using Booking_Domain.Models;
using Booking_Application.DTO_s.Room;
using Microsoft.AspNetCore.Http;
using Booking_Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
namespace Booking_Application.Services
{

    public class RoomService : IRoomService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMapper _mapper;

        public RoomService(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, IMapper mapper)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<RoomDTO>> AddRoom(CreateRoomDTO roomDTO, List<IFormFile> roomImages)
        {
            var response = new ServiceResponse<RoomDTO>();

            try
            {

                // Step 1: Validate room data
                if (roomDTO == null || roomDTO.HotelId <= 0 || roomDTO.RoomTypeId <= 0)
                {
                    response.Success = false;
                    response.Message = "Invalid room data.";
                    return response;
                }

                // Step 2: Ensure Hotel exists
                var hotel = await _context.Hotels.FindAsync(roomDTO.HotelId);
                if (hotel == null)
                {
                    response.Success = false;
                    response.Message = "Hotel not found.";
                    return response;
                }

                // Step 3: Create Room entity and save it to the database
                var newRoom = new Room
                {
                    Name = roomDTO.Name,
                    HotelId = roomDTO.HotelId,
                    PricePerNight = roomDTO.PricePerNight,
                    Available = true,
                    MaximumGuests = roomDTO.MaximumGuests,
                    RoomTypeId = roomDTO.RoomTypeId
                };

                // Add Room to DB
                _context.Rooms.Add(newRoom);
                await _context.SaveChangesAsync();  // Save to get Room Id

                // Step 4: Save room images if any are provided
                var imagePaths = new List<string>();
                if (roomImages != null && roomImages.Any())
                {
                    imagePaths = await SaveRoomImagesAsync(roomImages);
                }

                // Step 5: Add Image entries to the database and associate with the room
                var images = imagePaths.Select(path => new Image
                {
                    RoomId = newRoom.Id,
                    roomImage = path
                }).ToList();

                if (images.Any())
                {
                    _context.Images.AddRange(images);
                    await _context.SaveChangesAsync();
                }

                // Step 6: Attach images to the room entity and return DTO
                newRoom.Images = images;
                response.Data = _mapper.Map<RoomDTO>(newRoom);
                response.Message = "Room added successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error adding room: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<RoomDTO>> UpdateRoom(int roomId, CreateRoomDTO roomDTO, List<IFormFile> roomImages)
        {
            var response = new ServiceResponse<RoomDTO>();

            try
            {
                // Step 1: Validate room data
                if (roomDTO == null || roomDTO.HotelId <= 0 || roomDTO.RoomTypeId <= 0)
                {
                    response.Success = false;
                    response.Message = "Invalid room data.";
                    return response;
                }

                // Step 2: Ensure Room exists
                var room = await _context.Rooms.Include(r => r.Images).Include(h => h.Hotel).Include(t => t.RoomType).FirstOrDefaultAsync(r => r.Id == roomId);
                if (room == null)
                {
                    response.Success = false;
                    response.Message = "Room not found.";
                    return response;
                }

                // Step 3: Ensure Hotel exists
                var hotel = await _context.Hotels.FindAsync(roomDTO.HotelId);
                if (hotel == null)
                {
                    response.Success = false;
                    response.Message = "Hotel not found.";
                    return response;
                }

                // Step 4: Update room details
                room.Name = roomDTO.Name;
                room.HotelId = roomDTO.HotelId;
                room.PricePerNight = roomDTO.PricePerNight;
                room.MaximumGuests = roomDTO.MaximumGuests;
                room.RoomTypeId = roomDTO.RoomTypeId;

                // Step 5: Update images if new ones are uploaded
                if (roomImages != null && roomImages.Any())
                {
                    if (room.Images != null && room.Images.Any())
                    {
                        foreach (var image in room.Images)
                        {
                            var filePath = Path.Combine(_hostEnvironment.WebRootPath, "room_images", Path.GetFileName(image.roomImage)); // Get the file path.
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath); // Delete the file.
                            }
                        }
                    }

                    // Remove old images first
                    _context.Images.RemoveRange(room.Images!);
                    await _context.SaveChangesAsync();

                    // Save new images
                    var newImagePaths = await SaveRoomImagesAsync(roomImages);

                    // Add new images to the database
                    var newImages = newImagePaths.Select(path => new Image
                    {
                        RoomId = room.Id,
                        roomImage = path
                    }).ToList();

                    _context.Images.AddRange(newImages);
                    await _context.SaveChangesAsync();

                    // Assign new images to the room
                    room.Images = newImages;
                }

                // Step 6: Save changes to the room
                _context.Rooms.Update(room);
                await _context.SaveChangesAsync();

                response.Data = _mapper.Map<RoomDTO>(room);
                response.Message = "Room updated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error updating room: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<List<RoomDTO>>> GetAllRooms()
        {
            var response = new ServiceResponse<List<RoomDTO>>();

            try
            {
                var rooms = await _context.Rooms.Include(r => r.Images).Include(h => h.Hotel).Include(t => t.RoomType).ToListAsync();
                response.Data = _mapper.Map<List<RoomDTO>>(rooms);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving rooms: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<RoomDTO>> GetRoomById(int roomId)
        {
            var response = new ServiceResponse<RoomDTO>();

            try
            {
                var room = await _context.Rooms.Include(r => r.Images).Include(h => h.Hotel).Include(t => t.RoomType).FirstOrDefaultAsync(r => r.Id == roomId);
                if (room == null)
                {
                    response.Success = false;
                    response.Message = "Room not found.";
                    return response;
                }

                response.Data = _mapper.Map<RoomDTO>(room);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving room: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<List<RoomDTO>>> GetRoomsByHotelId(int hotelId)
        {
            var response = new ServiceResponse<List<RoomDTO>>();

            try
            {
                var rooms = await _context.Rooms.Include(r => r.Images).Include(h => h.Hotel).Include(t => t.RoomType)
                    .Where(r => r.HotelId == hotelId).ToListAsync();

                response.Data = _mapper.Map<List<RoomDTO>>(rooms);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving rooms for hotel {hotelId}: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteRoom(int roomId)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                var room = await _context.Rooms.Include(r => r.Images).FirstOrDefaultAsync(r => r.Id == roomId);
                if (room == null)
                {
                    response.Success = false;
                    response.Message = "Room not found.";
                    return response;
                }

                if (room.Images != null && room.Images.Any())
                {
                    foreach (var image in room.Images)
                    {
                        var filePath = Path.Combine(_hostEnvironment.WebRootPath, "room_images", Path.GetFileName(image.roomImage)); // Get the file path.
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath); // Delete the file.
                        }
                    }
                }
                // Delete images first
                _context.Images.RemoveRange(room.Images);

                // Delete room
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();

                response.Data = true;
                response.Message = "Room deleted successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error deleting room: {ex.Message}";
            }

            return response;
        }

        private async Task<List<string>> SaveRoomImagesAsync(List<IFormFile> roomImages)
        {
            var imagePaths = new List<string>();

            // Define allowed image extensions
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

            // Define image folder path
            var roomImagesFolder = Path.Combine(_hostEnvironment.WebRootPath, "room_images");

            // Ensure the directory exists (only create if it doesn't exist)
            if (!Directory.Exists(roomImagesFolder))
            {
                Directory.CreateDirectory(roomImagesFolder);
            }

            foreach (var image in roomImages)
            {
                // Get file extension and validate
                var fileExtension = Path.GetExtension(image.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    throw new InvalidOperationException("Invalid file type. Only .jpg, .jpeg, and .png are allowed.");
                }

                // Generate a unique file name
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(roomImagesFolder, fileName);

                // Save the image to the file system
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // Add the relative image path (for saving in DB)
                imagePaths.Add($"room_images/{fileName}");
            }

            return imagePaths;
        }
    }





}
