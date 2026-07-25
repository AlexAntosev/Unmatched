USE [Unmatched];
GO

-- Corrects and extends the Expansion catalog based on verified web research (see PR discussion):
-- 1) Splits the earlier, incorrectly-merged "Unmatched: The Witcher" into its two real retail boxes.
-- 2) Backfills ExpansionId for heroes/maps that were previously unassigned but are now confidently known.
-- 3) Adds four new expansions (with their heroes/maps) that weren't in the catalog at all.
-- 4) Fills in Map.OfficialName wherever the real published map name is known; where the app's Name
--    already matches the official name, OfficialName is set equal to Name for the record.
-- 5) Adds metadata-only Expansion rows (no heroes/maps yet) for real boxes that have no catalog data
--    in this app yet - those will be filled in later.
-- Anything not covered here (a handful of maps: Castle, Laboratory, Mansion, Ruins, Ship, Tavern; the
-- "London" Cobble & Fog map) has no reliable source and is intentionally left unassigned.

-- ============================================================
-- 1) Split "Unmatched: The Witcher" -> Steel and Silver / Realms Fall
-- ============================================================
DECLARE @SteelAndSilverId UNIQUEIDENTIFIER;
SELECT @SteelAndSilverId = Id FROM Expansions WHERE Name = N'Unmatched: The Witcher - Steel and Silver';
IF @SteelAndSilverId IS NULL
BEGIN
    SET @SteelAndSilverId = NEWID();
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher)
    VALUES (@SteelAndSilverId, N'Unmatched: The Witcher - Steel and Silver', 2025, N'Restoration Games');
END
GO

DECLARE @RealmsFallId UNIQUEIDENTIFIER;
SELECT @RealmsFallId = Id FROM Expansions WHERE Name = N'Unmatched: The Witcher - Realms Fall';
IF @RealmsFallId IS NULL
BEGIN
    SET @RealmsFallId = NEWID();
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher)
    VALUES (@RealmsFallId, N'Unmatched: The Witcher - Realms Fall', 2025, N'Restoration Games');
END
GO

DECLARE @SteelAndSilverId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched: The Witcher - Steel and Silver');
UPDATE Heroes SET ExpansionId = @SteelAndSilverId WHERE Name IN (N'Geralt of Rivia', N'Ciri', N'Ancient Leshen');
UPDATE Maps SET ExpansionId = @SteelAndSilverId, OfficialName = N'Kaer Morhen' WHERE Name = N'Kaer Morhen';
GO

DECLARE @RealmsFallId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched: The Witcher - Realms Fall');
UPDATE Heroes SET ExpansionId = @RealmsFallId WHERE Name IN (N'Yennefer & Triss', N'Philippa', N'Eredin');
UPDATE Maps SET ExpansionId = @RealmsFallId, OfficialName = N'Naglfar' WHERE Name = N'Naglfar';
UPDATE Maps SET ExpansionId = @RealmsFallId, OfficialName = N'Novigrad' WHERE Name = N'Streets of Novigrad';
GO

-- Now-orphaned old single box: safe to delete once nothing references it anymore.
DELETE FROM Expansions
WHERE Name = N'Unmatched: The Witcher'
  AND NOT EXISTS (SELECT 1 FROM Heroes WHERE ExpansionId = Expansions.Id)
  AND NOT EXISTS (SELECT 1 FROM Maps WHERE ExpansionId = Expansions.Id)
  AND NOT EXISTS (SELECT 1 FROM Villains WHERE ExpansionId = Expansions.Id)
  AND NOT EXISTS (SELECT 1 FROM Minions WHERE ExpansionId = Expansions.Id)
  AND NOT EXISTS (SELECT 1 FROM OwnedExpansions WHERE ExpansionId = Expansions.Id);
GO

-- ============================================================
-- 2) Unmatched Adventures: Tales to Amaze - add the 4 competitive heroes (villains/minions already assigned)
-- ============================================================
DECLARE @TalesToAmazeId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched Adventures: Tales to Amaze');
UPDATE Heroes SET ExpansionId = @TalesToAmazeId WHERE Name IN (N'Annie Christmas', N'Dr. Jill Trent', N'Golden Bat', N'Nikola Tesla') AND ExpansionId IS NULL;
UPDATE Maps SET ExpansionId = @TalesToAmazeId, OfficialName = N'Point Pleasant' WHERE Name = N'Point Pleasant' AND ExpansionId IS NULL;
UPDATE Maps SET ExpansionId = @TalesToAmazeId, OfficialName = N'McMinnville' WHERE Name = N'McMinnville' AND ExpansionId IS NULL;
GO

-- ============================================================
-- 3) New expansion: Unmatched: Robin Hood vs. Bigfoot (2020)
-- ============================================================
DECLARE @ExpansionId UNIQUEIDENTIFIER;
SELECT @ExpansionId = Id FROM Expansions WHERE Name = N'Unmatched: Robin Hood vs. Bigfoot';
IF @ExpansionId IS NULL
BEGIN
    SET @ExpansionId = NEWID();
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher)
    VALUES (@ExpansionId, N'Unmatched: Robin Hood vs. Bigfoot', 2020, N'Restoration Games');
END
GO

DECLARE @ExpansionId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched: Robin Hood vs. Bigfoot');
UPDATE Heroes SET ExpansionId = @ExpansionId WHERE Name IN (N'Robin Hood', N'Bigfoot') AND ExpansionId IS NULL;
UPDATE Maps SET ExpansionId = @ExpansionId, OfficialName = N'Sherwood Forest' WHERE Name = N'Golden forest' AND ExpansionId IS NULL;
UPDATE Maps SET ExpansionId = @ExpansionId, OfficialName = N'Yukon' WHERE Name = N'Green forest' AND ExpansionId IS NULL;
GO

-- ============================================================
-- 4) New expansion: Unmatched: Houdini vs. The Genie
-- ============================================================
DECLARE @ExpansionId UNIQUEIDENTIFIER;
SELECT @ExpansionId = Id FROM Expansions WHERE Name = N'Unmatched: Houdini vs. The Genie';
IF @ExpansionId IS NULL
BEGIN
    SET @ExpansionId = NEWID();
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher)
    VALUES (@ExpansionId, N'Unmatched: Houdini vs. The Genie', NULL, N'Restoration Games');
END
GO

DECLARE @ExpansionId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched: Houdini vs. The Genie');
UPDATE Heroes SET ExpansionId = @ExpansionId WHERE Name IN (N'Houdini', N'The Genie') AND ExpansionId IS NULL;
UPDATE Maps SET ExpansionId = @ExpansionId, OfficialName = N'King Solomon''s Mine' WHERE Name = N'King Solomon''s Mine' AND ExpansionId IS NULL;
GO

-- ============================================================
-- 5) New expansion: Unmatched Marvel - Hell's Kitchen (2021)
-- ============================================================
DECLARE @ExpansionId UNIQUEIDENTIFIER;
SELECT @ExpansionId = Id FROM Expansions WHERE Name = N'Unmatched Marvel: Hell''s Kitchen';
IF @ExpansionId IS NULL
BEGIN
    SET @ExpansionId = NEWID();
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher)
    VALUES (@ExpansionId, N'Unmatched Marvel: Hell''s Kitchen', 2021, N'Restoration Games');
END
GO

DECLARE @ExpansionId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched Marvel: Hell''s Kitchen');
UPDATE Heroes SET ExpansionId = @ExpansionId WHERE Name IN (N'Daredevil', N'Elektra', N'Bullseye') AND ExpansionId IS NULL;
UPDATE Maps SET ExpansionId = @ExpansionId, OfficialName = N'Hell''s Kitchen' WHERE Name = N'Hell''s Kitchen' AND ExpansionId IS NULL;
GO

-- ============================================================
-- 6) New expansion: Unmatched Marvel - Redemption Row (2021)
-- ============================================================
DECLARE @ExpansionId UNIQUEIDENTIFIER;
SELECT @ExpansionId = Id FROM Expansions WHERE Name = N'Unmatched Marvel: Redemption Row';
IF @ExpansionId IS NULL
BEGIN
    SET @ExpansionId = NEWID();
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher)
    VALUES (@ExpansionId, N'Unmatched Marvel: Redemption Row', 2021, N'Restoration Games');
END
GO

DECLARE @ExpansionId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched Marvel: Redemption Row');
UPDATE Heroes SET ExpansionId = @ExpansionId WHERE Name IN (N'Luke Cage', N'Ghost Rider', N'Moon Knight') AND ExpansionId IS NULL;
GO

-- ============================================================
-- 7) OfficialName for maps that were already correctly assigned to an expansion
--    (no better published source found to contradict the app's existing Name)
-- ============================================================
UPDATE Maps SET OfficialName = N'Azuchi Castle' WHERE Name = N'Azuchi Castle' AND OfficialName IS NULL;
UPDATE Maps SET OfficialName = N'Raptor Paddock' WHERE Name = N'Raptor Paddock' AND OfficialName IS NULL;
UPDATE Maps SET OfficialName = N'T. Rex Paddock' WHERE Name = N'T. Rex Paddock' AND OfficialName IS NULL;
UPDATE Maps SET OfficialName = N'Fayrlund Forest' WHERE Name = N'Fayrlund Forest' AND OfficialName IS NULL;
GO

-- ============================================================
-- 8) Metadata-only expansions: real boxes with no heroes/maps in this app's catalog yet
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM Expansions WHERE Name = N'Unmatched: Battle of Legends, Volume Three')
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher) VALUES (NEWID(), N'Unmatched: Battle of Legends, Volume Three', NULL, N'Restoration Games');
IF NOT EXISTS (SELECT 1 FROM Expansions WHERE Name = N'Unmatched: Buffy the Vampire Slayer')
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher) VALUES (NEWID(), N'Unmatched: Buffy the Vampire Slayer', NULL, N'Restoration Games');
IF NOT EXISTS (SELECT 1 FROM Expansions WHERE Name = N'Unmatched: Slings & Arrows')
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher) VALUES (NEWID(), N'Unmatched: Slings & Arrows', NULL, N'Restoration Games');
IF NOT EXISTS (SELECT 1 FROM Expansions WHERE Name = N'Unmatched: Teenage Mutant Ninja Turtles')
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher) VALUES (NEWID(), N'Unmatched: Teenage Mutant Ninja Turtles', NULL, N'Restoration Games');
IF NOT EXISTS (SELECT 1 FROM Expansions WHERE Name = N'Unmatched: Stars and Stripes')
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher) VALUES (NEWID(), N'Unmatched: Stars and Stripes', NULL, N'Restoration Games');
IF NOT EXISTS (SELECT 1 FROM Expansions WHERE Name = N'Unmatched Marvel: Teen Spirit')
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher) VALUES (NEWID(), N'Unmatched Marvel: Teen Spirit', 2021, N'Restoration Games');
IF NOT EXISTS (SELECT 1 FROM Expansions WHERE Name = N'Unmatched Marvel: For King and Country')
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher) VALUES (NEWID(), N'Unmatched Marvel: For King and Country', NULL, N'Restoration Games');
IF NOT EXISTS (SELECT 1 FROM Expansions WHERE Name = N'Unmatched Marvel: Brains and Brawn')
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher) VALUES (NEWID(), N'Unmatched Marvel: Brains and Brawn', NULL, N'Restoration Games');
IF NOT EXISTS (SELECT 1 FROM Expansions WHERE Name = N'Unmatched: Muhammad Ali vs. Bruce Lee')
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher) VALUES (NEWID(), N'Unmatched: Muhammad Ali vs. Bruce Lee', 2025, N'Restoration Games');
IF NOT EXISTS (SELECT 1 FROM Expansions WHERE Name = N'Unmatched: Bruce Lee')
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher) VALUES (NEWID(), N'Unmatched: Bruce Lee', NULL, N'Restoration Games');
IF NOT EXISTS (SELECT 1 FROM Expansions WHERE Name = N'Unmatched: Deadpool')
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher) VALUES (NEWID(), N'Unmatched: Deadpool', NULL, N'Restoration Games');
IF NOT EXISTS (SELECT 1 FROM Expansions WHERE Name = N'Unmatched: Star Wars - Escape from Hoth')
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher) VALUES (NEWID(), N'Unmatched: Star Wars - Escape from Hoth', NULL, N'Restoration Games');
IF NOT EXISTS (SELECT 1 FROM Expansions WHERE Name = N'Unmatched: Dungeons & Dragons - Tomb of Annihilation')
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher) VALUES (NEWID(), N'Unmatched: Dungeons & Dragons - Tomb of Annihilation', NULL, N'Restoration Games');
GO
