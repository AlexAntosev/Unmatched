using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

using Unmatched.PlayerService.EntityFramework.Context;

#nullable disable

namespace Unmatched.EntityFramework.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(UnmatchedDbContext))]
    [Migration("20260726100000_AddImageFileNameToPlayer")]
    public partial class AddImageFileNameToPlayer : Migration
    {
        /// <inheritdoc />
        /// <remarks>
        /// Guarded raw SQL (COL_LENGTH check) instead of raw AddColumn - safe whether EnsureCreated()
        /// already created the column on a fresh DB or Migrate() has to add it to an existing one.
        /// Same pattern as Catalog service's AddImageFileName migration.
        /// </remarks>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlFile = Path.Combine(AppContext.BaseDirectory, "Migrations", "Scripts", "AddImageFileNameToPlayer.sql");
            migrationBuilder.Sql(File.ReadAllText(sqlFile));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("IF COL_LENGTH('Players', 'ImageFileName') IS NOT NULL ALTER TABLE Players DROP COLUMN ImageFileName;");
        }
    }
}
