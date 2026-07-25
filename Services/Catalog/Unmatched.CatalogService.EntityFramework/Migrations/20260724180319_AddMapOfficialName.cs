using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.CatalogService.EntityFramework.Migrations
{
    /// <inheritdoc />
    /// <remarks>
    /// Written as guarded raw SQL instead of the EF-generated AddColumn call, same as
    /// AddExpansionEntity - Program.cs calls both EnsureCreated() and Migrate() on startup, so a
    /// non-guarded AddColumn would fail with "column already exists" on a genuinely empty database.
    /// </remarks>
    public partial class AddMapOfficialName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlFile = Path.Combine(AppContext.BaseDirectory, "Migrations", "Scripts", "AddMapOfficialName.sql");
            migrationBuilder.Sql(File.ReadAllText(sqlFile));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("IF COL_LENGTH('Maps', 'OfficialName') IS NOT NULL ALTER TABLE Maps DROP COLUMN OfficialName;");
        }
    }
}
