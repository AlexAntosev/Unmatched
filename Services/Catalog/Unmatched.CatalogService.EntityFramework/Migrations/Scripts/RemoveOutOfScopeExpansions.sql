USE [Unmatched];
GO

-- Star Wars: Escape from Hoth and D&D: Tomb of Annihilation are full base-game-scale products, not
-- small expansions, and are out of scope for now (per user decision). Both are still empty (no
-- Heroes/Maps/Villains/Minions/OwnedExpansions reference them) so a plain delete is safe.
DELETE FROM Expansions
WHERE Name IN (N'Unmatched: Star Wars - Escape from Hoth', N'Unmatched: Dungeons & Dragons - Tomb of Annihilation')
  AND NOT EXISTS (SELECT 1 FROM Heroes WHERE ExpansionId = Expansions.Id)
  AND NOT EXISTS (SELECT 1 FROM Maps WHERE ExpansionId = Expansions.Id)
  AND NOT EXISTS (SELECT 1 FROM Villains WHERE ExpansionId = Expansions.Id)
  AND NOT EXISTS (SELECT 1 FROM Minions WHERE ExpansionId = Expansions.Id)
  AND NOT EXISTS (SELECT 1 FROM OwnedExpansions WHERE ExpansionId = Expansions.Id);
GO
