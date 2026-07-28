USE [Unmatched];
GO

-- Backfills ImageFileName for existing Player rows, matched by Name, to the file it was renamed to
-- under Unmatched.UI.BlazorServer/wwwroot/images/players/ (see the Catalog service's
-- BackfillImageFileNames.sql for the same pattern applied to Heroes/Maps/Villains/Minions).
-- Guarded on ImageFileName IS NULL so re-running or manual overrides are safe.

UPDATE Players SET ImageFileName = N'andrii.png' WHERE Name = N'Andrii' AND ImageFileName IS NULL;
UPDATE Players SET ImageFileName = N'ksuha.png' WHERE Name = N'Ksuha' AND ImageFileName IS NULL;
UPDATE Players SET ImageFileName = N'oleksandr.png' WHERE Name = N'Oleksandr' AND ImageFileName IS NULL;
UPDATE Players SET ImageFileName = N'tetyana.png' WHERE Name = N'Tetyana' AND ImageFileName IS NULL;
GO
