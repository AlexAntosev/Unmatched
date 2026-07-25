USE [Unmatched];
GO

-- London = Soho, the other published Cobble & Fog map (alongside Baskerville).
UPDATE Maps SET OfficialName = N'Soho' WHERE Name = N'London' AND OfficialName IS NULL;
GO

-- Laboratory (The Raft) belongs to Unmatched Marvel: Redemption Row.
DECLARE @RedemptionRowId UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched Marvel: Redemption Row');
UPDATE Maps SET ExpansionId = @RedemptionRowId WHERE Name = N'Laboratory' AND ExpansionId IS NULL;
GO

-- Ruins (Hanging) belongs to Unmatched: Battle of Legends, Volume Two.
DECLARE @BattleOfLegendsVol2Id UNIQUEIDENTIFIER = (SELECT Id FROM Expansions WHERE Name = N'Unmatched: Battle of Legends, Volume Two');
UPDATE Maps SET ExpansionId = @BattleOfLegendsVol2Id WHERE Name = N'Ruins' AND ExpansionId IS NULL;
GO
