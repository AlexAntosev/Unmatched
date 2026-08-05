using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.MatchService.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class FixTournamentParticipantsMismatchDetection : Migration
    {
        /// <summary>
        /// FixBackfilledTournamentParticipants shipped with a broken mismatch check: SQL Server evaluates
        /// "A EXCEPT B UNION B EXCEPT A" left-to-right as "(A EXCEPT B UNION B) EXCEPT A", which collapses
        /// to the empty set whenever the correct set B is a subset of the current set A - exactly the
        /// shape of the real bug (a bracket's true entrants are always a subset of a backfill that also
        /// swept in group-stage/stray matches), so it silently found nothing to fix. This redoes the same
        /// correction with each EXCEPT wrapped in its own derived table so UNION combines two already-
        /// complete difference sets instead of chaining into one big left-to-right expression.
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
                      SELECT HeroId FROM (
                          SELECT HeroId FROM TournamentParticipants p WHERE p.TournamentId = t.Id
                          EXCEPT
                          SELECT HeroId FROM CorrectParticipants cp WHERE cp.TournamentId = t.Id
                      ) AS OnlyInCurrent
                      UNION
                      SELECT HeroId FROM (
                          SELECT HeroId FROM CorrectParticipants cp WHERE cp.TournamentId = t.Id
                          EXCEPT
                          SELECT HeroId FROM TournamentParticipants p WHERE p.TournamentId = t.Id
                      ) AS OnlyInCorrect
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
            // No data reversal - same reasoning as FixBackfilledTournamentParticipants: which tournaments
            // this touches is only known at Up-time.
        }
    }
}
