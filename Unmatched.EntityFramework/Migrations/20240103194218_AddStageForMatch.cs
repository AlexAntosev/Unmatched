using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddStageForMatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Stage",
                table: "Matches",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stage",
                table: "Matches");
        }
    }
}
