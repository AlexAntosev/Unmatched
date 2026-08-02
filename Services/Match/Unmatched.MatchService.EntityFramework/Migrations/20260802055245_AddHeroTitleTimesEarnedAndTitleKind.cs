using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.MatchService.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddHeroTitleTimesEarnedAndTitleKind : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Kind",
                table: "Titles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TimesEarned",
                table: "HeroTitle",
                type: "int",
                nullable: false,
                defaultValue: 1);

            // Backfill Kind for existing tournament-scoped titles from the label baked into Name by
            // TournamentTitleAwarder ("{KindLabel} of {TournamentName}") - a one-time reconciliation,
            // new rows get Kind stamped directly going forward.
            migrationBuilder.Sql(
                "UPDATE Titles SET Kind = 0 WHERE TournamentId IS NOT NULL AND Name LIKE 'Champion of %'; " +
                "UPDATE Titles SET Kind = 1 WHERE TournamentId IS NOT NULL AND Name LIKE 'Runner-Up of %'; " +
                "UPDATE Titles SET Kind = 2 WHERE TournamentId IS NOT NULL AND Name LIKE 'Iron Chin of %'; " +
                "UPDATE Titles SET Kind = 3 WHERE TournamentId IS NOT NULL AND Name LIKE 'Card Shark of %'; " +
                "UPDATE Titles SET Kind = 4 WHERE TournamentId IS NOT NULL AND Name LIKE 'Executioner of %'; " +
                "UPDATE Titles SET Kind = 5 WHERE TournamentId IS NOT NULL AND Name LIKE 'Cinderella of %';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Kind",
                table: "Titles");

            migrationBuilder.DropColumn(
                name: "TimesEarned",
                table: "HeroTitle");
        }
    }
}
