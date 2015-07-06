IF OBJECT_ID(N'[dbo].[BigStringValues]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[BigStringValues]
	(
		[IdBigString] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
		[Value] NVARCHAR(MAX) NULL
	)
END