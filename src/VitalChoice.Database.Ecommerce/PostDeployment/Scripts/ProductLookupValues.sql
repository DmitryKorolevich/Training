IF OBJECT_ID(N'[dbo].[ProductLookupValues]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[ProductLookupValues]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[IdLookup] INT NOT NULL, 
		[IdValue] INT NOT NULL, 
		CONSTRAINT [FK_ProductLookupValue_ToLookup] FOREIGN KEY ([IdLookup]) REFERENCES [Lookups]([Id]), 
		CONSTRAINT [FK_ProductLookupValue_ToLookupVariant] FOREIGN KEY ([IdValue]) REFERENCES [LookupVariants]([Id])
	)
END