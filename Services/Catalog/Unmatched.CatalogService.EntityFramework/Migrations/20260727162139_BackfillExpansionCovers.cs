using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.CatalogService.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class BackfillExpansionCovers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlFile = Path.Combine(AppContext.BaseDirectory, "Migrations", "Scripts", "BackfillExpansionCovers.sql");
            migrationBuilder.Sql(File.ReadAllText(sqlFile));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE Expansions SET ImageFileName = NULL
WHERE ImageFileName IN ('cobble-and-fog.png', 'battle-of-legends-3.png');
");
        }
    }
}
