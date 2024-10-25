using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatchFinder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_col_deposit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Deposit",
                table: "PartialFields",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DepositAmount",
                table: "Bookings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deposit",
                table: "PartialFields");

            migrationBuilder.DropColumn(
                name: "DepositAmount",
                table: "Bookings");
        }
    }
}