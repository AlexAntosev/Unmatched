USE [Unmatched];
GO

-- Vados/Zheka aren't in PlayerNames.cs (that file only tracks a subset), but they're real Player rows
-- in the DB with photos already in wwwroot/images/players/ - missed by the first backfill pass.
UPDATE Players SET ImageFileName = N'vados.png' WHERE Name = N'Vados' AND ImageFileName IS NULL;
UPDATE Players SET ImageFileName = N'zheka.png' WHERE Name = N'Zheka' AND ImageFileName IS NULL;
GO
