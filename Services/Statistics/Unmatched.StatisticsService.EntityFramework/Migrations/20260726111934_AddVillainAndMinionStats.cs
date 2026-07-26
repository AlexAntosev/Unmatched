using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.StatisticsService.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddVillainAndMinionStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MinionStats",
                columns: table => new
                {
                    MinionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hp = table.Column<int>(type: "int", nullable: false),
                    DeckSize = table.Column<int>(type: "int", nullable: false),
                    IsRanged = table.Column<bool>(type: "bit", nullable: false),
                    ImageFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalLooses = table.Column<int>(type: "int", nullable: false),
                    TotalMatches = table.Column<int>(type: "int", nullable: false),
                    TotalWins = table.Column<int>(type: "int", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastMatchIncludedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinionStats", x => x.MinionId);
                });

            migrationBuilder.CreateTable(
                name: "VillainStats",
                columns: table => new
                {
                    VillainId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hp = table.Column<int>(type: "int", nullable: false),
                    DeckSize = table.Column<int>(type: "int", nullable: false),
                    IsRanged = table.Column<bool>(type: "bit", nullable: false),
                    ImageFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalLooses = table.Column<int>(type: "int", nullable: false),
                    TotalMatches = table.Column<int>(type: "int", nullable: false),
                    TotalWins = table.Column<int>(type: "int", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastMatchIncludedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VillainStats", x => x.VillainId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MinionStats");

            migrationBuilder.DropTable(
                name: "VillainStats");
        }
    }
}
