USE [Unmatched];
GO

-- Stat rows are only ever created when a hero/map/villain/minion appears in a played match (see
-- StatisticsInitializer/*StatsCoordinator), so catalog entries that have never been played have no
-- row here and are invisible to the statistics pages. This backfills a zero-stat row for every
-- catalog entry that is still missing one. Guarded by NOT EXISTS, so it never touches a row that
-- already exists.

INSERT INTO HeroStats (HeroId, Name, Color, Hp, DeckSize, IsRanged, ImageFileName, Place, Points,
                        TotalLooses, TotalMatches, TotalWins, LastMatchPoints, ModifiedAt, LastMatchIncludedAt)
SELECT h.Id, h.Name, h.Color, h.Hp, h.DeckSize, h.IsRanged, h.ImageFileName, 0, 0, 0, 0, 0, 0,
       GETUTCDATE(), '0001-01-01T00:00:00'
FROM Heroes h
WHERE NOT EXISTS (SELECT 1 FROM HeroStats hs WHERE hs.HeroId = h.Id);
GO

INSERT INTO MapStats (MapId, Name, ImageFileName, TotalMatches, ModifiedAt, LastMatchIncludedAt)
SELECT m.Id, m.Name, m.ImageFileName, 0, GETUTCDATE(), '0001-01-01T00:00:00'
FROM Maps m
WHERE NOT EXISTS (SELECT 1 FROM MapStats ms WHERE ms.MapId = m.Id);
GO

INSERT INTO VillainStats (VillainId, Name, Color, Hp, DeckSize, IsRanged, ImageFileName,
                           TotalLooses, TotalMatches, TotalWins, ModifiedAt, LastMatchIncludedAt)
SELECT v.Id, v.Name, v.Color, v.Hp, v.DeckSize, v.IsRanged, v.ImageFileName, 0, 0, 0,
       GETUTCDATE(), '0001-01-01T00:00:00'
FROM Villains v
WHERE NOT EXISTS (SELECT 1 FROM VillainStats vs WHERE vs.VillainId = v.Id);
GO

INSERT INTO MinionStats (MinionId, Name, Color, Hp, DeckSize, IsRanged, ImageFileName,
                          TotalLooses, TotalMatches, TotalWins, ModifiedAt, LastMatchIncludedAt)
SELECT mn.Id, mn.Name, mn.Color, mn.Hp, mn.DeckSize, mn.IsRanged, mn.ImageFileName, 0, 0, 0,
       GETUTCDATE(), '0001-01-01T00:00:00'
FROM Minions mn
WHERE NOT EXISTS (SELECT 1 FROM MinionStats mns WHERE mns.MinionId = mn.Id);
GO
