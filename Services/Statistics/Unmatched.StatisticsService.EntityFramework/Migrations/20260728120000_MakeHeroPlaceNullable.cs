using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.StatisticsService.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class MakeHeroPlaceNullable : Migration
    {
        /// <inheritdoc />
        /// <remarks>
        /// Heroes with no recorded matches no longer get a Place assigned by HeroPlaceAdjuster, so
        /// the column has to accept NULL instead of the placeholder 0 it used to get.
        /// </remarks>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Place",
                table: "HeroStats",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE HeroStats SET Place = 0 WHERE Place IS NULL;");

            migrationBuilder.AlterColumn<int>(
                name: "Place",
                table: "HeroStats",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
