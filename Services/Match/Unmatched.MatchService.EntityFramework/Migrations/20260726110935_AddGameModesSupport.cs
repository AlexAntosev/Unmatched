using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.MatchService.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddGameModesSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameMode",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Placement",
                table: "Fighters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Team",
                table: "Fighters",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MatchVillains",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VillainId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HpLeft = table.Column<int>(type: "int", nullable: true),
                    CardsLeft = table.Column<int>(type: "int", nullable: true),
                    IsWinner = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchVillains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchVillains_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchMinions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MatchVillainId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MinionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HpLeft = table.Column<int>(type: "int", nullable: true),
                    IsWinner = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchMinions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchMinions_MatchVillains_MatchVillainId",
                        column: x => x.MatchVillainId,
                        principalTable: "MatchVillains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MatchMinions_MatchVillainId",
                table: "MatchMinions",
                column: "MatchVillainId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchVillains_MatchId",
                table: "MatchVillains",
                column: "MatchId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchMinions");

            migrationBuilder.DropTable(
                name: "MatchVillains");

            migrationBuilder.DropColumn(
                name: "GameMode",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "Placement",
                table: "Fighters");

            migrationBuilder.DropColumn(
                name: "Team",
                table: "Fighters");
        }
    }
}
