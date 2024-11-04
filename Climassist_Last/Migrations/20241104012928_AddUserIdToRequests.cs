using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Climassist_Last.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Requests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Requests");
        }
    }
}
