using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unmatched.StatisticsService.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddImageFileNameToStats : Migration
    {
        /// <inheritdoc />
        /// <remarks>
        /// Guarded raw SQL (COL_LENGTH check) for the same reason as Catalog's AddExpansionEntity/
        /// AddImageFileName migrations - safe whether EnsureCreated() or Migrate() created the column.
        /// Backfills existing rows in the same migration, matched by HeroId/MapId to the same
        /// ImageFileName values assigned in Catalog's BackfillImageFileNames migration (see
        /// Services/Catalog/.../Migrations/Scripts/BackfillImageFileNames.sql for the source mapping).
        /// New heroes/maps discovered going forward get ImageFileName from Catalog automatically via
        /// HeroStatsCoordinator/MapStatsCoordinator (DomainMapper already maps it by convention).
        /// </remarks>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('HeroStats', 'ImageFileName') IS NULL ALTER TABLE HeroStats ADD ImageFileName NVARCHAR(MAX) NULL;
IF COL_LENGTH('MapStats', 'ImageFileName') IS NULL ALTER TABLE MapStats ADD ImageFileName NVARCHAR(MAX) NULL;
");

            migrationBuilder.Sql(@"
UPDATE HeroStats SET ImageFileName = N'achilles.png' WHERE HeroId = '071C861A-2E1F-4C0E-997A-E610DB6039DC' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'alice.png' WHERE HeroId = '4E1C5B41-C6FE-4AF6-9FCD-21F271DB2B22' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'ancient-leshen.png' WHERE HeroId = 'CE4CACE0-455E-4731-B559-2401257A6A0A' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'annie-christmas.png' WHERE HeroId = '8419E9B3-C803-40F4-A9E9-77D1738178C9' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'beowulf.png' WHERE HeroId = 'E43B6A80-D4FD-41D6-8BC5-E1095C1CBF54' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'bigfoot.png' WHERE HeroId = 'CD38DDD8-3676-4E94-A50E-F6E328898D44' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'bloody-mary.png' WHERE HeroId = '1770DE9F-80DF-4385-A628-39C65AA8F57C' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'bullseye.png' WHERE HeroId = 'D4BEDE01-BFC5-4806-813F-B5E6400AD52B' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'ciri.png' WHERE HeroId = '76344F47-1C89-463E-ACFF-AFC699D100B8' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'daredevil.png' WHERE HeroId = '3D6CAAA2-1D93-47A4-A6C4-6B2662FCA853' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'dr-jill-trent.png' WHERE HeroId = 'FEBE8639-C4DB-441D-B7A1-ED23C5B00E61' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'dr-sattler.png' WHERE HeroId = '02D2EE1D-F7A1-4509-B3A4-C24C2CE207E4' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'dracula.png' WHERE HeroId = '07457CD8-3D79-4DAC-8DDF-584FBDF5AA81' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'elektra.png' WHERE HeroId = '4B4C9086-EE72-4AC8-BCF2-9DCA2FFF3F0A' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'eredin.png' WHERE HeroId = '9CD55219-BA87-41FF-9854-5CCFA257391C' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'geralt-of-rivia.png' WHERE HeroId = '10BBA81E-F305-4428-82EA-75EE01C51EA2' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'ghost-rider.png' WHERE HeroId = '1D0CAD01-0D35-4308-A662-CEA71672D11E' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'golden-bat.png' WHERE HeroId = '50CB7410-F1E3-42A8-8DD1-0CEEE7E8E655' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'houdini.png' WHERE HeroId = '916BAB1D-35D1-4B8D-82C0-8A3714E52A80' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'invisible-man.png' WHERE HeroId = '10B753BA-D8AA-4C47-8115-AB8FE7C90AD9' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'jekyll-hyde.png' WHERE HeroId = '1D4D861F-BC96-4F4D-9BA7-C97C2465E7E8' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'king-arthur.png' WHERE HeroId = 'B9201C43-BA8D-4527-9CD7-1BAE8C1BA200' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'little-red.png' WHERE HeroId = '5491096D-4FB6-49E8-818E-47B1DDED8F82' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'luke-cage.png' WHERE HeroId = 'A5C15E8F-4E98-44DA-B3F2-5D14D51FC0F9' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'medusa.png' WHERE HeroId = '4DDFD945-70C7-4313-9326-820DA0CDA6CD' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'moon-knight.png' WHERE HeroId = 'FB56747F-3E50-4945-B772-A82E84E7A593' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'nikola-tesla.png' WHERE HeroId = 'F4EAA298-1DD3-4200-B78E-402100990408' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'oda-nobunaga.png' WHERE HeroId = '60E64502-91CB-41C7-8B53-82CCF79AF210' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'philippa.png' WHERE HeroId = 'EFC67BED-4986-4AFF-84DB-A43A4085DFC4' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'raptors.png' WHERE HeroId = '34830402-4366-4361-AD07-0E8AE6EE220B' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'robert-muldoon.png' WHERE HeroId = '92391E95-D634-4B65-8872-669717F54623' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'robin-hood.png' WHERE HeroId = 'BA8DBE79-D8E3-4C3F-8FC5-D0B0D9F08360' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'sherlock-holmes.png' WHERE HeroId = '249543AE-06F0-47BB-B301-F1F312A622DB' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'sinbad.png' WHERE HeroId = 'E84E2C68-FAA0-41FD-AAE5-49A97CBC46B6' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'sun-wukong.png' WHERE HeroId = 'CFA04BE2-F89F-4EF8-B809-CA3A1646DD03' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'the-genie.png' WHERE HeroId = 'BAE9DF44-B7FF-4E02-AD9D-6B55F666FEFE' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'tomoe-gozen.png' WHERE HeroId = '52B34CCC-BF52-489F-A444-94A4160787F7' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N't-rex.png' WHERE HeroId = '9410D4AD-50CA-494D-A702-52EA8EDDA357' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'yennefer-triss.png' WHERE HeroId = 'EDCF90BE-E952-4208-B393-8809A32EED65' AND ImageFileName IS NULL;
UPDATE HeroStats SET ImageFileName = N'yennenga.png' WHERE HeroId = '1FFFD192-23E7-494D-8ABA-9FB756B03B35' AND ImageFileName IS NULL;

UPDATE MapStats SET ImageFileName = N'azuchi-castle.png' WHERE MapId = 'FB2222F9-1450-4D02-9F92-19D2597EC2A6' AND ImageFileName IS NULL;
UPDATE MapStats SET ImageFileName = N'castle.png' WHERE MapId = '4459D5C0-3E8F-43D5-AA15-08DBA3E73224' AND ImageFileName IS NULL;
UPDATE MapStats SET ImageFileName = N'fayrlund-forest.png' WHERE MapId = '9DAEDF18-23BC-42AC-BE80-88023C7B4432' AND ImageFileName IS NULL;
UPDATE MapStats SET ImageFileName = N'golden-forest.png' WHERE MapId = '175A1C6C-3BF5-41BA-AA18-08DBA3E73224' AND ImageFileName IS NULL;
UPDATE MapStats SET ImageFileName = N'green-forest.png' WHERE MapId = '692EA7AD-0B64-4B55-AA17-08DBA3E73224' AND ImageFileName IS NULL;
UPDATE MapStats SET ImageFileName = N'hells-kitchen.png' WHERE MapId = 'B1D4D228-0D31-4E9D-AA1D-08DBA3E73224' AND ImageFileName IS NULL;
UPDATE MapStats SET ImageFileName = N'kaer-morhen.png' WHERE MapId = '2DD2D958-2FDC-40D5-A745-8FAF21477558' AND ImageFileName IS NULL;
UPDATE MapStats SET ImageFileName = N'king-solomons-mine.png' WHERE MapId = '2528087B-C131-4973-526E-08DBC5BAEB17' AND ImageFileName IS NULL;
UPDATE MapStats SET ImageFileName = N'laboratory.png' WHERE MapId = 'A75396E4-CA7A-4104-AA1C-08DBA3E73224' AND ImageFileName IS NULL;
UPDATE MapStats SET ImageFileName = N'london.png' WHERE MapId = 'DF84DE25-775B-4EA0-AA19-08DBA3E73224' AND ImageFileName IS NULL;
UPDATE MapStats SET ImageFileName = N'mansion.png' WHERE MapId = '35796F95-D025-47A0-AA1A-08DBA3E73224' AND ImageFileName IS NULL;
UPDATE MapStats SET ImageFileName = N'mcminnville.png' WHERE MapId = '267252BD-99A2-4118-DA02-08DC15191DE2' AND ImageFileName IS NULL;
UPDATE MapStats SET ImageFileName = N'naglfar.png' WHERE MapId = '44D5229F-D311-43A3-8F41-6FC512517E52' AND ImageFileName IS NULL;
UPDATE MapStats SET ImageFileName = N'point-pleasant.png' WHERE MapId = 'E1C8229C-A783-4B3F-DA03-08DC15191DE2' AND ImageFileName IS NULL;
UPDATE MapStats SET ImageFileName = N'raptor-paddock.png' WHERE MapId = '2AC482BE-3EB0-4DAD-AA1F-08DBA3E73224' AND ImageFileName IS NULL;
UPDATE MapStats SET ImageFileName = N'ruins.png' WHERE MapId = '65E46A0F-7EFC-4BDA-AA1E-08DBA3E73224' AND ImageFileName IS NULL;
UPDATE MapStats SET ImageFileName = N'ship.png' WHERE MapId = '566E853A-B84E-4902-AA16-08DBA3E73224' AND ImageFileName IS NULL;
UPDATE MapStats SET ImageFileName = N'streets-of-novigrad.png' WHERE MapId = 'FB1D3C51-5407-4CFD-809C-8640BD998F0F' AND ImageFileName IS NULL;
UPDATE MapStats SET ImageFileName = N't-rex-paddock.png' WHERE MapId = 'CB1FB717-7DBA-4AE1-AA20-08DBA3E73224' AND ImageFileName IS NULL;
UPDATE MapStats SET ImageFileName = N'tavern.png' WHERE MapId = 'DCF4A9F9-B6C5-45C9-AA1B-08DBA3E73224' AND ImageFileName IS NULL;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('MapStats', 'ImageFileName') IS NOT NULL ALTER TABLE MapStats DROP COLUMN ImageFileName;
IF COL_LENGTH('HeroStats', 'ImageFileName') IS NOT NULL ALTER TABLE HeroStats DROP COLUMN ImageFileName;
");
        }
    }
}
