USE [Unmatched];
GO

-- Unmatched: Sun's Origin (Restoration Games, 2024) - Oda Nobunaga, Tomoe Gozen, Azuchi Castle.
-- Stats sourced/cited in Migrations/Scripts/CatalogData/suns-origin.json.
--
-- git history (commit 31fe533 "add Sun's Origin data", 2024-10-13) shows this content was likely
-- already added directly against a live dev DB and only captured in a .bacpac snapshot, with no
-- migration/seed script ever committed. So this script cannot assume the Hero/Map rows are
-- missing - it checks by Name first and only backfills ExpansionId (never re-inserts or edits
-- stats) when a row already exists, to avoid creating duplicate Heroes/Maps with a different Id
-- than whatever other services (Match/Statistics) may already reference.

DECLARE @ExpansionId UNIQUEIDENTIFIER;
SELECT @ExpansionId = Id FROM Expansions WHERE Name = N'Unmatched: Sun''s Origin';
IF @ExpansionId IS NULL
BEGIN
    SET @ExpansionId = NEWID();
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher)
    VALUES (@ExpansionId, N'Unmatched: Sun''s Origin', 2024, N'Restoration Games');
END
GO

DECLARE @ExpansionId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched: Sun''s Origin');

IF EXISTS (SELECT 1 FROM Maps WHERE Name = N'Azuchi Castle')
    UPDATE Maps SET ExpansionId = @ExpansionId WHERE Name = N'Azuchi Castle' AND ExpansionId IS NULL;
ELSE
    INSERT INTO Maps (Id, Name, ExpansionId) VALUES (NEWID(), N'Azuchi Castle', @ExpansionId);
GO

DECLARE @ExpansionId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched: Sun''s Origin');
DECLARE @OdaNobunaga UNIQUEIDENTIFIER = (SELECT Id FROM Heroes WHERE Name = N'Oda Nobunaga');

IF @OdaNobunaga IS NULL
BEGIN
    SET @OdaNobunaga = NEWID();
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId)
    VALUES (@OdaNobunaga, 30, 13, 0, N'Oda Nobunaga', '#8B1E1E', @ExpansionId);
END
ELSE
BEGIN
    UPDATE Heroes SET ExpansionId = @ExpansionId WHERE Id = @OdaNobunaga AND ExpansionId IS NULL;
END

IF NOT EXISTS (SELECT 1 FROM Sidekicks WHERE HeroId = @OdaNobunaga AND Name = N'Honor Guard')
    INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name)
    VALUES (NEWID(), 2, @OdaNobunaga, 6, 0, N'Honor Guard');
GO

DECLARE @ExpansionId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched: Sun''s Origin');
DECLARE @TomoeGozen UNIQUEIDENTIFIER = (SELECT Id FROM Heroes WHERE Name = N'Tomoe Gozen');

IF @TomoeGozen IS NULL
BEGIN
    SET @TomoeGozen = NEWID();
    INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color, ExpansionId)
    VALUES (@TomoeGozen, 30, 14, 0, N'Tomoe Gozen', '#4B3F72', @ExpansionId);
END
ELSE
BEGIN
    UPDATE Heroes SET ExpansionId = @ExpansionId WHERE Id = @TomoeGozen AND ExpansionId IS NULL;
END
GO
