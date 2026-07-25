using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.CatalogService.EntityFramework.Migrations
{
    /// <inheritdoc />
    /// <remarks>
    /// Written as guarded raw SQL instead of the EF-generated CreateTable call, same as
    /// AddExpansionEntity - Program.cs calls both EnsureCreated() and Migrate() on startup, so a
    /// non-guarded CreateTable would fail with "object already exists" on a genuinely empty database.
    /// </remarks>
    public partial class AddOwnedExpansion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlFile = Path.Combine(AppContext.BaseDirectory, "Migrations", "Scripts", "AddOwnedExpansion.sql");
            migrationBuilder.Sql(File.ReadAllText(sqlFile));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID('FK_OwnedExpansions_Expansions_ExpansionId', 'F') IS NOT NULL ALTER TABLE OwnedExpansions DROP CONSTRAINT FK_OwnedExpansions_Expansions_ExpansionId;
IF OBJECT_ID('OwnedExpansions', 'U') IS NOT NULL DROP TABLE OwnedExpansions;
");
        }
    }
}
