using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatchFinder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Handle_transactions_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Transactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}