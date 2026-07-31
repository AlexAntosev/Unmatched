using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.MatchService.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchIsRanked : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRanked",
                table: "Matches",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // Ranked-ness used to be implied by tournament membership - every match that already has a
            // TournamentId provably has complete HP/cards/sidekick data (the old calculator crashed
            // otherwise), so backfilling from that gives every existing match the same Ranked/Unranked
            // classification it already had under the old rules.
            migrationBuilder.Sql(
                "UPDATE Matches SET IsRanked = CASE WHEN TournamentId IS NULL THEN 0 ELSE 1 END");

            // Unranked matches (no TournamentId) never required this data, so some of it is UI-forced
            // filler rather than a real result - null it out instead of feeding fabricated numbers into
            // the ladder if these matches are ever flipped to Ranked. This is a one-way step: Down()
            // cannot restore whatever was here before.
            migrationBuilder.Sql(
                """
                UPDATE f SET HpLeft = NULL, SidekickHpLeft = NULL, CardsLeft = NULL
                FROM Fighters f
                JOIN Matches m ON m.Id = f.MatchId
                WHERE m.IsRanked = 0
                """);

            // guarded rather than a plain CreateIndex: if an earlier deploy attempt got this far before
            // crashing on a later migration without recording this one as applied, retrying would
            // otherwise fail with "index already exists" even though nothing is actually wrong.
            migrationBuilder.Sql(
                """
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Ratings_HeroId' AND object_id = OBJECT_ID(N'[Ratings]'))
                BEGIN
                    CREATE UNIQUE INDEX [IX_Ratings_HeroId] ON [Ratings] ([HeroId]);
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS [IX_Ratings_HeroId] ON [Ratings];");

            migrationBuilder.DropColumn(
                name: "IsRanked",
                table: "Matches");

            // Note: the HpLeft/SidekickHpLeft/CardsLeft values nulled out for Unranked fighters in Up()
            // cannot be recovered here - that data is genuinely gone, not just hidden.
        }
    }
}
