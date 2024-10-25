using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatchFinder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class modify_transaction_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookingId",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BookingId",
                table: "Transactions",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Bookings_BookingId",
                table: "Transactions",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Bookings_BookingId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_BookingId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "Transactions");
        }
    }
}