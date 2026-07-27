USE [Unmatched];
GO

-- Villains and minions were created before the Expansion entity existed, so their ExpansionId is
-- still NULL on databases restored from a .bacpac: AddKnownExpansions and AddMissingCatalogContent
-- are recorded as applied, so their UPDATE/INSERT statements never ran against these rows.
--
-- The Collection screen lists what is inside each box, which needs those links, so the same
-- assignments are re-applied here. Every statement is guarded with ExpansionId IS NULL, so this is
-- idempotent and never moves a row that already belongs to a box.

DECLARE @TalesToAmazeId UNIQUEIDENTIFIER =
    (SELECT Id FROM Expansions WHERE Name = N'Unmatched Adventures: Tales to Amaze');

IF @TalesToAmazeId IS NOT NULL
BEGIN
    UPDATE Villains SET ExpansionId = @TalesToAmazeId
    WHERE Name IN (N'Martian Invader', N'Mothman') AND ExpansionId IS NULL;

    UPDATE Minions SET ExpansionId = @TalesToAmazeId
    WHERE Name IN (N'Ant Queen', N'Blob', N'Jersey Devil', N'Skunk Ape', N'Tarantula', N'The Loveland Frog')
      AND ExpansionId IS NULL;
END
GO

DECLARE @TmntId UNIQUEIDENTIFIER =
    (SELECT Id FROM Expansions WHERE Name = N'Unmatched Adventures: Teenage Mutant Ninja Turtles');

IF @TmntId IS NOT NULL
BEGIN
    UPDATE Villains SET ExpansionId = @TmntId
    WHERE Name IN (N'Shredder', N'Krang') AND ExpansionId IS NULL;

    UPDATE Minions SET ExpansionId = @TmntId
    WHERE Name IN (N'Baxter Stockman', N'Bebop & Rocksteady', N'Leatherhead', N'Rat King', N'Slash', N'Wingnut')
      AND ExpansionId IS NULL;
END
GO
