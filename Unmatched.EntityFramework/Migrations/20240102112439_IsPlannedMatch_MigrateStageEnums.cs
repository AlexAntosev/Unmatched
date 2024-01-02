using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class IsPlannedMatch_MigrateStageEnums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Tournaments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPlanned",
                table: "Matches",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "IsPlanned",
                table: "Matches");
        }
    }
}
