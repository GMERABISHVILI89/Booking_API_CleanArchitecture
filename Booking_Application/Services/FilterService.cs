using AutoMapper;
using Booking_Application.DTO_s.Hotel;
using Booking_Application.DTO_s.Room;
using Booking_Application.DTO_s.RoomType;
using Booking_Application.Interfaces;
using Booking_Domain.Models;
using Booking_Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Booking_Application.Services
{
    public class FilterService : IFilterService
    {

        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public FilterService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<List<FilteredRoomDTO>>> GetFilteredRooms(FilterDTO filter)
        {
            var response = new ServiceResponse<List<FilteredRoomDTO>>();

            try
            {
                var query = _context.Rooms
                    .Include(r => r.BookedDates)
                    .Include(r => r.Images)
                     .AsNoTracking()
                    .AsQueryable();

                // Filter by Room Type
                if (filter.RoomTypeId.HasValue && filter.RoomTypeId.Value > 0)
                {
                    query = query.Where(r => r.RoomTypeId == filter.RoomTypeId);
                }

                // Filter by Price Range
                if (filter.PriceFrom.HasValue)
                {
                    query = query.Where(r => r.PricePerNight >= filter.PriceFrom);
                }
                if (filter.PriceTo.HasValue && filter.PriceTo.Value > 0)
                {
                    query = query.Where(r => r.PricePerNight <= filter.PriceTo);
                }

                // Filter by Maximum Guests
                if (filter.MaximumGuests.HasValue && filter.MaximumGuests.Value > 0)
                {
                    query = query.Where(r => r.MaximumGuests >= filter.MaximumGuests);
                }

                // Filter by Availability (Check-in & Check-out)
                if (filter.CheckIn.HasValue && filter.CheckOut.HasValue)
                {
                    var checkIn = filter.CheckIn.Value.Date;
                    var checkOut = filter.CheckOut.Value.Date;

                    query = query.Where(r => !r.BookedDates
                        .Any(b => b.StartDate >= checkIn && b.EndDate < checkOut));
                }

                var rooms = await query.ToListAsync();

                response.Data = rooms.Select(r => new FilteredRoomDTO
                {
                    Id = r.Id,
                    Name = r.Name,
                    HotelId = r.HotelId,
                    PricePerNight = r.PricePerNight,
                    Available = r.Available,
                    MaximumGuests = r.MaximumGuests,
                    RoomTypeId = r.RoomTypeId,
                    BookedDates = r.BookedDates.Select(b => new BookedDateDTO
                    {
                        Id = b.Id,
                        StartDate = b.StartDate,
                        EndDate = b.EndDate,
                        RoomId = b.RoomId
                    }).ToList(),
                        imageUrls = r.Images.Select(i => new ImageDTO
                    {
                         roomImage = i.roomImage
                    }).ToList()
                }).ToList();

                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error fetching filtered rooms: " + ex.Message;
            }

            return response;
        }


        // Get available rooms based on the start and end dates
        public async Task<ServiceResponse<List<FilteredRoomDTO>>> GetAvailableRooms(DateTime startDate, DateTime endDate)
        {
            var response = new ServiceResponse<List<FilteredRoomDTO>>();

            try
            {
                var availableRooms = await _context.Rooms
                 .Where(r => !r.BookedDates.Any(b => (b.StartDate < endDate && b.EndDate > startDate)))
                 .Include(r => r.RoomType)
                 .Include(r => r.Images)
                 .Include(r => r.BookedDates) 
                 .ToListAsync();

                response.Data = _mapper.Map<List<FilteredRoomDTO>>(availableRooms);
                response.Message = "Available rooms retrieved successfully.";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred while retrieving available rooms: {ex.Message}";
            }

            return response;
        }

        // Get all room types
        public async Task<ServiceResponse<List<RoomTypeGetAllDTO>>> GetRoomTypes()
        {
            var response = new ServiceResponse<List<RoomTypeGetAllDTO>>();

            try
            {
                var roomTypes = await _context.RoomTypes.ToListAsync();
                response.Data = _mapper.Map<List<RoomTypeGetAllDTO>>(roomTypes);
                response.Message = "Room types retrieved successfully.";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred while retrieving room types: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<List<FilterByCityDTO>>> GetHotelsByCity(string city)
        {
            var response = new ServiceResponse<List<FilterByCityDTO>>();

            try
            {
                var hotels = await _context.Hotels
                    .Where(h => h.City == city)
                    .Select(h => new FilterByCityDTO 
                    {
                        
                        Name = h.Name,
                        Address = h.Address,
                        City = h.City,
                       hotelImage = h.hotelImage
                    })
                    .ToListAsync();

                response.Data = hotels;
                response.Success = true; 

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message; 
                                              
            }

            return response;
        }
    }
}
