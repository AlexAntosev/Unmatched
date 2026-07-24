using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.CatalogService.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddExpansionEntity : Migration
    {
        /// <inheritdoc />
        /// <remarks>
        /// This is the first migration to change actual schema (earlier migrations were data-only
        /// .Sql() calls). Written as guarded raw SQL instead of the EF-generated CreateTable/AddColumn
        /// calls because Program.cs calls both EnsureCreated() and Migrate() on startup - on a genuinely
        /// empty database EnsureCreated() would already create these tables/columns from the current
        /// model, and a non-guarded CreateTable would then fail with "object already exists".
        /// </remarks>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlFile = Path.Combine(AppContext.BaseDirectory, "Migrations", "Scripts", "AddExpansionEntity.sql");
            migrationBuilder.Sql(File.ReadAllText(sqlFile));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID('FK_Heroes_Expansions_ExpansionId', 'F') IS NOT NULL ALTER TABLE Heroes DROP CONSTRAINT FK_Heroes_Expansions_ExpansionId;
IF OBJECT_ID('FK_Maps_Expansions_ExpansionId', 'F') IS NOT NULL ALTER TABLE Maps DROP CONSTRAINT FK_Maps_Expansions_ExpansionId;
IF OBJECT_ID('FK_Villains_Expansions_ExpansionId', 'F') IS NOT NULL ALTER TABLE Villains DROP CONSTRAINT FK_Villains_Expansions_ExpansionId;
IF OBJECT_ID('FK_Minions_Expansions_ExpansionId', 'F') IS NOT NULL ALTER TABLE Minions DROP CONSTRAINT FK_Minions_Expansions_ExpansionId;
IF COL_LENGTH('Heroes', 'ExpansionId') IS NOT NULL ALTER TABLE Heroes DROP COLUMN ExpansionId;
IF COL_LENGTH('Maps', 'ExpansionId') IS NOT NULL ALTER TABLE Maps DROP COLUMN ExpansionId;
IF COL_LENGTH('Villains', 'ExpansionId') IS NOT NULL ALTER TABLE Villains DROP COLUMN ExpansionId;
IF COL_LENGTH('Minions', 'ExpansionId') IS NOT NULL ALTER TABLE Minions DROP COLUMN ExpansionId;
IF OBJECT_ID('Expansions', 'U') IS NOT NULL DROP TABLE Expansions;
");
        }
    }
}
