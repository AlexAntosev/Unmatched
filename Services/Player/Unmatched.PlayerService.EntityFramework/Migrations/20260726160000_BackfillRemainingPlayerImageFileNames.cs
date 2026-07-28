using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

using Unmatched.PlayerService.EntityFramework.Context;

#nullable disable

namespace Unmatched.EntityFramework.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(UnmatchedDbContext))]
    [Migration("20260726160000_BackfillRemainingPlayerImageFileNames")]
    public partial class BackfillRemainingPlayerImageFileNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlFile = Path.Combine(AppContext.BaseDirectory, "Migrations", "Scripts", "BackfillRemainingPlayerImageFileNames.sql");
            migrationBuilder.Sql(File.ReadAllText(sqlFile));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
