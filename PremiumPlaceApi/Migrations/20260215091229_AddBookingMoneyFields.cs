using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PremiumPlace_API.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingMoneyFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookings_PlaceId_CheckInDate_CheckOutDate",
                table: "Bookings");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "Bookings",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "Bookings",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PlaceId_Status_CheckInDate_CheckOutDate",
                table: "Bookings",
                columns: new[] { "PlaceId", "Status", "CheckInDate", "CheckOutDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookings_PlaceId_Status_CheckInDate_CheckOutDate",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Bookings");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PlaceId_CheckInDate_CheckOutDate",
                table: "Bookings",
                columns: new[] { "PlaceId", "CheckInDate", "CheckOutDate" });
        }
    }
}
