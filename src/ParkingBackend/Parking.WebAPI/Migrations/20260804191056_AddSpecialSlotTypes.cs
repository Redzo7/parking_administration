using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parking.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecialSlotTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorizedSlotTypes",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "ParkingSlots",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorizedSlotTypes",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ParkingSlots");
        }
    }
}
