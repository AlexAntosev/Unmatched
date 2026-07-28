using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.CatalogService.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddExpansionImageFileName : Migration
    {
        /// <inheritdoc />
        /// <remarks>
        /// Guarded raw SQL rather than AddColumn, matching AddImageFileName: services run
        /// EnsureCreated() before Migrate(), so on a fresh database the column already exists.
        /// </remarks>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('Expansions', 'ImageFileName') IS NULL
    ALTER TABLE Expansions ADD ImageFileName NVARCHAR(MAX) NULL;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('Expansions', 'ImageFileName') IS NOT NULL ALTER TABLE Expansions DROP COLUMN ImageFileName;
");
        }
    }
}
