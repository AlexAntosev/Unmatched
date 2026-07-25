using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.StatisticsService.EntityFramework.Migrations
{
    /// <inheritdoc />
    /// <remarks>
    /// Same situation as SyncHeroMapStatsNamesWithCatalog: HeroStatsCoordinator.CheckAndInitializeAsync
    /// only ever adds a row for a HeroId it hasn't seen before, it never refreshes Name for an existing
    /// row. The Catalog-side split of "Yennefer &amp; Triss" into "Triss" (same HeroId, existing stats row)
    /// plus a new "Yennefer" hero (new HeroId, will be picked up automatically as a new row) left this
    /// table's copy of the existing row's Name stale. Guarded by HeroId + the known-stale old name.
    /// </remarks>
    public partial class SyncTrissYenneferSplitNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE HeroStats SET Name = N'Triss' WHERE HeroId = 'EDCF90BE-E952-4208-B393-8809A32EED65' AND Name = N'Yennefer & Triss';
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
