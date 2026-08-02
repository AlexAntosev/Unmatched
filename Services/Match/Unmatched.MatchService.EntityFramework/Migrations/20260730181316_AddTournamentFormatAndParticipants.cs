using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.MatchService.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddTournamentFormatAndParticipants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // TournamentType {League=0, Championship=1} lines up by value with the new
            // TournamentFormat {League=0, SingleElimination=1, ...}, so this is a pure rename with no
            // data transformation - Championship tournaments become SingleElimination automatically.
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Tournaments",
                newName: "Format");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Tournaments");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Tournaments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Tournaments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageFileName",
                table: "Tournaments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxParticipants",
                table: "Tournaments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TrophyImageFileName",
                table: "Tournaments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Round",
                table: "Matches",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TournamentParticipants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HeroId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FinalPlacement = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentParticipants_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentTitles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Kind = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentTitles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentTitles_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // guarded rather than a plain CreateIndex - see AddMatchIsRanked for why: a partially-applied
            // earlier attempt can leave objects behind without recording the migration as complete.
            migrationBuilder.Sql(
                """
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_TournamentParticipants_TournamentId_HeroId' AND object_id = OBJECT_ID(N'[TournamentParticipants]'))
                BEGIN
                    CREATE UNIQUE INDEX [IX_TournamentParticipants_TournamentId_HeroId] ON [TournamentParticipants] ([TournamentId], [HeroId]);
                END
                """);

            migrationBuilder.Sql(
                """
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_TournamentTitles_TournamentId_Kind' AND object_id = OBJECT_ID(N'[TournamentTitles]'))
                BEGIN
                    CREATE UNIQUE INDEX [IX_TournamentTitles_TournamentId_Kind] ON [TournamentTitles] ([TournamentId], [Kind]);
                END
                """);

            // Backfill for tournaments that already existed:
            //  - Status: there's no reliable historical signal for "actually finished" (that concept
            //    only becomes real once Phase 4's CompleteAsync exists), so a tournament with any
            //    matches is conservatively InProgress rather than guessed at Completed; a truly
            //    finished one can be marked Completed by hand once that flow ships.
            migrationBuilder.Sql(
                """
                UPDATE Tournaments
                SET Status = CASE WHEN EXISTS (SELECT 1 FROM Matches m WHERE m.TournamentId = Tournaments.Id) THEN 1 ELSE 0 END
                """);

            //  - Participants: every hero who ever fought in the tournament's matches. Guarded with
            //    NOT EXISTS (not just naturally idempotent like the UPDATEs above) - re-running this
            //    insert unguarded on a retried migration would create duplicate participant rows.
            migrationBuilder.Sql(
                """
                INSERT INTO TournamentParticipants (Id, TournamentId, HeroId)
                SELECT NEWID(), m.TournamentId, f.HeroId
                FROM Fighters f
                JOIN Matches m ON m.Id = f.MatchId
                WHERE m.TournamentId IS NOT NULL
                  AND NOT EXISTS (
                      SELECT 1 FROM TournamentParticipants p
                      WHERE p.TournamentId = m.TournamentId AND p.HeroId = f.HeroId
                  )
                GROUP BY m.TournamentId, f.HeroId
                """);

            //  - MaxParticipants: the participant count just backfilled above.
            migrationBuilder.Sql(
                """
                UPDATE t
                SET MaxParticipants = (SELECT COUNT(*) FROM TournamentParticipants p WHERE p.TournamentId = t.Id)
                FROM Tournaments t
                """);

            //  - Champion (TournamentTitleKind = 0) is mandatory for every tournament, existing ones
            //    included. Guarded the same way as the participants insert above.
            migrationBuilder.Sql(
                """
                INSERT INTO TournamentTitles (Id, TournamentId, Kind)
                SELECT NEWID(), Id, 0
                FROM Tournaments
                WHERE NOT EXISTS (
                    SELECT 1 FROM TournamentTitles tt WHERE tt.TournamentId = Tournaments.Id AND tt.Kind = 0
                )
                """);

            // The three cover images that used to sit loose in wwwroot now live under
            // wwwroot/images/tournaments/ (same filenames), so the matching tournaments can point at them.
            migrationBuilder.Sql(
                """
                UPDATE Tournaments SET ImageFileName = 'Golden Halat League.jpg' WHERE Name = 'Golden Halat League';
                UPDATE Tournaments SET ImageFileName = 'Silverhand Tournament.jpg' WHERE Name = 'Silverhand Tournament';
                UPDATE Tournaments SET ImageFileName = 'Unmatched 1st Tournament.jpg' WHERE Name = 'Unmatched 1st Tournament';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TournamentParticipants");

            migrationBuilder.DropTable(
                name: "TournamentTitles");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "ImageFileName",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "MaxParticipants",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "TrophyImageFileName",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "Round",
                table: "Matches");

            migrationBuilder.RenameColumn(
                name: "Format",
                table: "Tournaments",
                newName: "Type");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Tournaments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
