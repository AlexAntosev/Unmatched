USE [Unmatched];
GO

-- Corrections sourced and verified in Migrations/Scripts/CatalogData/_name-corrections.json.
-- Only entries with "verified": true are applied here. Low-confidence findings (e.g. the "Green
-- forest"/"Golden forest" maps, which do not match any known official Unmatched map name) are
-- intentionally left untouched pending a human decision.

IF EXISTS (SELECT 1 FROM Heroes WHERE Name = 'Sherlok Holmes')
    UPDATE Heroes SET Name = 'Sherlock Holmes' WHERE Name = 'Sherlok Holmes';

IF EXISTS (SELECT 1 FROM Heroes WHERE Name = 'Robin hood')
    UPDATE Heroes SET Name = 'Robin Hood' WHERE Name = 'Robin hood';

IF EXISTS (SELECT 1 FROM Heroes WHERE Name = 'Jakyl and Hide')
    UPDATE Heroes SET Name = 'Jekyll & Hyde' WHERE Name = 'Jakyl and Hide';

IF EXISTS (SELECT 1 FROM Heroes WHERE Name = 'Ghostrider')
    UPDATE Heroes SET Name = 'Ghost Rider' WHERE Name = 'Ghostrider';

IF EXISTS (SELECT 1 FROM Heroes WHERE Name = 'Ingen')
    UPDATE Heroes SET Name = 'Robert Muldoon' WHERE Name = 'Ingen';

IF EXISTS (SELECT 1 FROM Heroes WHERE Name = 'Princess Yennenga')
    UPDATE Heroes SET Name = 'Yennenga' WHERE Name = 'Princess Yennenga';

IF EXISTS (SELECT 1 FROM Heroes WHERE Name = 'Sindbad')
    UPDATE Heroes SET Name = 'Sinbad' WHERE Name = 'Sindbad';

IF EXISTS (SELECT 1 FROM Sidekicks WHERE Name = 'Ingen Workers')
    UPDATE Sidekicks SET Name = 'InGen Workers' WHERE Name = 'Ingen Workers';

IF EXISTS (SELECT 1 FROM Maps WHERE Name = 'Hells Kitchen')
    UPDATE Maps SET Name = 'Hell''s Kitchen' WHERE Name = 'Hells Kitchen';

IF EXISTS (SELECT 1 FROM Maps WHERE Name = 'Raptor paddock')
    UPDATE Maps SET Name = 'Raptor Paddock' WHERE Name = 'Raptor paddock';

IF EXISTS (SELECT 1 FROM Maps WHERE Name = 'T. Rex paddock')
    UPDATE Maps SET Name = 'T. Rex Paddock' WHERE Name = 'T. Rex paddock';
GO
