using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace warehouse_BE.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class addPriceReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Pricce",
                table: "Reservations",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pricce",
                table: "Reservations");
        }
    }
}
