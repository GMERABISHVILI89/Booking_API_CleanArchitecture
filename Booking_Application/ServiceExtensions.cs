

using Booking_Application.Interfaces;
using Booking_Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Booking_Application
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register Application Layer services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IHotelService, HotelService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IFilterService, FilterService>();
            services.AddScoped<IRoomTypeService, RoomTypeService>();
            services.AddTransient<IEmailService, EmailService>();
            return services;
        }
    }
}
