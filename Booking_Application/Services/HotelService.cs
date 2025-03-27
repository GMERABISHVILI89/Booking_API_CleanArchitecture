using AutoMapper;

using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System;
using Booking_Application.Interfaces;
using Booking_Domain.Models;
using Booking_Application.DTO_s.Hotel;
using Microsoft.AspNetCore.Http;
using Booking_Infrastructure.Persistence;

namespace Booking_Application.Services
{
    public class HotelService : IHotelService
    {

        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public HotelService(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        // ✅ Get All Hotels
        public async Task<ServiceResponse<List<Hotel>>> GetAllHotels()
        {
            var response = new ServiceResponse<List<Hotel>>();
            try
            {
                var hotels = await _dbContext.Hotels.Include(h => h.Rooms).ThenInclude(i => i.Images).ToListAsync();

                response.Data = hotels;
                response.Success = true;
                response.Message = "Hotels retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        // ✅ Get Hotel By ID
        public async Task<ServiceResponse<Hotel>> GetHotelById(int id)
        {
            var response = new ServiceResponse<Hotel>();

            try
            {
                var hotel = await _dbContext.Hotels
                    .Include(h => h.Rooms)
                    .ThenInclude(r => r.Images) 
                    .FirstOrDefaultAsync(h => h.Id == id);

                if (hotel == null)
                {
                    response.Success = false;
                    response.Message = "Hotel not found";
                }
                else
                {
                    response.Data = hotel;
                    response.Success = true;
                    response.Message = "Hotel retrieved successfully.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ Add Hotel
        public async Task<ServiceResponse<Hotel>> AddHotel(CreateHotelDTO hotelDTO, IFormFile hotelImage)
        {
            var response = new ServiceResponse<Hotel>();

            try
            {
                // 🔹 Validate DTO
                if (string.IsNullOrEmpty(hotelDTO.name) || string.IsNullOrEmpty(hotelDTO.address))
                {
                    response.Success = false;
                    response.Message = "Hotel name and address are required.";
                    return response;
                }

                // 🔹 Check if the hotel already exists
                var existingHotel = await _dbContext.Hotels
                    .FirstOrDefaultAsync(h => h.Name == hotelDTO.name && h.Address == hotelDTO.address);

                if (existingHotel != null)
                {
                    response.Success = false;
                    response.Message = "A hotel with the same name and address already exists.";
                    return response;
                }

                // 🔹 Validate and Save Image
                if (hotelImage == null || hotelImage.Length == 0)
                {
                    response.Success = false;
                    response.Message = "Hotel image is required.";
                    return response;
                }

                if (hotelImage.Length > 5 * 1024 * 1024) // 5MB limit
                {
                    response.Success = false;
                    response.Message = "Image size must be less than 5MB.";
                    return response;
                }

                string imagePath = await SaveHotelImageAsync(hotelImage);

                // 🔹 Create and Save Hotel
                var hotel = new Hotel
                {
                    Name = hotelDTO.name,
                    Address = hotelDTO.address,
                    City = hotelDTO.city,
                    hotelImage = imagePath
                };

                _dbContext.Hotels.Add(hotel);
                await _dbContext.SaveChangesAsync();

                response.Data = hotel;
                response.Message = "Hotel added successfully!";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error occurred while adding the hotel.";
            }

            return response;
        }

        // ✅ Update Hotel
        public async Task<ServiceResponse<Hotel>> UpdateHotel(int hotelId, UpdateHotelDTO hotelDTO, IFormFile hotelImage)
        {
            var response = new ServiceResponse<Hotel>();

            try
            {
                // 🔹 Find the hotel by ID
                var hotel = await _dbContext.Hotels.FindAsync(hotelId);

                if (hotel == null)
                {
                    response.Success = false;
                    response.Message = "Hotel not found.";
                    return response;
                }

                // 🔹 Update hotel details
                hotel.Name = hotelDTO.name;
                hotel.Address = hotelDTO.address;
                hotel.City = hotelDTO.city;

                // 🔹 Update hotel image if a new image is provided
                if (hotelImage != null && hotelImage.Length > 0)
                {
                    // Delete the old image if it exists
                    if (!string.IsNullOrEmpty(hotel.hotelImage))
                    {
                        var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", hotel.hotelImage.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Save the new image
                    string newImagePath = await SaveHotelImageAsync(hotelImage);
                    hotel.hotelImage = newImagePath;
                }

                // 🔹 Save changes to the database
                _dbContext.Hotels.Update(hotel);
                await _dbContext.SaveChangesAsync();

                response.Data = hotel;
                response.Message = "Hotel updated successfully!";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error occurred while updating the hotel.";
            }

            return response;
        }

        // ✅ Delete Hotel
        public async Task<ServiceResponse<bool>> DeleteHotel(int id)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var hotel = await _dbContext.Hotels.FindAsync(id);
                if (hotel == null)
                {
                    response.Success = false;
                    response.Message = "Hotel not found.";
                    return response;
                }
                if (!string.IsNullOrEmpty(hotel.hotelImage))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", hotel.hotelImage.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                _dbContext.Hotels.Remove(hotel);
                await _dbContext.SaveChangesAsync();

                response.Data = true;
                response.Success = true;
                response.Message = "Hotel deleted successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }



        #region HelperMethods


        public async Task<string> SaveHotelImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("No image file provided.");

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
                throw new ArgumentException("Invalid file type. Only JPG/JPEG/PNG files are allowed.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/hotel_images");
            Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

            string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return $"/hotel_images/{uniqueFileName}"; // Return relative path to store in DB
        }
        #endregion
    }

}
