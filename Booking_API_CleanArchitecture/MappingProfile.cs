using AutoMapper;
using Booking_Application.DTO_s.Booking;
using Booking_Application.DTO_s.Hotel;
using Booking_Application.DTO_s.Room;
using Booking_Application.DTO_s.RoomType;
using Booking_Domain.Models;

namespace Booking_API_CleanArchitecture
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<CreateHotelDTO, Hotel>()
           .ForMember(dest => dest.hotelImage, opt => opt.MapFrom(src => src.hotelImage));

            CreateMap<Hotel, CreateHotelDTO>()
                .ForMember(dest => dest.hotelImage, opt => opt.MapFrom(src => src.hotelImage));

            CreateMap<Room, RoomDTO>()
            .ForMember(dest => dest.RoomTypeName, opt => opt.MapFrom(src => src.RoomType.TypeName))
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.Images.Select(i => i.roomImage)))
            .ForMember(dest => dest.BookedDates, opt => opt.MapFrom(src => src.BookedDates
            .Select(bd => new BookedDateDTO
            {
            StartDate = bd.StartDate,
            EndDate = bd.EndDate
             }).ToList()));

            CreateMap<Booking, BookingWithImageDTO>()
           .ForMember(dest => dest.RoomImage, opt => opt.Ignore());
            CreateMap<ImageDTO, Image>();
            CreateMap<RoomType, RoomTypeGetDTO>(); // For single item retrieval
            CreateMap<RoomType, RoomTypeGetAllDTO>(); // For list retrieval

            CreateMap<RoomTypeDTO, RoomTypeGetAllDTO>();
            CreateMap<RoomTypeGetAllDTO, RoomTypeDTO>();

            CreateMap<CreateRoomTypeDTO, RoomType>(); // For creating a RoomType
            CreateMap<UpdateRoomTypeDTO, RoomType>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); // for updating a roomType.
            CreateMap<Booking, BookingDTO>();

            CreateMap<Room, FilteredRoomDTO>()
              .ForMember(dest => dest.imageUrls, opt => opt.MapFrom(src =>
                  src.Images.Select(img => new ImageDTO { roomImage = img.roomImage }).ToList()))
              .ForMember(dest => dest.BookedDates, opt => opt.MapFrom(src =>
                  src.BookedDates.Select(bd => new BookedDateDTO
                  {
                      StartDate = bd.StartDate,
                      EndDate = bd.EndDate
                  }).ToList()));

        }
    }
}
