using Booking_Domain.Entities;
using Booking_Domain.Models;
using Microsoft.EntityFrameworkCore;
namespace Booking_Infrastructure.Persistence // Change to the correct namespace
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<BookedDate> BookedDates { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<ExceptionLogs> ExceptionLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            static (byte[] passwordHash, byte[] passwordSalt) CreatePasswordHash(string password)
            {
                using (var hmac = new System.Security.Cryptography.HMACSHA512())
                {
                    var passwordSalt = hmac.Key;
                    var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                    return (passwordHash, passwordSalt);
                }
            }


            var (passwordHash, passwordSalt) = CreatePasswordHash("Admin!234");
            modelBuilder.Entity<User>().HasData(
                 new User
                 {
                  Id = 1,
                  UserName = "admin",
                  Email = "admin@email.com",
                  Role = "Admin",
                  PasswordHash = passwordHash,
                  PasswordSalt = passwordSalt
                  }
             );


            // Ensure Id is the primary key for each model
            modelBuilder.Entity<Room>().HasKey(r => r.Id);
            modelBuilder.Entity<BookedDate>().HasKey(b => b.Id);
            modelBuilder.Entity<Image>().HasKey(i => i.Id);

            modelBuilder.Entity<RoomType>().HasData(
                new RoomType { Id = 1, TypeName = "Single Room" },
                new RoomType { Id = 2, TypeName = "Double Room" },
                new RoomType { Id = 3, TypeName = "Triple Room" },
                new RoomType { Id = 4, TypeName = "Deluxe Room" },
                new RoomType { Id = 5, TypeName = "Family Room" }
            );


            //Add Hotels On Initial
            static string GenerateSampleImagePath(string hotelName)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/hotel_images");
                Directory.CreateDirectory(uploadsFolder);

                // Use a consistent filename based on the hotel name
                string fileExtension = ".jpg";
                string uniqueFileName = $"{hotelName.Replace(" ", "_").ToLower()}{fileExtension}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Return the relative path for database storage
                return $"/hotel_images/{uniqueFileName}";
            }

            // Seed Hotels
            modelBuilder.Entity<Hotel>().HasData(
                new Hotel
                {
                    Id = 1,
                    Name = "Chembers Grand Hotel",
                    Address = "123 Main Street, City Center",
                    City = "Tbilisi",
                    hotelImage = "/hotel_images/tbilisi-chembers.JPG"
                },
                new Hotel
                {
                    Id = 2,
                    Name = "Episode Retreat",
                    Address = "456 River Road, Scenic Area",
                    City = "Kutaisi",
                    hotelImage = "/hotel_images/episode.JPG"
                }
            );



            base.OnModelCreating(modelBuilder);

        }
    }
}