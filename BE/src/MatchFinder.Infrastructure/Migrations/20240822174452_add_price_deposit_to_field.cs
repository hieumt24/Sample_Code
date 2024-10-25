using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatchFinder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_price_deposit_to_field : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "PartialFields");

            migrationBuilder.DropColumn(
                name: "Deposit",
                table: "PartialFields");

            migrationBuilder.DropColumn(
                name: "SlotTime",
                table: "Fields");

            migrationBuilder.AddColumn<decimal>(
                name: "Deposit",
                table: "Fields",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Fields",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deposit",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Fields");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "PartialFields",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<float>(
                name: "Deposit",
                table: "PartialFields",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SlotTime",
                table: "Fields",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}