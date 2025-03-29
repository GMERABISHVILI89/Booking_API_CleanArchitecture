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


            var (passwordHashAdmin, passwordSaltAdmin) = CreatePasswordHash("Admin!234");
            var (passwordHashUser, passwordSaltUser) = CreatePasswordHash("User!234");


            modelBuilder.Entity<User>().HasData(
                 new User
                 {
                  Id = 1,
                  UserName = "admin",
                  Email = "admin@email.com",
                  Role = "Admin",
                  PasswordHash = passwordHashAdmin,
                  PasswordSalt = passwordSaltAdmin
                 }
             );
            modelBuilder.Entity<User>().HasData(
              new User
              {
                  Id = 2,
                  UserName = "user",
                  Email = "user@email.com",
                  Role = "User",
                  PasswordHash = passwordHashUser,
                  PasswordSalt = passwordSaltUser
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




            SeedRoomsWithImages(modelBuilder);

            base.OnModelCreating(modelBuilder);

        }
        private void SeedRoomsWithImages(ModelBuilder modelBuilder)
        {
            // Seed Rooms for Chembers Grand Hotel (Id = 1)
            modelBuilder.Entity<Room>().HasData(
                new Room
                {
                    Id = 1,
                    Name = "Deluxe King Room",
                    HotelId = 1,
                    PricePerNight = 250.00m,
                    Available = true,
                    MaximumGuests = 2,
                    RoomTypeId = 2 
                },
                new Room
                {
                    Id = 2,
                    Name = "Executive Suite",
                    HotelId = 1,
                    PricePerNight = 450.00m,
                    Available = true,
                    MaximumGuests = 4,
                    RoomTypeId = 3 
                }
            );

            // Seed Rooms for Episode Retreat (Id = 2)
            modelBuilder.Entity<Room>().HasData(
                new Room
                {
                    Id = 3,
                    Name = "Riverside View Room",
                    HotelId = 2,
                    PricePerNight = 200.00m,
                    Available = true,
                    MaximumGuests = 2,
                    RoomTypeId = 1
                },
                new Room
                {
                    Id = 4,
                    Name = "Garden View Suite",
                    HotelId = 2,
                    PricePerNight = 350.00m,
                    Available = true,
                    MaximumGuests = 3,
                    RoomTypeId = 2
                }
            );

            // Seed Images for Chembers Grand Hotel Rooms
            modelBuilder.Entity<Image>().HasData(
                // Images for Deluxe King Room (Id = 1)
                new Image
                {
                    Id = 1,
                    RoomId = 1,
                    roomImage = GenerateSampleRoomImagePath("Chembers Grand Hotel", "Deluxe King Room", 1)
                },
                new Image
                {
                    Id = 2,
                    RoomId = 1,
                    roomImage = GenerateSampleRoomImagePath("Chembers Grand Hotel", "Deluxe King Room", 2)
                },
                // Images for Executive Suite (Id = 2)
                new Image
                {
                    Id = 3,
                    RoomId = 2,
                    roomImage = GenerateSampleRoomImagePath("Chembers Grand Hotel", "Executive Suite", 1)
                },
                new Image
                {
                    Id = 4,
                    RoomId = 2,
                    roomImage = GenerateSampleRoomImagePath("Chembers Grand Hotel", "Executive Suite", 2)
                },

                // Images for Episode Retreat Rooms
                // Images for Riverside View Room (Id = 3)
                new Image
                {
                    Id = 5,
                    RoomId = 3,
                    roomImage = GenerateSampleRoomImagePath("Episode Retreat", "Riverside View Room", 1)
                },
                new Image
                {
                    Id = 6,
                    RoomId = 3,
                    roomImage = GenerateSampleRoomImagePath("Episode Retreat", "Riverside View Room", 2)
                },
                // Images for Garden View Suite (Id = 4)
                new Image
                {
                    Id = 7,
                    RoomId = 4,
                    roomImage = GenerateSampleRoomImagePath("Episode Retreat", "Garden View Suite", 1)
                },
                new Image
                {
                    Id = 8,
                    RoomId = 4,
                    roomImage = GenerateSampleRoomImagePath("Episode Retreat", "Garden View Suite", 2)
                }
            );
        }
        public static string GenerateSampleRoomImagePath(string hotelName, string roomName, int imageIndex)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/room_images");
            Directory.CreateDirectory(uploadsFolder);

            // Create a consistent filename based on hotel name, room name, and index
            string fileExtension = ".jpg";
            string uniqueFileName = $"{hotelName.Replace(" ", "_").ToLower()}-{roomName.Replace(" ", "_").ToLower()}-{imageIndex}{fileExtension}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Return the relative path for database storage
            return $"/room_images/{uniqueFileName}";
        }




    }
}