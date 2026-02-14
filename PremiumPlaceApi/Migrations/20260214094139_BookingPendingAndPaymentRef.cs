using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PremiumPlace_API.Migrations
{
    /// <inheritdoc />
    public partial class BookingPendingAndPaymentRef : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentRef",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentRef",
                table: "Bookings");
        }
    }
}
