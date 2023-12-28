using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddHeroColor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Heroes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Heroes");
        }
    }
}
