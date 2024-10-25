using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatchFinder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class modify_table_opponentFindingRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OpponentFindingRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "OpponentFindingRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "OpponentFindingRequests",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "OpponentFindingRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedBy",
                table: "OpponentFindingRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OpponentFindingRequests");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "OpponentFindingRequests");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "OpponentFindingRequests");

            migrationBuilder.DropColumn(
                name: "LastUpdatedAt",
                table: "OpponentFindingRequests");

            migrationBuilder.DropColumn(
                name: "LastUpdatedBy",
                table: "OpponentFindingRequests");
        }
    }
}