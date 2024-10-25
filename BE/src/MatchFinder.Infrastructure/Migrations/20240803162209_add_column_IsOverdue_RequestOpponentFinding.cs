using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatchFinder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_column_IsOverdue_RequestOpponentFinding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOverdue",
                table: "OpponentFindings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOverdue",
                table: "OpponentFindings");
        }
    }
}