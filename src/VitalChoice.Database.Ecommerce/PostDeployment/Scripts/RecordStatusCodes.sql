IF OBJECT_ID(N'dbo.RecordStatusCodes', N'U') IS NULL
BEGIN

CREATE TABLE [dbo].[RecordStatusCodes] (
    [StatusCode] INT           NOT NULL,
    [Name]       NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([StatusCode] ASC)
);

INSERT INTO RecordStatusCodes
(StatusCode, Name)
SELECT 1, 'NotActive'
UNION
SELECT 2, 'Active'
UNION
SELECT 3, 'Deleted'

END