using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.MatchService.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class FixBackfilledTournamentParticipants : Migration
    {
        /// <summary>
        /// The original backfill in AddTournamentFormatAndParticipants counted every hero who ever fought
        /// in ANY match tied to a tournament, regardless of stage - for tournaments whose recorded matches
        /// include an earlier Group stage (a real part of the tournament, just not the bracket the UI
        /// sizes/displays from InitialStage) or stray non-bracket matches that merely share the
        /// TournamentId, that inflated MaxParticipants/TournamentParticipants far beyond who actually
        /// entered the bracket. The correct set is the distinct heroes fighting in matches at exactly
        /// Tournament.InitialStage - self-limiting to only tournaments where that set is non-empty and
        /// actually differs from what's stored keeps this a no-op for already-correct tournaments (and for
        /// formats like Bounty/League/Swiss where Stage/InitialStage isn't a bracket-entry concept).
        /// </summary>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF OBJECT_ID('tempdb..#AffectedTournaments') IS NOT NULL DROP TABLE #AffectedTournaments;

                ;WITH CorrectParticipants AS (
                    SELECT DISTINCT m.TournamentId, f.HeroId
                    FROM Fighters f
                    JOIN Matches m ON m.Id = f.MatchId
                    JOIN Tournaments t ON t.Id = m.TournamentId
                    WHERE m.Stage = t.InitialStage
                )
                SELECT t.Id AS TournamentId
                INTO #AffectedTournaments
                FROM Tournaments t
                WHERE EXISTS (SELECT 1 FROM CorrectParticipants cp WHERE cp.TournamentId = t.Id)
                  AND EXISTS (
                      SELECT HeroId FROM TournamentParticipants p WHERE p.TournamentId = t.Id
                      EXCEPT
                      SELECT HeroId FROM CorrectParticipants cp WHERE cp.TournamentId = t.Id
                      UNION
                      SELECT HeroId FROM CorrectParticipants cp WHERE cp.TournamentId = t.Id
                      EXCEPT
                      SELECT HeroId FROM TournamentParticipants p WHERE p.TournamentId = t.Id
                  );
                """);

            migrationBuilder.Sql(
                """
                DELETE FROM TournamentParticipants
                WHERE TournamentId IN (SELECT TournamentId FROM #AffectedTournaments);
                """);

            migrationBuilder.Sql(
                """
                ;WITH CorrectParticipants AS (
                    SELECT DISTINCT m.TournamentId, f.HeroId
                    FROM Fighters f
                    JOIN Matches m ON m.Id = f.MatchId
                    JOIN Tournaments t ON t.Id = m.TournamentId
                    WHERE m.Stage = t.InitialStage
                )
                INSERT INTO TournamentParticipants (Id, TournamentId, HeroId, FinalPlacement)
                SELECT NEWID(), cp.TournamentId, cp.HeroId, NULL
                FROM CorrectParticipants cp
                WHERE cp.TournamentId IN (SELECT TournamentId FROM #AffectedTournaments);
                """);

            migrationBuilder.Sql(
                """
                UPDATE t
                SET MaxParticipants = (SELECT COUNT(*) FROM TournamentParticipants p WHERE p.TournamentId = t.Id)
                FROM Tournaments t
                WHERE t.Id IN (SELECT TournamentId FROM #AffectedTournaments);
                """);

            migrationBuilder.Sql("DROP TABLE #AffectedTournaments;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No data reversal, same as AddTournamentFormatAndParticipants's own backfill: which
            // tournaments this touches is only known at Up-time, and re-inflating MaxParticipants back to
            // the wrong number isn't a meaningful rollback to offer.
        }
    }
}
