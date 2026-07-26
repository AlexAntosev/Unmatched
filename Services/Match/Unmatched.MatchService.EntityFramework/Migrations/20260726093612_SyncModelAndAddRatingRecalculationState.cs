using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.MatchService.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelAndAddRatingRecalculationState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // The previous model snapshot had drifted badly out of sync with the actual database (stale
            // pre-microservice-split entity names, a phantom int-keyed "Title" table). Everything except the
            // new table below was already true of the real schema - see the accompanying PR/commit notes -
            // so only the genuinely new change is applied here.
            migrationBuilder.CreateTable(
                name: "RatingRecalculationState",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsRecalculationRequired = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatingRecalculationState", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RatingRecalculationState");
        }
    }
}
