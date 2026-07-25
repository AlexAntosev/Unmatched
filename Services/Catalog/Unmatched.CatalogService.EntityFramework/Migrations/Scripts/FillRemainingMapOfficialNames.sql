USE [Unmatched];
GO

-- User-confirmed official map names for the maps that had no known source before.
UPDATE Maps SET OfficialName = N'Marmoreal' WHERE Name = N'Castle' AND OfficialName IS NULL;
UPDATE Maps SET OfficialName = N'Sarpedon' WHERE Name = N'Ship' AND OfficialName IS NULL;
UPDATE Maps SET OfficialName = N'Baskerville' WHERE Name = N'Mansion' AND OfficialName IS NULL;
UPDATE Maps SET OfficialName = N'Heorot' WHERE Name = N'Tavern' AND OfficialName IS NULL;
UPDATE Maps SET OfficialName = N'The Raft' WHERE Name = N'Laboratory' AND OfficialName IS NULL;
UPDATE Maps SET OfficialName = N'Hanging' WHERE Name = N'Ruins' AND OfficialName IS NULL;
GO

-- Castle/Ship = Marmoreal/Sarpedon, the two published maps of Battle of Legends, Volume One.
DECLARE @BattleOfLegendsVol1Id UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched: Battle of Legends, Volume One');
UPDATE Maps SET ExpansionId = @BattleOfLegendsVol1Id WHERE Name IN (N'Castle', N'Ship') AND ExpansionId IS NULL;
GO

-- Mansion = Baskerville (Manor), one of Cobble & Fog's two published maps.
DECLARE @CobbleFogId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched: Cobble & Fog');
UPDATE Maps SET ExpansionId = @CobbleFogId WHERE Name = N'Mansion' AND ExpansionId IS NULL;
GO

-- Tavern = Heorot, Beowulf's mead hall - Little Red Riding Hood & Beowulf box.
DECLARE @LittleRedBeowulfId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched: Little Red Riding Hood & Beowulf');
UPDATE Maps SET ExpansionId = @LittleRedBeowulfId WHERE Name = N'Tavern' AND ExpansionId IS NULL;
GO

-- Laboratory (The Raft) and Ruins (Hanging) - official name confirmed by user, but which box
-- they belong to is still unknown; ExpansionId intentionally left untouched.
