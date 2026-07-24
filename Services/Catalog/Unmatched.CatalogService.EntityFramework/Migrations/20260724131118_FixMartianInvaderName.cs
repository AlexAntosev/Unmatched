using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.CatalogService.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class FixMartianInvaderName : Migration
    {
        /// <inheritdoc />
        /// <remarks>
        /// "Martain Invader" is a typo (transposed letters) for the Martian villain from Unmatched
        /// Adventures: Tales to Amaze. See Migrations/Scripts/CatalogData/_name-corrections.json for
        /// sourcing. No VillainStats table exists in the DB yet (unlike HeroStats/MapStats), so there
        /// is no downstream denormalized copy to sync for this one.
        /// </remarks>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "IF EXISTS (SELECT 1 FROM Villains WHERE Name = N'Martain Invader') " +
                "UPDATE Villains SET Name = N'Martian Invader' WHERE Name = N'Martain Invader';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
