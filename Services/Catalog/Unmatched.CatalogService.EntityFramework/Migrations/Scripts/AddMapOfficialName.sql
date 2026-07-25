USE [Unmatched];
GO

IF COL_LENGTH('Maps', 'OfficialName') IS NULL
BEGIN
    ALTER TABLE Maps ADD OfficialName NVARCHAR(MAX) NULL;
END
GO
