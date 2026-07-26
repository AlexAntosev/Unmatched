USE [Unmatched];
GO

IF COL_LENGTH('Players', 'ImageFileName') IS NULL
    ALTER TABLE Players ADD ImageFileName NVARCHAR(MAX) NULL;
GO
