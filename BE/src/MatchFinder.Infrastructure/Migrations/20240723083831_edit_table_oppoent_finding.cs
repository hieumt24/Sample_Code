using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatchFinder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class edit_table_oppoent_finding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EndTime",
                table: "OpponentFindings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FieldAddress",
                table: "OpponentFindings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FieldCommune",
                table: "OpponentFindings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FieldDistrict",
                table: "OpponentFindings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FieldName",
                table: "OpponentFindings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FieldProvince",
                table: "OpponentFindings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartTime",
                table: "OpponentFindings",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "OpponentFindings");

            migrationBuilder.DropColumn(
                name: "FieldAddress",
                table: "OpponentFindings");

            migrationBuilder.DropColumn(
                name: "FieldCommune",
                table: "OpponentFindings");

            migrationBuilder.DropColumn(
                name: "FieldDistrict",
                table: "OpponentFindings");

            migrationBuilder.DropColumn(
                name: "FieldName",
                table: "OpponentFindings");

            migrationBuilder.DropColumn(
                name: "FieldProvince",
                table: "OpponentFindings");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "OpponentFindings");
        }
    }
}