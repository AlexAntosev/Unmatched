USE [Unmatched];
GO

-- The design handoff shipped real box art for two sets; the rest keep the dashed placeholder
-- until a cover is uploaded from the Collection screen. Only fills blanks, so a cover uploaded
-- before this migration runs is never overwritten.
UPDATE Expansions
SET ImageFileName = 'cobble-and-fog.png'
WHERE Name = 'Unmatched: Cobble & Fog' AND ImageFileName IS NULL;
GO

UPDATE Expansions
SET ImageFileName = 'battle-of-legends-3.png'
WHERE Name = 'Unmatched: Battle of Legends, Volume Three' AND ImageFileName IS NULL;
GO
