using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Booking_API_CleanArchitecture.Migrations
{
    /// <inheritdoc />
    public partial class roomsandhotels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExceptionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusCode = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StackTrace = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatorId = table.Column<int>(type: "int", nullable: true),
                    ModifierId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExceptionLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hotels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    hotelImage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoomTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatorId = table.Column<int>(type: "int", nullable: true),
                    ModifierId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HotelId = table.Column<int>(type: "int", nullable: false),
                    PricePerNight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Available = table.Column<bool>(type: "bit", nullable: false),
                    MaximumGuests = table.Column<int>(type: "int", nullable: false),
                    RoomTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rooms_RoomTypes_RoomTypeId",
                        column: x => x.RoomTypeId,
                        principalTable: "RoomTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookedDates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookedDates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookedDates_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    CheckInDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckOutDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerPhone = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    roomImage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Images_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Hotels",
                columns: new[] { "Id", "Address", "City", "Name", "hotelImage" },
                values: new object[,]
                {
                    { 1, "123 Main Street, City Center", "Tbilisi", "Chembers Grand Hotel", "/hotel_images/tbilisi-chembers.JPG" },
                    { 2, "456 River Road, Scenic Area", "Kutaisi", "Episode Retreat", "/hotel_images/episode.JPG" }
                });

            migrationBuilder.InsertData(
                table: "RoomTypes",
                columns: new[] { "Id", "TypeName" },
                values: new object[,]
                {
                    { 1, "Single Room" },
                    { 2, "Double Room" },
                    { 3, "Triple Room" },
                    { 4, "Deluxe Room" },
                    { 5, "Family Room" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreateDate", "CreatorId", "Email", "LastModifiedDate", "ModifierId", "PasswordHash", "PasswordSalt", "RefreshToken", "RefreshTokenExpirationDate", "Role", "UserName" },
                values: new object[] { 1, new DateTime(2025, 3, 28, 12, 14, 41, 340, DateTimeKind.Local).AddTicks(4702), null, "admin@email.com", null, null, new byte[] { 48, 188, 51, 243, 141, 37, 25, 113, 95, 67, 212, 210, 216, 85, 51, 227, 198, 64, 168, 243, 24, 185, 138, 55, 146, 157, 16, 199, 193, 170, 182, 54, 57, 60, 136, 250, 123, 157, 135, 99, 49, 39, 213, 178, 144, 22, 117, 244, 149, 37, 160, 199, 133, 80, 39, 18, 38, 162, 232, 219, 201, 47, 78, 41 }, new byte[] { 11, 127, 178, 39, 161, 0, 84, 0, 55, 126, 6, 241, 115, 10, 163, 49, 180, 239, 93, 1, 87, 191, 234, 122, 34, 34, 36, 11, 24, 208, 40, 247, 244, 34, 188, 211, 80, 229, 173, 34, 221, 166, 178, 106, 7, 205, 169, 54, 71, 73, 117, 3, 148, 44, 158, 90, 119, 44, 239, 251, 21, 52, 135, 80, 218, 182, 234, 90, 93, 245, 197, 116, 94, 163, 228, 136, 211, 62, 142, 254, 216, 69, 164, 105, 133, 166, 42, 86, 218, 210, 108, 243, 183, 146, 100, 6, 138, 65, 224, 151, 16, 159, 220, 124, 154, 202, 87, 172, 143, 223, 132, 255, 17, 210, 199, 112, 101, 97, 234, 122, 216, 170, 31, 7, 201, 147, 201, 39 }, null, null, "Admin", "admin" });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "Available", "HotelId", "MaximumGuests", "Name", "PricePerNight", "RoomTypeId" },
                values: new object[,]
                {
                    { 1, true, 1, 2, "Deluxe King Room", 250.00m, 2 },
                    { 2, true, 1, 4, "Executive Suite", 450.00m, 3 },
                    { 3, true, 2, 2, "Riverside View Room", 200.00m, 1 },
                    { 4, true, 2, 3, "Garden View Suite", 350.00m, 2 }
                });

            migrationBuilder.InsertData(
                table: "Images",
                columns: new[] { "Id", "RoomId", "roomImage" },
                values: new object[,]
                {
                    { 1, 1, "/room_images/chembers_grand_hotel-deluxe_king_room-1.jpg" },
                    { 2, 1, "/room_images/chembers_grand_hotel-deluxe_king_room-2.jpg" },
                    { 3, 2, "/room_images/chembers_grand_hotel-executive_suite-1.jpg" },
                    { 4, 2, "/room_images/chembers_grand_hotel-executive_suite-2.jpg" },
                    { 5, 3, "/room_images/episode_retreat-riverside_view_room-1.jpg" },
                    { 6, 3, "/room_images/episode_retreat-riverside_view_room-2.jpg" },
                    { 7, 4, "/room_images/episode_retreat-garden_view_suite-1.jpg" },
                    { 8, 4, "/room_images/episode_retreat-garden_view_suite-2.jpg" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookedDates_RoomId",
                table: "BookedDates",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_RoomId",
                table: "Bookings",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_RoomId",
                table: "Images",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_HotelId",
                table: "Rooms",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_RoomTypeId",
                table: "Rooms",
                column: "RoomTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookedDates");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "ExceptionLogs");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Hotels");

            migrationBuilder.DropTable(
                name: "RoomTypes");
        }
    }
}
