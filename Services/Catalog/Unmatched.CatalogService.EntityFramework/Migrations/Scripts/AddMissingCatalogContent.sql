USE [Unmatched];
GO

-- Populates Heroes/Sidekicks/Villains/Minions/Maps for the expansions that were still metadata-only,
-- per the reviewed document and the user's follow-up corrections (see PR discussion). All INSERTs are
-- guarded by Name-existence checks, matching the idempotent pattern used throughout this catalog's
-- migrations (e.g. AddSunsOriginData.sql, FixAndExpandExpansions.sql).
--
-- Data-modeling decisions applied here (confirmed with user):
--  - Multi-miniature "single hero" units (Wayward Sisters, Cloak & Dagger) store SUMMED Hp across all
--    their minis, as one Hero row.
--  - Co-op Villains whose Hp scales with player count (Shredder/Krang) store the 2-player value for now.
--  - Swappable hero/sidekick identity pairs (Yennefer & Triss) become TWO Hero rows, one per direction,
--    each with the other as its Sidekick - mirroring the existing "Yennefer & Triss" row (renamed to
--    "Triss" here) with a new "Yennefer" row added.
--  - Bebop & Rocksteady have no confirmed hero/sidekick swap mechanic (verified via research), so they
--    stay a single combined Minion/Sidekick entry, not split into a pair.
--  - Any DeckSize that couldn't be verified from a real source is set to 30 (the standard Unmatched deck
--    size almost every verified hero in the game uses).

-- ============================================================
-- 0a) Rename TMNT expansion to its real product name
-- ============================================================
UPDATE Expansions SET Name = N'Unmatched Adventures: Teenage Mutant Ninja Turtles' WHERE Name = N'Unmatched: Teenage Mutant Ninja Turtles';
GO

-- ============================================================
-- 0b) Remove the two standalone single-hero boxes the user asked to skip for now
-- ============================================================
DELETE FROM Expansions
WHERE Name IN (N'Unmatched: Bruce Lee', N'Unmatched: Deadpool')
  AND NOT EXISTS (SELECT 1 FROM Heroes WHERE ExpansionId = Expansions.Id)
  AND NOT EXISTS (SELECT 1 FROM Maps WHERE ExpansionId = Expansions.Id)
  AND NOT EXISTS (SELECT 1 FROM Villains WHERE ExpansionId = Expansions.Id)
  AND NOT EXISTS (SELECT 1 FROM Minions WHERE ExpansionId = Expansions.Id)
  AND NOT EXISTS (SELECT 1 FROM OwnedExpansions WHERE ExpansionId = Expansions.Id);
GO

-- ============================================================
-- 0c) New expansion: TMNT - Shredder and Krang hero deck box (real, separate product, BGG 429397)
-- ============================================================
DECLARE @ExpansionId UNIQUEIDENTIFIER;
SELECT @ExpansionId = Id FROM Expansions WHERE Name = N'Unmatched Adventures: Teenage Mutant Ninja Turtles - Shredder and Krang';
IF @ExpansionId IS NULL
BEGIN
    SET @ExpansionId = NEWID();
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher)
    VALUES (@ExpansionId, N'Unmatched Adventures: Teenage Mutant Ninja Turtles - Shredder and Krang', NULL, N'Restoration Games');
END
GO

-- ============================================================
-- 0d) Rename Dr. Sattler's sidekick to its official full name
-- ============================================================
UPDATE Sidekicks SET Name = N'Dr. Ian Malcolm' WHERE Name = N'Dr. Malcolm';
GO

-- ============================================================
-- 0e) Split "Yennefer & Triss" into two hero/sidekick pairs
-- ============================================================
UPDATE Heroes SET Name = N'Triss', ImageFileName = N'triss.png' WHERE Name = N'Yennefer & Triss';
GO

UPDATE Sidekicks SET Name = N'Yennefer'
WHERE HeroId = (SELECT Id FROM Heroes WHERE Name = N'Triss') AND Name = N'Triss & Yennefer';
GO

DECLARE @RealmsFallId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched: The Witcher - Realms Fall');
DECLARE @TrissHp INT = (SELECT Hp FROM Heroes WHERE Name = N'Triss');
DECLARE @TrissDeckSize INT = (SELECT DeckSize FROM Heroes WHERE Name = N'Triss');
DECLARE @YennId UNIQUEIDENTIFIER;
SELECT @YennId = Id FROM Heroes WHERE Name = N'Yennefer';
IF @YennId IS NULL
BEGIN
    SET @YennId = NEWID();
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (@YennId, @TrissDeckSize, @TrissHp, 1, N'Yennefer', N'#8E44AD', @RealmsFallId, N'yennefer.png');
END
IF NOT EXISTS (SELECT 1 FROM Sidekicks WHERE HeroId = @YennId AND Name = N'Triss')
    INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name) VALUES (NEWID(), 1, @YennId, 6, 1, N'Triss');
GO

-- ============================================================
-- 1) Unmatched: Battle of Legends, Volume Three
-- ============================================================
DECLARE @ExpansionId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched: Battle of Legends, Volume Three');

DECLARE @BlackbeardId UNIQUEIDENTIFIER;
SELECT @BlackbeardId = Id FROM Heroes WHERE Name = N'Blackbeard';
IF @BlackbeardId IS NULL
BEGIN
    SET @BlackbeardId = NEWID();
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (@BlackbeardId, 30, 13, 1, N'Blackbeard', N'#2C1B12', @ExpansionId, N'blackbeard.png');
    INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name) VALUES (NEWID(), 2, @BlackbeardId, 1, 0, N'Seadog');
END

DECLARE @ChupacabraId UNIQUEIDENTIFIER;
SELECT @ChupacabraId = Id FROM Heroes WHERE Name = N'Chupacabra';
IF @ChupacabraId IS NULL
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (NEWID(), 30, 14, 0, N'Chupacabra', N'#4B5320', @ExpansionId, N'chupacabra.png');

DECLARE @PandoraId UNIQUEIDENTIFIER;
SELECT @PandoraId = Id FROM Heroes WHERE Name = N'Pandora';
IF @PandoraId IS NULL
BEGIN
    SET @PandoraId = NEWID();
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (@PandoraId, 30, 14, 0, N'Pandora', N'#6A0DAD', @ExpansionId, N'pandora.png');
    INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name) VALUES (NEWID(), 2, @PandoraId, 1, 0, N'Kakodaemon');
END

IF NOT EXISTS (SELECT 1 FROM Heroes WHERE Name = N'Loki')
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (NEWID(), 30, 16, 0, N'Loki', N'#2E8B57', @ExpansionId, N'loki.png');

IF NOT EXISTS (SELECT 1 FROM Maps WHERE Name = N'Venice')
    INSERT INTO Maps (Id, Name, OfficialName, ExpansionId) VALUES (NEWID(), N'Venice', N'Venice', @ExpansionId);
IF NOT EXISTS (SELECT 1 FROM Maps WHERE Name = N'Santa''s Workshop')
    INSERT INTO Maps (Id, Name, OfficialName, ExpansionId) VALUES (NEWID(), N'Santa''s Workshop', N'Santa''s Workshop', @ExpansionId);
GO

-- ============================================================
-- 2) Unmatched: Buffy the Vampire Slayer
-- ============================================================
DECLARE @ExpansionId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched: Buffy the Vampire Slayer');

DECLARE @BuffyId UNIQUEIDENTIFIER;
SELECT @BuffyId = Id FROM Heroes WHERE Name = N'Buffy';
IF @BuffyId IS NULL
BEGIN
    SET @BuffyId = NEWID();
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (@BuffyId, 35, 14, 0, N'Buffy', N'#C9184A', @ExpansionId, N'buffy.png');
    -- Buffy picks one of these two as her sidekick before a match; both recorded as available options.
    INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name) VALUES (NEWID(), 1, @BuffyId, 6, 0, N'Giles');
    INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name) VALUES (NEWID(), 1, @BuffyId, 6, 0, N'Xander');
END

DECLARE @WillowId UNIQUEIDENTIFIER;
SELECT @WillowId = Id FROM Heroes WHERE Name = N'Willow';
IF @WillowId IS NULL
BEGIN
    SET @WillowId = NEWID();
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (@WillowId, 30, 14, 1, N'Willow', N'#4C9A2A', @ExpansionId, N'willow.png');
    INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name) VALUES (NEWID(), 1, @WillowId, 6, 1, N'Tara');
END

DECLARE @AngelId UNIQUEIDENTIFIER;
SELECT @AngelId = Id FROM Heroes WHERE Name = N'Angel';
IF @AngelId IS NULL
BEGIN
    SET @AngelId = NEWID();
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (@AngelId, 30, 16, 0, N'Angel', N'#1B1B2F', @ExpansionId, N'angel.png');
    INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name) VALUES (NEWID(), 1, @AngelId, 8, 0, N'Faith');
END

DECLARE @SpikeId UNIQUEIDENTIFIER;
SELECT @SpikeId = Id FROM Heroes WHERE Name = N'Spike';
IF @SpikeId IS NULL
BEGIN
    SET @SpikeId = NEWID();
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (@SpikeId, 30, 15, 0, N'Spike', N'#8B0000', @ExpansionId, N'spike.png');
    INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name) VALUES (NEWID(), 1, @SpikeId, 7, 0, N'Drusilla');
END

IF NOT EXISTS (SELECT 1 FROM Maps WHERE Name = N'Sunnydale High')
    INSERT INTO Maps (Id, Name, OfficialName, ExpansionId) VALUES (NEWID(), N'Sunnydale High', N'Sunnydale High', @ExpansionId);
IF NOT EXISTS (SELECT 1 FROM Maps WHERE Name = N'The Bronze')
    INSERT INTO Maps (Id, Name, OfficialName, ExpansionId) VALUES (NEWID(), N'The Bronze', N'The Bronze', @ExpansionId);
GO

-- ============================================================
-- 3) Unmatched: Slings & Arrows
-- ============================================================
DECLARE @ExpansionId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched: Slings & Arrows');

DECLARE @ShakespeareId UNIQUEIDENTIFIER;
SELECT @ShakespeareId = Id FROM Heroes WHERE Name = N'William Shakespeare';
IF @ShakespeareId IS NULL
BEGIN
    SET @ShakespeareId = NEWID();
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (@ShakespeareId, 30, 13, 0, N'William Shakespeare', N'#4B3621', @ExpansionId, N'william-shakespeare.png');
    INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name) VALUES (NEWID(), 3, @ShakespeareId, 1, 0, N'Actor');
END

DECLARE @TitaniaId UNIQUEIDENTIFIER;
SELECT @TitaniaId = Id FROM Heroes WHERE Name = N'Titania';
IF @TitaniaId IS NULL
BEGIN
    SET @TitaniaId = NEWID();
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (@TitaniaId, 30, 12, 1, N'Titania', N'#C77DFF', @ExpansionId, N'titania.png');
    INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name) VALUES (NEWID(), 1, @TitaniaId, 6, 0, N'Oberon');
END

DECLARE @HamletId UNIQUEIDENTIFIER;
SELECT @HamletId = Id FROM Heroes WHERE Name = N'Hamlet';
IF @HamletId IS NULL
BEGIN
    SET @HamletId = NEWID();
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (@HamletId, 30, 15, 0, N'Hamlet', N'#2F2F4F', @ExpansionId, N'hamlet.png');
    INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name) VALUES (NEWID(), 1, @HamletId, 6, 0, N'Rosencrantz & Guildenstern');
END

IF NOT EXISTS (SELECT 1 FROM Heroes WHERE Name = N'The Wayward Sisters')
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (NEWID(), 30, 18, 0, N'The Wayward Sisters', N'#556B2F', @ExpansionId, N'the-wayward-sisters.png');

IF NOT EXISTS (SELECT 1 FROM Maps WHERE Name = N'The Globe Theatre')
    INSERT INTO Maps (Id, Name, OfficialName, ExpansionId) VALUES (NEWID(), N'The Globe Theatre', N'The Globe Theatre', @ExpansionId);
GO

-- ============================================================
-- 4) Unmatched Adventures: Teenage Mutant Ninja Turtles (heroes, co-op villains, minions)
-- ============================================================
DECLARE @ExpansionId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched Adventures: Teenage Mutant Ninja Turtles');

DECLARE @LeonardoId UNIQUEIDENTIFIER;
SELECT @LeonardoId = Id FROM Heroes WHERE Name = N'Leonardo';
IF @LeonardoId IS NULL
BEGIN
    SET @LeonardoId = NEWID();
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (@LeonardoId, 30, 16, 0, N'Leonardo', N'#1E90FF', @ExpansionId, N'leonardo.png');
    INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name) VALUES (NEWID(), 1, @LeonardoId, 9, 0, N'Splinter');
END

DECLARE @DonatelloId UNIQUEIDENTIFIER;
SELECT @DonatelloId = Id FROM Heroes WHERE Name = N'Donatello';
IF @DonatelloId IS NULL
BEGIN
    SET @DonatelloId = NEWID();
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (@DonatelloId, 30, 14, 0, N'Donatello', N'#8A2BE2', @ExpansionId, N'donatello.png');
    INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name) VALUES (NEWID(), 1, @DonatelloId, 7, 0, N'Metalhead');
END

DECLARE @MichelangeloId UNIQUEIDENTIFIER;
SELECT @MichelangeloId = Id FROM Heroes WHERE Name = N'Michelangelo';
IF @MichelangeloId IS NULL
BEGIN
    SET @MichelangeloId = NEWID();
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (@MichelangeloId, 30, 14, 0, N'Michelangelo', N'#FF8C00', @ExpansionId, N'michelangelo.png');
    INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name) VALUES (NEWID(), 1, @MichelangeloId, 6, 1, N'April O''Neil');
END

DECLARE @RaphaelId UNIQUEIDENTIFIER;
SELECT @RaphaelId = Id FROM Heroes WHERE Name = N'Raphael';
IF @RaphaelId IS NULL
BEGIN
    SET @RaphaelId = NEWID();
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (@RaphaelId, 30, 17, 0, N'Raphael', N'#DC143C', @ExpansionId, N'raphael.png');
    INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name) VALUES (NEWID(), 1, @RaphaelId, 8, 1, N'Casey Jones');
END

-- Co-op villains: Hp scales with player count (7 x (players+1)); storing the 2-player value (21) for now.
IF NOT EXISTS (SELECT 1 FROM Villains WHERE Name = N'Shredder')
    INSERT INTO Villains (Id, Name, Hp, DeckSize, IsRanged, Color, ExpansionId, ImageFileName)
    VALUES (NEWID(), N'Shredder', 21, 30, 0, N'#6B0F1A', @ExpansionId, N'shredder.png');
IF NOT EXISTS (SELECT 1 FROM Villains WHERE Name = N'Krang')
    INSERT INTO Villains (Id, Name, Hp, DeckSize, IsRanged, Color, ExpansionId, ImageFileName)
    VALUES (NEWID(), N'Krang', 21, 30, 0, N'#FF69B4', @ExpansionId, N'krang.png');

IF NOT EXISTS (SELECT 1 FROM Minions WHERE Name = N'Slash')
    INSERT INTO Minions (Id, Name, Hp, DeckSize, IsRanged, Color, ExpansionId, ImageFileName)
    VALUES (NEWID(), N'Slash', 10, 30, 0, N'#2F4F2F', @ExpansionId, N'slash.png');
IF NOT EXISTS (SELECT 1 FROM Minions WHERE Name = N'Rat King')
    INSERT INTO Minions (Id, Name, Hp, DeckSize, IsRanged, Color, ExpansionId, ImageFileName)
    VALUES (NEWID(), N'Rat King', 10, 30, 0, N'#4B3B2A', @ExpansionId, N'rat-king.png');
IF NOT EXISTS (SELECT 1 FROM Minions WHERE Name = N'Leatherhead')
    INSERT INTO Minions (Id, Name, Hp, DeckSize, IsRanged, Color, ExpansionId, ImageFileName)
    VALUES (NEWID(), N'Leatherhead', 10, 30, 0, N'#2F5233', @ExpansionId, N'leatherhead.png');
IF NOT EXISTS (SELECT 1 FROM Minions WHERE Name = N'Bebop & Rocksteady')
    INSERT INTO Minions (Id, Name, Hp, DeckSize, IsRanged, Color, ExpansionId, ImageFileName)
    VALUES (NEWID(), N'Bebop & Rocksteady', 10, 30, 0, N'#708090', @ExpansionId, N'bebop-and-rocksteady.png');
IF NOT EXISTS (SELECT 1 FROM Minions WHERE Name = N'Baxter Stockman')
    INSERT INTO Minions (Id, Name, Hp, DeckSize, IsRanged, Color, ExpansionId, ImageFileName)
    VALUES (NEWID(), N'Baxter Stockman', 10, 30, 0, N'#4682B4', @ExpansionId, N'baxter-stockman.png');
IF NOT EXISTS (SELECT 1 FROM Minions WHERE Name = N'Wingnut')
    INSERT INTO Minions (Id, Name, Hp, DeckSize, IsRanged, Color, ExpansionId, ImageFileName)
    VALUES (NEWID(), N'Wingnut', 10, 30, 0, N'#8B4513', @ExpansionId, N'wingnut.png');
-- No confirmed official map title found for this box (only "New York" / "Technodrome" board-side
-- descriptions) - intentionally not adding a Map row rather than inventing a name.
GO

-- ============================================================
-- 5) Unmatched Adventures: TMNT - Shredder and Krang (Shredder/Krang as competitive Heroes)
-- ============================================================
DECLARE @ExpansionId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched Adventures: Teenage Mutant Ninja Turtles - Shredder and Krang');

DECLARE @ShredderHeroId UNIQUEIDENTIFIER;
SELECT @ShredderHeroId = Id FROM Heroes WHERE Name = N'Shredder';
IF @ShredderHeroId IS NULL
BEGIN
    SET @ShredderHeroId = NEWID();
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (@ShredderHeroId, 30, 15, 0, N'Shredder', N'#6B0F1A', @ExpansionId, N'shredder.png');
    INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name) VALUES (NEWID(), 1, @ShredderHeroId, 7, 0, N'Bebop & Rocksteady');
END

IF NOT EXISTS (SELECT 1 FROM Heroes WHERE Name = N'Krang')
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (NEWID(), 30, 16, 0, N'Krang', N'#FF69B4', @ExpansionId, N'krang.png');
GO

-- ============================================================
-- 6) Unmatched: Stars and Stripes
-- ============================================================
DECLARE @ExpansionId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched: Stars and Stripes');

DECLARE @WashingtonId UNIQUEIDENTIFIER;
SELECT @WashingtonId = Id FROM Heroes WHERE Name = N'George Washington';
IF @WashingtonId IS NULL
BEGIN
    SET @WashingtonId = NEWID();
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (@WashingtonId, 30, 14, 0, N'George Washington', N'#002147', @ExpansionId, N'george-washington.png');
    INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name) VALUES (NEWID(), 3, @WashingtonId, 1, 0, N'Culper Spy');
END

DECLARE @RosieId UNIQUEIDENTIFIER;
SELECT @RosieId = Id FROM Heroes WHERE Name = N'Rosie the Riveter';
IF @RosieId IS NULL
BEGIN
    SET @RosieId = NEWID();
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (@RosieId, 30, 15, 0, N'Rosie the Riveter', N'#1F4E8C', @ExpansionId, N'rosie-the-riveter.png');
    INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name) VALUES (NEWID(), 1, @RosieId, 7, 0, N'Wendy the Welder');
END

IF NOT EXISTS (SELECT 1 FROM Heroes WHERE Name = N'John Henry')
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (NEWID(), 30, 17, 0, N'John Henry', N'#2F2F2F', @ExpansionId, N'john-henry.png');

DECLARE @EarpId UNIQUEIDENTIFIER;
SELECT @EarpId = Id FROM Heroes WHERE Name = N'Wyatt Earp';
IF @EarpId IS NULL
BEGIN
    SET @EarpId = NEWID();
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (@EarpId, 30, 15, 1, N'Wyatt Earp', N'#8B7355', @ExpansionId, N'wyatt-earp.png');
    INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name) VALUES (NEWID(), 1, @EarpId, 8, 1, N'Doc Holliday');
END

IF NOT EXISTS (SELECT 1 FROM Maps WHERE Name = N'The White House')
    INSERT INTO Maps (Id, Name, OfficialName, ExpansionId) VALUES (NEWID(), N'The White House', N'The White House', @ExpansionId);
IF NOT EXISTS (SELECT 1 FROM Maps WHERE Name = N'The Alamo')
    INSERT INTO Maps (Id, Name, OfficialName, ExpansionId) VALUES (NEWID(), N'The Alamo', N'The Alamo', @ExpansionId);
GO

-- ============================================================
-- 7) Unmatched Marvel: Teen Spirit
-- ============================================================
DECLARE @ExpansionId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched Marvel: Teen Spirit');

IF NOT EXISTS (SELECT 1 FROM Heroes WHERE Name = N'Ms. Marvel')
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (NEWID(), 30, 14, 0, N'Ms. Marvel', N'#C0392B', @ExpansionId, N'ms-marvel.png');

DECLARE @SquirrelGirlId UNIQUEIDENTIFIER;
SELECT @SquirrelGirlId = Id FROM Heroes WHERE Name = N'Squirrel Girl';
IF @SquirrelGirlId IS NULL
BEGIN
    SET @SquirrelGirlId = NEWID();
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (@SquirrelGirlId, 30, 13, 0, N'Squirrel Girl', N'#8B5A2B', @ExpansionId, N'squirrel-girl.png');
    INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name) VALUES (NEWID(), 8, @SquirrelGirlId, 1, 0, N'Squirrel');
END

IF NOT EXISTS (SELECT 1 FROM Heroes WHERE Name = N'Cloak & Dagger')
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (NEWID(), 30, 16, 0, N'Cloak & Dagger', N'#1A1A2E', @ExpansionId, N'cloak-and-dagger.png');

IF NOT EXISTS (SELECT 1 FROM Maps WHERE Name = N'Navy Pier')
    INSERT INTO Maps (Id, Name, OfficialName, ExpansionId) VALUES (NEWID(), N'Navy Pier', N'Navy Pier', @ExpansionId);
GO

-- ============================================================
-- 8) Unmatched Marvel: For King and Country
-- ============================================================
DECLARE @ExpansionId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched Marvel: For King and Country');

DECLARE @BlackWidowId UNIQUEIDENTIFIER;
SELECT @BlackWidowId = Id FROM Heroes WHERE Name = N'Black Widow';
IF @BlackWidowId IS NULL
BEGIN
    SET @BlackWidowId = NEWID();
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (@BlackWidowId, 31, 13, 1, N'Black Widow', N'#1C1C1C', @ExpansionId, N'black-widow.png');
    INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name) VALUES (NEWID(), 1, @BlackWidowId, 6, 1, N'Maria Hill');
END

DECLARE @BlackPantherId UNIQUEIDENTIFIER;
SELECT @BlackPantherId = Id FROM Heroes WHERE Name = N'Black Panther';
IF @BlackPantherId IS NULL
BEGIN
    SET @BlackPantherId = NEWID();
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (@BlackPantherId, 30, 14, 0, N'Black Panther', N'#7B2D8E', @ExpansionId, N'black-panther.png');
    -- Shuri's Hp conflicts across sources (6 vs 8); using 6, the better-supported value (2 of 3 sources).
    INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name) VALUES (NEWID(), 1, @BlackPantherId, 6, 1, N'Shuri');
END

IF NOT EXISTS (SELECT 1 FROM Heroes WHERE Name = N'Winter Soldier')
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (NEWID(), 30, 15, 1, N'Winter Soldier', N'#4A4A4A', @ExpansionId, N'winter-soldier.png');

IF NOT EXISTS (SELECT 1 FROM Maps WHERE Name = N'Helicarrier')
    INSERT INTO Maps (Id, Name, OfficialName, ExpansionId) VALUES (NEWID(), N'Helicarrier', N'Helicarrier', @ExpansionId);
GO

-- ============================================================
-- 9) Unmatched Marvel: Brains and Brawn
-- ============================================================
DECLARE @ExpansionId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched Marvel: Brains and Brawn');

IF NOT EXISTS (SELECT 1 FROM Heroes WHERE Name = N'Spider-Man')
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (NEWID(), 30, 15, 0, N'Spider-Man', N'#D62828', @ExpansionId, N'spider-man.png');

DECLARE @StrangeId UNIQUEIDENTIFIER;
SELECT @StrangeId = Id FROM Heroes WHERE Name = N'Doctor Strange';
IF @StrangeId IS NULL
BEGIN
    SET @StrangeId = NEWID();
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (@StrangeId, 30, 14, 1, N'Doctor Strange', N'#7209B7', @ExpansionId, N'doctor-strange.png');
    INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name) VALUES (NEWID(), 1, @StrangeId, 6, 1, N'Wong');
END

IF NOT EXISTS (SELECT 1 FROM Heroes WHERE Name = N'She-Hulk')
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (NEWID(), 30, 20, 0, N'She-Hulk', N'#6FBF73', @ExpansionId, N'she-hulk.png');

IF NOT EXISTS (SELECT 1 FROM Maps WHERE Name = N'Sanctum Sanctorum')
    INSERT INTO Maps (Id, Name, OfficialName, ExpansionId) VALUES (NEWID(), N'Sanctum Sanctorum', N'Sanctum Sanctorum', @ExpansionId);
GO

-- ============================================================
-- 10) Unmatched: Muhammad Ali vs. Bruce Lee (a.k.a. "Lee vs Ali")
-- ============================================================
DECLARE @ExpansionId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched: Muhammad Ali vs. Bruce Lee');

IF NOT EXISTS (SELECT 1 FROM Heroes WHERE Name = N'Muhammad Ali')
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (NEWID(), 30, 16, 0, N'Muhammad Ali', N'#B8860B', @ExpansionId, N'muhammad-ali.png');

IF NOT EXISTS (SELECT 1 FROM Heroes WHERE Name = N'Bruce Lee')
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId, ImageFileName)
    VALUES (NEWID(), 30, 14, 0, N'Bruce Lee', N'#FFD700', @ExpansionId, N'bruce-lee.png');

IF NOT EXISTS (SELECT 1 FROM Maps WHERE Name = N'Thrilla in Manila')
    INSERT INTO Maps (Id, Name, OfficialName, ExpansionId) VALUES (NEWID(), N'Thrilla in Manila', N'Thrilla in Manila', @ExpansionId);
IF NOT EXISTS (SELECT 1 FROM Maps WHERE Name = N'Tsing Shan Monastery')
    INSERT INTO Maps (Id, Name, OfficialName, ExpansionId) VALUES (NEWID(), N'Tsing Shan Monastery', N'Tsing Shan Monastery', @ExpansionId);
GO
