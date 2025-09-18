using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.StatisticsService.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class MapStatsDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastMatchIncludedAt",
                table: "MapStats",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "MapStats",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastMatchIncludedAt",
                table: "MapStats");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "MapStats");
        }
    }
}
