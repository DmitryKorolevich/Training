IF OBJECT_ID(N'[dbo].[Lookups]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[Lookups]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[LookupValueType] NVARCHAR(250) NOT NULL
	)
END