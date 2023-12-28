using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHeroesColors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlFile = Path.Combine("Scripts", "20231228124300_UpdateHeroesColors.sql");
            migrationBuilder.Sql(File.ReadAllText(sqlFile));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var sqlFile = Path.Combine("Scripts", "20231228124300_UpdateHeroesColors_Revert.sql");
            migrationBuilder.Sql(File.ReadAllText(sqlFile));
        }
    }
}
