using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.CatalogService.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class FixExistingCatalogNames : Migration
    {
        /// <inheritdoc />
        /// <remarks>
        /// Corrects a handful of typos/casing mistakes in existing Hero/Sidekick/Map names, found by
        /// comparing them against official Unmatched sources (see Migrations/Scripts/CatalogData/_name-corrections.json
        /// for the full audit with citations). Runs before any Expansion backfill migration that
        /// matches rows by Name, so those don't zero in on the wrong (misspelled) values.
        /// </remarks>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlFile = Path.Combine(AppContext.BaseDirectory, "Migrations", "Scripts", "FixExistingCatalogNames.sql");
            migrationBuilder.Sql(File.ReadAllText(sqlFile));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
