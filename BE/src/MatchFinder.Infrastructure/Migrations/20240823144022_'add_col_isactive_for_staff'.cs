using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatchFinder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_col_isactive_for_staff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Staffs");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Staffs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Staffs");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Staffs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}