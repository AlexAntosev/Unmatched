USE [Unmatched];
GO

-- Seeds the real-world Unmatched expansion boxes the app maintainer could confidently identify, and
-- backfills ExpansionId on the existing Heroes/Maps/Villains/Minions that belong to them, matched by
-- Name (never inserting new catalog rows here - all of these already exist in the DB). Everything not
-- listed below is left with ExpansionId = NULL rather than guessed, since the Collection filter treats
-- NULL as "always shown".

-- Unmatched: Cobble & Fog (Restoration Games, 2020)
DECLARE @ExpansionId UNIQUEIDENTIFIER;
SELECT @ExpansionId = Id FROM Expansions WHERE Name = N'Unmatched: Cobble & Fog';
IF @ExpansionId IS NULL
BEGIN
    SET @ExpansionId = NEWID();
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher)
    VALUES (@ExpansionId, N'Unmatched: Cobble & Fog', 2020, N'Restoration Games');
END
GO

DECLARE @ExpansionId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched: Cobble & Fog');
UPDATE Heroes SET ExpansionId = @ExpansionId WHERE Name IN (N'Dracula', N'Invisible Man', N'Jekyll & Hyde', N'Sherlock Holmes') AND ExpansionId IS NULL;
UPDATE Maps SET ExpansionId = @ExpansionId WHERE Name = N'London' AND ExpansionId IS NULL;
GO

-- Unmatched: Battle of Legends, Volume One (Restoration Games, 2019)
DECLARE @ExpansionId UNIQUEIDENTIFIER;
SELECT @ExpansionId = Id FROM Expansions WHERE Name = N'Unmatched: Battle of Legends, Volume One';
IF @ExpansionId IS NULL
BEGIN
    SET @ExpansionId = NEWID();
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher)
    VALUES (@ExpansionId, N'Unmatched: Battle of Legends, Volume One', 2019, N'Restoration Games');
END
GO

DECLARE @ExpansionId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched: Battle of Legends, Volume One');
UPDATE Heroes SET ExpansionId = @ExpansionId WHERE Name IN (N'Medusa', N'Sinbad', N'King Arthur', N'Alice') AND ExpansionId IS NULL;
GO

-- Unmatched: Battle of Legends, Volume Two (Restoration Games, 2021)
DECLARE @ExpansionId UNIQUEIDENTIFIER;
SELECT @ExpansionId = Id FROM Expansions WHERE Name = N'Unmatched: Battle of Legends, Volume Two';
IF @ExpansionId IS NULL
BEGIN
    SET @ExpansionId = NEWID();
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher)
    VALUES (@ExpansionId, N'Unmatched: Battle of Legends, Volume Two', 2021, N'Restoration Games');
END
GO

DECLARE @ExpansionId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched: Battle of Legends, Volume Two');
UPDATE Heroes SET ExpansionId = @ExpansionId WHERE Name IN (N'Achilles', N'Bloody Mary', N'Sun Wukong', N'Yennenga') AND ExpansionId IS NULL;
GO

-- Unmatched: The Witcher (Restoration Games, 2022)
DECLARE @ExpansionId UNIQUEIDENTIFIER;
SELECT @ExpansionId = Id FROM Expansions WHERE Name = N'Unmatched: The Witcher';
IF @ExpansionId IS NULL
BEGIN
    SET @ExpansionId = NEWID();
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher)
    VALUES (@ExpansionId, N'Unmatched: The Witcher', 2022, N'Restoration Games');
END
GO

DECLARE @ExpansionId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched: The Witcher');
UPDATE Heroes SET ExpansionId = @ExpansionId WHERE Name IN (N'Geralt of Rivia', N'Yennefer & Triss', N'Ciri', N'Ancient Leshen') AND ExpansionId IS NULL;
UPDATE Maps SET ExpansionId = @ExpansionId WHERE Name IN (N'Kaer Morhen', N'Streets of Novigrad', N'Naglfar') AND ExpansionId IS NULL;
GO

-- Unmatched: Jurassic Park - InGen vs Raptors (Restoration Games, 2021)
DECLARE @ExpansionId UNIQUEIDENTIFIER;
SELECT @ExpansionId = Id FROM Expansions WHERE Name = N'Unmatched: Jurassic Park - InGen vs Raptors';
IF @ExpansionId IS NULL
BEGIN
    SET @ExpansionId = NEWID();
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher)
    VALUES (@ExpansionId, N'Unmatched: Jurassic Park - InGen vs Raptors', 2021, N'Restoration Games');
END
GO

DECLARE @ExpansionId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched: Jurassic Park - InGen vs Raptors');
UPDATE Heroes SET ExpansionId = @ExpansionId WHERE Name IN (N'Robert Muldoon', N'Raptors') AND ExpansionId IS NULL;
UPDATE Maps SET ExpansionId = @ExpansionId WHERE Name = N'Raptor Paddock' AND ExpansionId IS NULL;
GO

-- Unmatched: Jurassic Park - Dr. Sattler & Dr. Grant vs T. Rex (Restoration Games, 2021)
DECLARE @ExpansionId UNIQUEIDENTIFIER;
SELECT @ExpansionId = Id FROM Expansions WHERE Name = N'Unmatched: Jurassic Park - Dr. Sattler & Dr. Grant vs T. Rex';
IF @ExpansionId IS NULL
BEGIN
    SET @ExpansionId = NEWID();
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher)
    VALUES (@ExpansionId, N'Unmatched: Jurassic Park - Dr. Sattler & Dr. Grant vs T. Rex', 2021, N'Restoration Games');
END
GO

DECLARE @ExpansionId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched: Jurassic Park - Dr. Sattler & Dr. Grant vs T. Rex');
UPDATE Heroes SET ExpansionId = @ExpansionId WHERE Name IN (N'Dr. Sattler', N'T-Rex') AND ExpansionId IS NULL;
UPDATE Maps SET ExpansionId = @ExpansionId WHERE Name = N'T. Rex Paddock' AND ExpansionId IS NULL;
GO

-- Unmatched: Little Red Riding Hood & Beowulf (Restoration Games, 2020)
DECLARE @ExpansionId UNIQUEIDENTIFIER;
SELECT @ExpansionId = Id FROM Expansions WHERE Name = N'Unmatched: Little Red Riding Hood & Beowulf';
IF @ExpansionId IS NULL
BEGIN
    SET @ExpansionId = NEWID();
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher)
    VALUES (@ExpansionId, N'Unmatched: Little Red Riding Hood & Beowulf', 2020, N'Restoration Games');
END
GO

DECLARE @ExpansionId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched: Little Red Riding Hood & Beowulf');
UPDATE Heroes SET ExpansionId = @ExpansionId WHERE Name IN (N'Little Red', N'Beowulf') AND ExpansionId IS NULL;
UPDATE Maps SET ExpansionId = @ExpansionId WHERE Name = N'Fayrlund Forest' AND ExpansionId IS NULL;
GO

-- Unmatched Adventures: Tales to Amaze (Restoration Games, 2021)
DECLARE @ExpansionId UNIQUEIDENTIFIER;
SELECT @ExpansionId = Id FROM Expansions WHERE Name = N'Unmatched Adventures: Tales to Amaze';
IF @ExpansionId IS NULL
BEGIN
    SET @ExpansionId = NEWID();
    INSERT INTO Expansions (Id, Name, ReleaseYear, Publisher)
    VALUES (@ExpansionId, N'Unmatched Adventures: Tales to Amaze', 2021, N'Restoration Games');
END
GO

DECLARE @ExpansionId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched Adventures: Tales to Amaze');
UPDATE Villains SET ExpansionId = @ExpansionId WHERE Name IN (N'Martian Invader', N'Mothman') AND ExpansionId IS NULL;
UPDATE Minions SET ExpansionId = @ExpansionId WHERE Name IN (N'Ant Queen', N'Blob', N'Jersey Devil', N'Skunk Ape', N'Tarantula', N'The Loveland Frog') AND ExpansionId IS NULL;
GO
