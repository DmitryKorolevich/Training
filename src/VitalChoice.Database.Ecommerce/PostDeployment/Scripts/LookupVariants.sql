IF OBJECT_ID(N'[dbo].[LookupVariants]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[LookupVariants]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[IdLookup] INT NOT NULL, 
		[ValueVariant] NVARCHAR(50) NULL, 
		CONSTRAINT [FK_LookupVariants_ToLookup] FOREIGN KEY ([IdLookup]) REFERENCES [Lookups]([Id])
	)
END