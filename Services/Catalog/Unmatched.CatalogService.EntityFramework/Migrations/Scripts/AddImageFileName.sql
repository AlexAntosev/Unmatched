USE [Unmatched];
GO

IF COL_LENGTH('Heroes', 'ImageFileName') IS NULL
    ALTER TABLE Heroes ADD ImageFileName NVARCHAR(MAX) NULL;
GO

IF COL_LENGTH('Maps', 'ImageFileName') IS NULL
    ALTER TABLE Maps ADD ImageFileName NVARCHAR(MAX) NULL;
GO

IF COL_LENGTH('Villains', 'ImageFileName') IS NULL
    ALTER TABLE Villains ADD ImageFileName NVARCHAR(MAX) NULL;
GO

IF COL_LENGTH('Minions', 'ImageFileName') IS NULL
    ALTER TABLE Minions ADD ImageFileName NVARCHAR(MAX) NULL;
GO
