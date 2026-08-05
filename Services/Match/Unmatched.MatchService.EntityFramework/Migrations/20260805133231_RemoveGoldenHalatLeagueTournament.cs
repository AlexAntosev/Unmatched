using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.MatchService.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGoldenHalatLeagueTournament : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Golden Halat League's matches are already independently Ranked (IsRanked=1) - detaching
            // them from the tournament and deleting the tournament row doesn't change any hero's rating,
            // since rating recalculation (RatingTimeline.BuildAsync) filters purely on IsRanked, not
            // TournamentId.
            migrationBuilder.Sql(
                """
                IF EXISTS (SELECT 1 FROM Tournaments WHERE Name = 'Golden Halat League')
                BEGIN
                    DECLARE @tournamentId UNIQUEIDENTIFIER = (SELECT Id FROM Tournaments WHERE Name = 'Golden Halat League');

                    -- Titles.TournamentId and TournamentAwards.TournamentId have no FK/cascade
                    -- configured (see TitleEntity/TournamentAwardEntity) - deleted explicitly so
                    -- nothing is left orphaned. Deleting Titles cascades to HeroTitle automatically
                    -- (FK_HeroTitle_Titles_TitlesId is ON DELETE CASCADE).
                    DELETE FROM Titles WHERE TournamentId = @tournamentId;
                    DELETE FROM TournamentAwards WHERE TournamentId = @tournamentId;

                    -- Keep the matches and their IsRanked flag; only drop the tournament link.
                    UPDATE Matches SET TournamentId = NULL WHERE TournamentId = @tournamentId;

                    -- TournamentParticipants and TournamentTitles cascade-delete automatically
                    -- (both ON DELETE CASCADE in UnmatchedDbContext.OnModelCreating).
                    DELETE FROM Tournaments WHERE Id = @tournamentId;
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No data reversal, same reasoning as FixBackfilledTournamentParticipants: the tournament
            // row, its participants, and which matches pointed at it are only known at Up-time - hand-
            // reconstructing that here isn't a meaningful rollback to offer. Restore from a backup taken
            // before this migration runs if it needs to be undone.
        }
    }
}
