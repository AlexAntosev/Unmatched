using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.MatchService.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLastBreathDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Titles SET Comment = 'Win with 1 HP.' WHERE RuleKey = 'last-breath'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Titles SET Comment = 'Win with 2 HP or less.' WHERE RuleKey = 'last-breath'");
        }
    }
}
