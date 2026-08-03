using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.MatchService.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddTournamentFinalFormatAndBounty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TournamentAwards_TournamentId_HeroId_AwardKind",
                table: "TournamentAwards");

            // Existing SingleElimination tournaments' grand finals were always generated as three
            // identical stand-in pairings before this migration (see SingleEliminationGenerator) -
            // defaulting them to Bo3 preserves that behavior. New tournaments always send FinalFormat
            // explicitly at creation.
            migrationBuilder.AddColumn<int>(
                name: "FinalFormat",
                table: "Tournaments",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "Reward",
                table: "Tournaments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StartingChampionId",
                table: "Tournaments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TournamentAwards_TournamentId_HeroId_AwardKind",
                table: "TournamentAwards",
                columns: new[] { "TournamentId", "HeroId", "AwardKind" },
                unique: true,
                filter: "([AwardKind] <> 11)");

            // BountyHolderTitleRule is gone - Bounty now gets a live, per-tournament "Bounty {Name}
            // holder" title (see BountyChallengeResolver) instead of one global, always-vacant title.
            migrationBuilder.Sql(
                """
                DELETE FROM HeroTitle WHERE TitlesId IN (SELECT Id FROM Titles WHERE RuleKey = 'bounty-holder');
                DELETE FROM Titles WHERE RuleKey = 'bounty-holder';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF NOT EXISTS (SELECT 1 FROM Titles WHERE RuleKey = 'bounty-holder')
                BEGIN
                    INSERT INTO Titles (Id, Name, Comment, Exclusivity, RuleKey, TournamentId) VALUES
                    ('c4234fbb-5747-4d5f-b3a0-3aa18ce97ba4', 'Bounty Holder', 'Holds the Bounty pool.', 1, 'bounty-holder', NULL);
                END
                """);

            migrationBuilder.DropIndex(
                name: "IX_TournamentAwards_TournamentId_HeroId_AwardKind",
                table: "TournamentAwards");

            migrationBuilder.DropColumn(
                name: "FinalFormat",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "Reward",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "StartingChampionId",
                table: "Tournaments");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentAwards_TournamentId_HeroId_AwardKind",
                table: "TournamentAwards",
                columns: new[] { "TournamentId", "HeroId", "AwardKind" },
                unique: true);
        }
    }
}
