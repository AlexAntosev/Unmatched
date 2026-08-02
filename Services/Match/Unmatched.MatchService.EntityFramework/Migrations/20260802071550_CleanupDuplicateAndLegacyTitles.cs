using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.MatchService.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class CleanupDuplicateAndLegacyTitles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Titles SET Comment = 'Longest win streak.' WHERE Id = 'F13FBDFD-9558-44DA-56BB-08DC0AD0E5DF'");

            // HeroTitle rows for these titles cascade-delete (FK_HeroTitle_Titles_TitlesId is ON DELETE
            // CASCADE) - this also removes Golden Bat/Raptors/Bullseye's earned Rusher records and
            // disables the Rusher/Punisher rule-backed achievements going forward (TitleEvaluator skips a
            // rule whose Title row is gone, same as an unseeded rule).
            migrationBuilder.Sql(@"
                DELETE FROM Titles WHERE Id IN (
                    '4B672F95-7099-4424-7640-08DC16C5842D',
                    '8A741858-865A-47B0-9440-08DC16CBFFE6',
                    '23248028-51AB-4698-249D-08DC6F603CD3',
                    '882515EE-569D-4780-020E-08DC086DFF6C'
                )");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Titles SET Comment = '(9 wins in a row)' WHERE Id = 'F13FBDFD-9558-44DA-56BB-08DC0AD0E5DF'");

            migrationBuilder.Sql(@"
                INSERT INTO Titles (Id, Name, Comment, Exclusivity, RuleKey, TournamentId, Kind) VALUES
                ('4B672F95-7099-4424-7640-08DC16C5842D', 'Rusher', '(win the game with at least 2/3 of deck left)', 0, 'rusher', NULL, NULL),
                ('8A741858-865A-47B0-9440-08DC16CBFFE6', 'Punisher', '(win the game with more than 1000 points)', 0, 'punisher', NULL, NULL),
                ('23248028-51AB-4698-249D-08DC6F603CD3', 'Silverhand Champion', '', 0, NULL, NULL, NULL),
                ('882515EE-569D-4780-020E-08DC086DFF6C', 'Unmatched 1st Tournament Champion', '', 0, NULL, NULL, NULL)");

            migrationBuilder.Sql(@"
                INSERT INTO HeroTitle (TitlesId, HeroesId, EarnedAt, TimesEarned, Metric) VALUES
                ('4B672F95-7099-4424-7640-08DC16C5842D', '50CB7410-F1E3-42A8-8DD1-0CEEE7E8E655', NULL, 1, NULL),
                ('4B672F95-7099-4424-7640-08DC16C5842D', '34830402-4366-4361-AD07-0E8AE6EE220B', NULL, 1, NULL),
                ('4B672F95-7099-4424-7640-08DC16C5842D', 'D4BEDE01-BFC5-4806-813F-B5E6400AD52B', NULL, 1, NULL),
                ('23248028-51AB-4698-249D-08DC6F603CD3', 'BAE9DF44-B7FF-4E02-AD9D-6B55F666FEFE', NULL, 1, NULL),
                ('882515EE-569D-4780-020E-08DC086DFF6C', 'CFA04BE2-F89F-4EF8-B809-CA3A1646DD03', NULL, 1, NULL)");
        }
    }
}
