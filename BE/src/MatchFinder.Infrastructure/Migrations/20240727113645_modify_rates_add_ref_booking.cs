using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatchFinder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class modify_rates_add_ref_booking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Rates",
                table: "Rates");

            migrationBuilder.AddColumn<int>(
                name: "BookingId",
                table: "Rates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rates",
                table: "Rates",
                columns: new[] { "UserId", "BookingId" });

            migrationBuilder.CreateIndex(
                name: "IX_Rates_BookingId",
                table: "Rates",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rates_Bookings_BookingId",
                table: "Rates",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rates_Bookings_BookingId",
                table: "Rates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rates",
                table: "Rates");

            migrationBuilder.DropIndex(
                name: "IX_Rates_BookingId",
                table: "Rates");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "Rates");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rates",
                table: "Rates",
                columns: new[] { "UserId", "FieldId" });
        }
    }
}