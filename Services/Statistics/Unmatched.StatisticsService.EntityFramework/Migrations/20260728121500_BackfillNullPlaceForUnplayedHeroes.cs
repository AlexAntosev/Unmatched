using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.StatisticsService.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class BackfillNullPlaceForUnplayedHeroes : Migration
    {
        /// <inheritdoc />
        /// <remarks>
        /// MakeHeroPlaceNullable only widened the column - rows written before HeroPlaceAdjuster
        /// stopped ranking unplayed heroes still carry a stale non-null Place. This one-time
        /// backfill clears it for any hero with no recorded matches; HeroPlaceAdjuster keeps it
        /// clear going forward.
        /// </remarks>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE HeroStats SET Place = NULL WHERE TotalMatches = 0 AND Place IS NOT NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
