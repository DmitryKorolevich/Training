IF OBJECT_ID(N'[dbo].[LookupVariants]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[LookupVariants]
	(
		[Id] INT NOT NULL, 
		[IdLookup] INT NOT NULL, 
		[ValueVariant] NVARCHAR(250) NULL, 
		CONSTRAINT [FK_LookupVariants_ToLookup] FOREIGN KEY ([IdLookup]) REFERENCES [Lookups]([Id]),
		CONSTRAINT [PK_LookupVariants] PRIMARY KEY (Id, IdLookup)
	)
END