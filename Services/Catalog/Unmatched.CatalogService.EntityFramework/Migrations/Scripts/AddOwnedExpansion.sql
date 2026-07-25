USE [Unmatched];
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'OwnedExpansions')
BEGIN
    CREATE TABLE OwnedExpansions (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_OwnedExpansions PRIMARY KEY,
        ExpansionId UNIQUEIDENTIFIER NOT NULL
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_OwnedExpansions_ExpansionId' AND object_id = OBJECT_ID('OwnedExpansions'))
BEGIN
    CREATE UNIQUE INDEX IX_OwnedExpansions_ExpansionId ON OwnedExpansions(ExpansionId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_OwnedExpansions_Expansions_ExpansionId')
BEGIN
    ALTER TABLE OwnedExpansions ADD CONSTRAINT FK_OwnedExpansions_Expansions_ExpansionId
        FOREIGN KEY (ExpansionId) REFERENCES Expansions(Id) ON DELETE CASCADE;
END
GO
