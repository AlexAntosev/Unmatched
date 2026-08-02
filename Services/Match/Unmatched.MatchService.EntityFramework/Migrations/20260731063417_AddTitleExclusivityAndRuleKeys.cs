using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.MatchService.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddTitleExclusivityAndRuleKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Exclusivity",
                table: "Titles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RuleKey",
                table: "Titles",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TournamentId",
                table: "Titles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EarnedAt",
                table: "HeroTitle",
                type: "datetime2",
                nullable: true);

            // the three titles that already existed become rule-backed instead of manually maintained.
            migrationBuilder.Sql("UPDATE Titles SET RuleKey = 'streak', Exclusivity = 1 WHERE Name = 'The Streak'");
            migrationBuilder.Sql("UPDATE Titles SET RuleKey = 'rusher', Exclusivity = 0 WHERE Name = 'Rusher'");
            migrationBuilder.Sql("UPDATE Titles SET RuleKey = 'punisher', Exclusivity = 0 WHERE Name = 'Punisher'");

            // every other automatic title in the catalogue - see Domain/Constants/Titles.cs for the
            // matching RuleKey constants and Domain/Titles/Rules for what each one evaluates. Wrapped in
            // a single guard (checking one row stands for all eleven, since they're only ever inserted
            // together) rather than left as a bare INSERT - re-running this unguarded on a retried
            // migration would create duplicate title rows instead of failing loudly.
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM Titles WHERE RuleKey = 'flawless')
                BEGIN
                    INSERT INTO Titles (Id, Name, Comment, Exclusivity, RuleKey, TournamentId) VALUES
                    ('d567bb8d-d6a4-4763-82ec-a126e56c01fc', 'Flawless', 'Win without losing a single point of HP.', 0, 'flawless', NULL),
                    ('b823344b-f2ef-490d-b5b0-c360369cfe57', 'Last Breath', 'Win with 2 HP or less.', 0, 'last-breath', NULL),
                    ('19696ef4-2ddd-4d5d-a8b9-44320f53e017', 'Giant Slayer', 'Beat an opponent rated 300+ points higher.', 0, 'giant-slayer', NULL),
                    ('3371307d-e821-4a4b-80e9-ed6e5e696783', 'Deck Miller', 'Win with zero cards left.', 0, 'deck-miller', NULL),
                    ('8445440b-9744-4a3d-862e-99287ff0a189', 'The Sufferer', 'Longest loss streak.', 1, 'sufferer', NULL),
                    ('9b033a58-185b-4178-b3d7-53c98fbac64d', 'Grand Champion', 'Holds the #1 hero rating.', 1, 'grand-champion', NULL),
                    ('69a5e134-bf41-45dc-876a-a03f011d6b13', 'Kingslayer', 'Last hero to beat the Grand Champion.', 1, 'kingslayer', NULL),
                    ('728c2cea-522c-404f-a1bb-cd1c7141eb6f', 'The Executioner', 'Most sidekick HP destroyed, career-wide.', 1, 'executioner', NULL),
                    ('ab0228f3-10b7-4ad9-9e26-28750c4a8140', 'The Wall', 'Least HP lost per win.', 1, 'wall', NULL),
                    ('2a618dcd-e8a0-4b1f-b1ee-e2c81d1ded42', 'The Workhorse', 'Most ranked matches played.', 1, 'workhorse', NULL),
                    ('c4234fbb-5747-4d5f-b3a0-3aa18ce97ba4', 'Bounty Holder', 'Holds the Bounty pool.', 1, 'bounty-holder', NULL);
                END");

            // guarded rather than a plain CreateIndex - see AddMatchIsRanked for why.
            migrationBuilder.Sql(
                """
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Titles_RuleKey' AND object_id = OBJECT_ID(N'[Titles]'))
                BEGIN
                    CREATE UNIQUE INDEX [IX_Titles_RuleKey] ON [Titles] ([RuleKey]) WHERE [RuleKey] IS NOT NULL;
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM Titles WHERE RuleKey IN (
                    'flawless', 'last-breath', 'giant-slayer', 'deck-miller',
                    'sufferer', 'grand-champion', 'kingslayer', 'executioner', 'wall', 'workhorse', 'bounty-holder')");

            migrationBuilder.Sql("DROP INDEX IF EXISTS [IX_Titles_RuleKey] ON [Titles];");

            migrationBuilder.DropColumn(
                name: "Exclusivity",
                table: "Titles");

            migrationBuilder.DropColumn(
                name: "RuleKey",
                table: "Titles");

            migrationBuilder.DropColumn(
                name: "TournamentId",
                table: "Titles");

            migrationBuilder.DropColumn(
                name: "EarnedAt",
                table: "HeroTitle");
        }
    }
}
