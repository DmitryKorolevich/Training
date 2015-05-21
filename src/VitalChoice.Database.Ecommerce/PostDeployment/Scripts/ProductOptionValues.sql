IF OBJECT_ID(N'[dbo].[ProductOptionValues]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[ProductOptionValues]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[IdLookup] INT NULL, 
		[IdProduct] INT NULL, 
		[IdSku] INT NULL, 
		[IdOptionType] INT NOT NULL, 
		[Value] NVARCHAR(250) NULL, 
		CONSTRAINT [FK_ProductOptionValue_ToProductOptionType] FOREIGN KEY ([IdOptionType]) REFERENCES [ProductOptionTypes]([Id]), 
		CONSTRAINT [FK_ProductOptionValues_ToLookup] FOREIGN KEY ([IdLookup]) REFERENCES [Lookups]([Id]), 
		CONSTRAINT [FK_ProductOptionValues_ToSku] FOREIGN KEY ([IdSku]) REFERENCES [Skus]([Id]), 
		CONSTRAINT [FK_ProductOptionValues_ToProduct] FOREIGN KEY ([IdProduct]) REFERENCES [Products]([Id])
	);

	CREATE INDEX [IX_ProductOptionValues_Value] ON [dbo].[ProductOptionValues] ([Value]) INCLUDE (Id, IdLookup, IdProduct, IdSku, IdOptionType)
END