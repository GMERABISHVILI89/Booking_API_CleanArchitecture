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
        }
    }
}