USE [Unmatched];
GO

-- Backfills ImageFileName for every existing Hero/Map/Villain/Minion row, matched by current Name
-- (post FixExistingCatalogNames/FixMartianInvaderName) to the file it was renamed to under
-- Unmatched.UI.BlazorServer/wwwroot/images/{heroes,maps,villains,minions}/ (see the git mv history
-- for that commit). Guarded on ImageFileName IS NULL so re-running or manual overrides are safe.

UPDATE Heroes SET ImageFileName = N'achilles.png' WHERE Name = N'Achilles' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'alice.png' WHERE Name = N'Alice' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'ancient-leshen.png' WHERE Name = N'Ancient Leshen' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'annie-christmas.png' WHERE Name = N'Annie Christmas' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'beowulf.png' WHERE Name = N'Beowulf' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'bigfoot.png' WHERE Name = N'Bigfoot' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'bloody-mary.png' WHERE Name = N'Bloody Mary' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'bullseye.png' WHERE Name = N'Bullseye' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'ciri.png' WHERE Name = N'Ciri' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'daredevil.png' WHERE Name = N'Daredevil' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'dr-jill-trent.png' WHERE Name = N'Dr. Jill Trent' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'dr-sattler.png' WHERE Name = N'Dr. Sattler' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'dracula.png' WHERE Name = N'Dracula' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'elektra.png' WHERE Name = N'Elektra' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'eredin.png' WHERE Name = N'Eredin' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'geralt-of-rivia.png' WHERE Name = N'Geralt of Rivia' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'ghost-rider.png' WHERE Name = N'Ghost Rider' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'golden-bat.png' WHERE Name = N'Golden Bat' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'houdini.png' WHERE Name = N'Houdini' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'invisible-man.png' WHERE Name = N'Invisible Man' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'jekyll-hyde.png' WHERE Name = N'Jekyll & Hyde' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'king-arthur.png' WHERE Name = N'King Arthur' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'little-red.png' WHERE Name = N'Little Red' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'luke-cage.png' WHERE Name = N'Luke Cage' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'medusa.png' WHERE Name = N'Medusa' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'moon-knight.png' WHERE Name = N'Moon Knight' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'nikola-tesla.png' WHERE Name = N'Nikola Tesla' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'oda-nobunaga.png' WHERE Name = N'Oda Nobunaga' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'philippa.png' WHERE Name = N'Philippa' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'raptors.png' WHERE Name = N'Raptors' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'robert-muldoon.png' WHERE Name = N'Robert Muldoon' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'robin-hood.png' WHERE Name = N'Robin Hood' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'sherlock-holmes.png' WHERE Name = N'Sherlock Holmes' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'sinbad.png' WHERE Name = N'Sinbad' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'sun-wukong.png' WHERE Name = N'Sun Wukong' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'the-genie.png' WHERE Name = N'The Genie' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'tomoe-gozen.png' WHERE Name = N'Tomoe Gozen' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N't-rex.png' WHERE Name = N'T-Rex' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'yennefer-triss.png' WHERE Name = N'Yennefer & Triss' AND ImageFileName IS NULL;
UPDATE Heroes SET ImageFileName = N'yennenga.png' WHERE Name = N'Yennenga' AND ImageFileName IS NULL;
GO

UPDATE Maps SET ImageFileName = N'azuchi-castle.png' WHERE Name = N'Azuchi Castle' AND ImageFileName IS NULL;
UPDATE Maps SET ImageFileName = N'castle.png' WHERE Name = N'Castle' AND ImageFileName IS NULL;
UPDATE Maps SET ImageFileName = N'fayrlund-forest.png' WHERE Name = N'Fayrlund Forest' AND ImageFileName IS NULL;
UPDATE Maps SET ImageFileName = N'golden-forest.png' WHERE Name = N'Golden forest' AND ImageFileName IS NULL;
UPDATE Maps SET ImageFileName = N'green-forest.png' WHERE Name = N'Green forest' AND ImageFileName IS NULL;
UPDATE Maps SET ImageFileName = N'hells-kitchen.png' WHERE Name = N'Hell''s Kitchen' AND ImageFileName IS NULL;
UPDATE Maps SET ImageFileName = N'kaer-morhen.png' WHERE Name = N'Kaer Morhen' AND ImageFileName IS NULL;
UPDATE Maps SET ImageFileName = N'king-solomons-mine.png' WHERE Name = N'King Solomon''s Mine' AND ImageFileName IS NULL;
UPDATE Maps SET ImageFileName = N'laboratory.png' WHERE Name = N'Laboratory' AND ImageFileName IS NULL;
UPDATE Maps SET ImageFileName = N'london.png' WHERE Name = N'London' AND ImageFileName IS NULL;
UPDATE Maps SET ImageFileName = N'mansion.png' WHERE Name = N'Mansion' AND ImageFileName IS NULL;
UPDATE Maps SET ImageFileName = N'mcminnville.png' WHERE Name = N'McMinnville' AND ImageFileName IS NULL;
UPDATE Maps SET ImageFileName = N'naglfar.png' WHERE Name = N'Naglfar' AND ImageFileName IS NULL;
UPDATE Maps SET ImageFileName = N'point-pleasant.png' WHERE Name = N'Point Pleasant' AND ImageFileName IS NULL;
UPDATE Maps SET ImageFileName = N'raptor-paddock.png' WHERE Name = N'Raptor Paddock' AND ImageFileName IS NULL;
UPDATE Maps SET ImageFileName = N'ruins.png' WHERE Name = N'Ruins' AND ImageFileName IS NULL;
UPDATE Maps SET ImageFileName = N'ship.png' WHERE Name = N'Ship' AND ImageFileName IS NULL;
UPDATE Maps SET ImageFileName = N'streets-of-novigrad.png' WHERE Name = N'Streets of Novigrad' AND ImageFileName IS NULL;
UPDATE Maps SET ImageFileName = N't-rex-paddock.png' WHERE Name = N'T. Rex Paddock' AND ImageFileName IS NULL;
UPDATE Maps SET ImageFileName = N'tavern.png' WHERE Name = N'Tavern' AND ImageFileName IS NULL;
GO

UPDATE Villains SET ImageFileName = N'martian-invader.png' WHERE Name = N'Martian Invader' AND ImageFileName IS NULL;
UPDATE Villains SET ImageFileName = N'mothman.png' WHERE Name = N'Mothman' AND ImageFileName IS NULL;
GO

UPDATE Minions SET ImageFileName = N'ant-queen.png' WHERE Name = N'Ant Queen' AND ImageFileName IS NULL;
UPDATE Minions SET ImageFileName = N'blob.png' WHERE Name = N'Blob' AND ImageFileName IS NULL;
UPDATE Minions SET ImageFileName = N'jersey-devil.png' WHERE Name = N'Jersey Devil' AND ImageFileName IS NULL;
UPDATE Minions SET ImageFileName = N'skunk-ape.png' WHERE Name = N'Skunk Ape' AND ImageFileName IS NULL;
UPDATE Minions SET ImageFileName = N'tarantula.png' WHERE Name = N'Tarantula' AND ImageFileName IS NULL;
UPDATE Minions SET ImageFileName = N'the-loveland-frog.png' WHERE Name = N'The Loveland Frog' AND ImageFileName IS NULL;
GO
