using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.StatisticsService.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class SyncHeroMapStatsNamesWithCatalog : Migration
    {
        /// <inheritdoc />
        /// <remarks>
        /// HeroStats/MapStats hold their own denormalized copy of Name (HeroStatsCoordinator only ever
        /// adds a row for a hero it hasn't seen before - it never refreshes Name/Color/etc. for an
        /// existing row), so the Catalog name corrections applied in Unmatched.CatalogService's
        /// FixExistingCatalogNames migration left these tables out of sync. Confirmed against the live
        /// dev DB: only Name differs for these rows, Color/DeckSize/Hp/IsRanged already matched.
        /// Guarded by both Id and the known-stale old name, so this is a no-op if already applied or
        /// if someone already fixed it by hand.
        /// </remarks>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE HeroStats SET Name = N'Sherlock Holmes' WHERE HeroId = '249543AE-06F0-47BB-B301-F1F312A622DB' AND Name = N'Sherlok Holmes';
UPDATE HeroStats SET Name = N'Robin Hood' WHERE HeroId = 'BA8DBE79-D8E3-4C3F-8FC5-D0B0D9F08360' AND Name = N'Robin hood';
UPDATE HeroStats SET Name = N'Jekyll & Hyde' WHERE HeroId = '1D4D861F-BC96-4F4D-9BA7-C97C2465E7E8' AND Name = N'Jakyl and Hide';
UPDATE HeroStats SET Name = N'Ghost Rider' WHERE HeroId = '1D0CAD01-0D35-4308-A662-CEA71672D11E' AND Name = N'Ghostrider';
UPDATE HeroStats SET Name = N'Robert Muldoon' WHERE HeroId = '92391E95-D634-4B65-8872-669717F54623' AND Name = N'Ingen';
UPDATE HeroStats SET Name = N'Yennenga' WHERE HeroId = '1FFFD192-23E7-494D-8ABA-9FB756B03B35' AND Name = N'Princess Yennenga';
UPDATE HeroStats SET Name = N'Sinbad' WHERE HeroId = 'E84E2C68-FAA0-41FD-AAE5-49A97CBC46B6' AND Name = N'Sindbad';

UPDATE MapStats SET Name = N'Hell''s Kitchen' WHERE MapId = 'B1D4D228-0D31-4E9D-AA1D-08DBA3E73224' AND Name = N'Hells Kitchen';
UPDATE MapStats SET Name = N'Raptor Paddock' WHERE MapId = '2AC482BE-3EB0-4DAD-AA1F-08DBA3E73224' AND Name = N'Raptor paddock';
UPDATE MapStats SET Name = N'T. Rex Paddock' WHERE MapId = 'CB1FB717-7DBA-4AE1-AA20-08DBA3E73224' AND Name = N'T. Rex paddock';
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
