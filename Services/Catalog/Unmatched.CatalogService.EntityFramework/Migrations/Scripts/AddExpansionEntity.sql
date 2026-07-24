USE [Unmatched];
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Expansions')
BEGIN
    CREATE TABLE Expansions (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Expansions PRIMARY KEY,
        Name NVARCHAR(450) NOT NULL,
        ReleaseYear INT NULL,
        Publisher NVARCHAR(MAX) NULL
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Expansions_Name' AND object_id = OBJECT_ID('Expansions'))
BEGIN
    CREATE UNIQUE INDEX IX_Expansions_Name ON Expansions(Name);
END
GO

IF COL_LENGTH('Heroes', 'ExpansionId') IS NULL
BEGIN
    ALTER TABLE Heroes ADD ExpansionId UNIQUEIDENTIFIER NULL;
END
GO

IF COL_LENGTH('Maps', 'ExpansionId') IS NULL
BEGIN
    ALTER TABLE Maps ADD ExpansionId UNIQUEIDENTIFIER NULL;
END
GO

IF COL_LENGTH('Villains', 'ExpansionId') IS NULL
BEGIN
    ALTER TABLE Villains ADD ExpansionId UNIQUEIDENTIFIER NULL;
END
GO

IF COL_LENGTH('Minions', 'ExpansionId') IS NULL
BEGIN
    ALTER TABLE Minions ADD ExpansionId UNIQUEIDENTIFIER NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Heroes_ExpansionId' AND object_id = OBJECT_ID('Heroes'))
BEGIN
    CREATE INDEX IX_Heroes_ExpansionId ON Heroes(ExpansionId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Maps_ExpansionId' AND object_id = OBJECT_ID('Maps'))
BEGIN
    CREATE INDEX IX_Maps_ExpansionId ON Maps(ExpansionId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Villains_ExpansionId' AND object_id = OBJECT_ID('Villains'))
BEGIN
    CREATE INDEX IX_Villains_ExpansionId ON Villains(ExpansionId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Minions_ExpansionId' AND object_id = OBJECT_ID('Minions'))
BEGIN
    CREATE INDEX IX_Minions_ExpansionId ON Minions(ExpansionId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Heroes_Expansions_ExpansionId')
BEGIN
    ALTER TABLE Heroes ADD CONSTRAINT FK_Heroes_Expansions_ExpansionId
        FOREIGN KEY (ExpansionId) REFERENCES Expansions(Id) ON DELETE SET NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Maps_Expansions_ExpansionId')
BEGIN
    ALTER TABLE Maps ADD CONSTRAINT FK_Maps_Expansions_ExpansionId
        FOREIGN KEY (ExpansionId) REFERENCES Expansions(Id) ON DELETE SET NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Villains_Expansions_ExpansionId')
BEGIN
    ALTER TABLE Villains ADD CONSTRAINT FK_Villains_Expansions_ExpansionId
        FOREIGN KEY (ExpansionId) REFERENCES Expansions(Id) ON DELETE SET NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Minions_Expansions_ExpansionId')
BEGIN
    ALTER TABLE Minions ADD CONSTRAINT FK_Minions_Expansions_ExpansionId
        FOREIGN KEY (ExpansionId) REFERENCES Expansions(Id) ON DELETE SET NULL;
END
GO
