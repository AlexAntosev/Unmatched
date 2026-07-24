using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.CatalogService.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddImageFileName : Migration
    {
        /// <inheritdoc />
        /// <remarks>
        /// Guarded raw SQL (COL_LENGTH check) instead of raw AddColumn - same reasoning as
        /// AddExpansionEntity: safe whether EnsureCreated() already created the column on a fresh
        /// DB or Migrate() has to add it to an existing one.
        /// </remarks>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlFile = Path.Combine(AppContext.BaseDirectory, "Migrations", "Scripts", "AddImageFileName.sql");
            migrationBuilder.Sql(File.ReadAllText(sqlFile));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('Heroes', 'ImageFileName') IS NOT NULL ALTER TABLE Heroes DROP COLUMN ImageFileName;
IF COL_LENGTH('Maps', 'ImageFileName') IS NOT NULL ALTER TABLE Maps DROP COLUMN ImageFileName;
IF COL_LENGTH('Villains', 'ImageFileName') IS NOT NULL ALTER TABLE Villains DROP COLUMN ImageFileName;
IF COL_LENGTH('Minions', 'ImageFileName') IS NOT NULL ALTER TABLE Minions DROP COLUMN ImageFileName;
");
        }
    }
}
