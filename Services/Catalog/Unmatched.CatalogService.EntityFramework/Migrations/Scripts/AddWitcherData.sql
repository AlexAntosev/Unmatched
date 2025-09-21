USE [Unmatched];
GO

insert into Maps (Id, Name) values 
('2DD2D958-2FDC-40D5-A745-8FAF21477558' , 'Kaer Morhen'), 
('9DAEDF18-23BC-42AC-BE80-88023C7B4432','Fayrlund Forest'),
('44D5229F-D311-43A3-8F41-6FC512517E52','Naglfar'),
('FB1D3C51-5407-4CFD-809C-8640BD998F0F','Streets of Novigrad')



-- Yennefer & Triss
DECLARE @Yennefer UNIQUEIDENTIFIER = NEWID();
INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color)
VALUES (@Yennefer, 30, 14, 1, 'Yennefer & Triss', '#FFA721');

INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name)
VALUES (NEWID(), 1, @Yennefer, 6, 1, 'Triss & Yennefer');

-- Eredin
DECLARE @Eredin UNIQUEIDENTIFIER = NEWID();
INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color)
VALUES (@Eredin, 30, 14, 0, 'Eredin', '#423F40');

INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name)
VALUES (NEWID(), 4, @Eredin, 1, 0, 'Red Rider');

-- Philippa
DECLARE @Philippa UNIQUEIDENTIFIER = NEWID();
INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color)
VALUES (@Philippa, 30, 12, 1, 'Philippa', '#C7595A');

INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name)
VALUES (NEWID(), 1, @Philippa, 6, 0, 'Dijkstra');

-- Ancient Leshen
DECLARE @Leshen UNIQUEIDENTIFIER = NEWID();
INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color)
VALUES (@Leshen, 30, 13, 1, 'Ancient Leshen', '#5B6139');

INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name)
VALUES (NEWID(), 2, @Leshen, 1, 0, 'Wolf');

-- Ciri
DECLARE @Ciri UNIQUEIDENTIFIER = NEWID();
INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color)
VALUES (@Ciri, 30, 15, 0, 'Ciri', '#7ABFBC');

INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name)
VALUES (NEWID(), 1, @Ciri, 7, 0, 'Ihuarraquax');

-- Geralt of Rivia
DECLARE @Geralt UNIQUEIDENTIFIER = NEWID();
INSERT INTO Heroes (Id, DeckSize, Hp, IsRanged, Name, Color)
VALUES (@Geralt, 30, 16, 0, 'Geralt of Rivia', '#DBDEE6');

INSERT INTO Sidekicks (Id, Count, HeroId, Hp, IsRanged, Name)
VALUES (NEWID(), 1, @Geralt, 5, 1, 'Dandelion');
GO