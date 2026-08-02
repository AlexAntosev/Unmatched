using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.StatisticsService.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddVillainStatsHpScaling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // VillainStats is a read model copied from the Catalog service (see
            // MatchCreatedVillainHandler / VillainStatsCoordinator) - it self-heals on the next sync
            // regardless, but the existing value becomes the base (1-player) case here too, for the
            // same reason as the Catalog migration: an untouched "N per player" villain needs no
            // further correction since HpPerExtraPlayer defaults to the same value.
            migrationBuilder.RenameColumn(
                name: "Hp",
                table: "VillainStats",
                newName: "BaseHp");

            migrationBuilder.AddColumn<int>(
                name: "HpPerExtraPlayer",
                table: "VillainStats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE VillainStats SET HpPerExtraPlayer = BaseHp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HpPerExtraPlayer",
                table: "VillainStats");

            migrationBuilder.RenameColumn(
                name: "BaseHp",
                table: "VillainStats",
                newName: "Hp");
        }
    }
}
