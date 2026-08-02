using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.CatalogService.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddVillainHpScaling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Co-op HP was stored as a single "Hp" number, but it doesn't scale the same way for every
            // villain - some are simply "N per player" (Base == PerExtra), others (Shredder, Krang) add
            // less per player than their base. The existing value becomes the base (1-player) case;
            // HpPerExtraPlayer defaults to the same value below, which is the correct scaling for every
            // "per player" villain without a single special case in code.
            migrationBuilder.RenameColumn(
                name: "Hp",
                table: "Villains",
                newName: "BaseHp");

            migrationBuilder.AddColumn<int>(
                name: "HpPerExtraPlayer",
                table: "Villains",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE Villains SET HpPerExtraPlayer = BaseHp");

            // Shredder and Krang were entered at their 2-player value (21); their real scaling is
            // 14 for 1 player, +7 per additional player (14 / 21 / 28 / 35).
            migrationBuilder.Sql(
                "UPDATE Villains SET BaseHp = 14, HpPerExtraPlayer = 7 WHERE Name IN ('Shredder', 'Krang')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HpPerExtraPlayer",
                table: "Villains");

            migrationBuilder.RenameColumn(
                name: "BaseHp",
                table: "Villains",
                newName: "Hp");

            // Note: Shredder/Krang's BaseHp was corrected to their 1-player value in Up() - Down()
            // does not attempt to restore the original 2-player-value data.
        }
    }
}
