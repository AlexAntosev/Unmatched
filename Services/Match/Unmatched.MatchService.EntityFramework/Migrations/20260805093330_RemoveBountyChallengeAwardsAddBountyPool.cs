using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.MatchService.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBountyChallengeAwardsAddBountyPool : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Retired kinds - BountyChallengeWin(11)/BountyChallengeLoss(12) are gone, replaced by a
            // single BountyPool(11) balance row per tournament. Any leftover rows (e.g. a hero who
            // defended the pool more than once under the old per-challenge model) must go BEFORE the
            // index below is rebuilt without its old exclusion filter, or they'd violate the new plain
            // unique constraint on (TournamentId, HeroId, AwardKind).
            migrationBuilder.Sql("DELETE FROM TournamentAwards WHERE AwardKind IN (11, 12);");

            migrationBuilder.DropIndex(
                name: "IX_TournamentAwards_TournamentId_HeroId_AwardKind",
                table: "TournamentAwards");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentAwards_TournamentId_HeroId_AwardKind",
                table: "TournamentAwards",
                columns: new[] { "TournamentId", "HeroId", "AwardKind" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TournamentAwards_TournamentId_HeroId_AwardKind",
                table: "TournamentAwards");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentAwards_TournamentId_HeroId_AwardKind",
                table: "TournamentAwards",
                columns: new[] { "TournamentId", "HeroId", "AwardKind" },
                unique: true,
                filter: "([AwardKind] <> 11)");
        }
    }
}
