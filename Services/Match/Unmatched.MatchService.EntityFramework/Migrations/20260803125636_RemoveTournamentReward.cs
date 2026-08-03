using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.MatchService.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTournamentReward : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reward",
                table: "Tournaments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reward",
                table: "Tournaments",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
