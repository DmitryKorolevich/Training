IF OBJECT_ID(N'[dbo].[ProductOptionValues]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[ProductOptionValues]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[IdProduct] INT NULL, 
		[IdSku] INT NULL, 
		[IdOptionType] INT NOT NULL, 
		[Value] NVARCHAR(250) NULL, 
		CONSTRAINT [FK_ProductOptionValue_ToProductOptionType] FOREIGN KEY ([IdOptionType]) REFERENCES [ProductOptionTypes]([Id]), 
		CONSTRAINT [FK_ProductOptionValues_ToSku] FOREIGN KEY ([IdSku]) REFERENCES [Skus]([Id]), 
		CONSTRAINT [FK_ProductOptionValues_ToProduct] FOREIGN KEY ([IdProduct]) REFERENCES [Products]([Id])
	);

	CREATE INDEX [IX_ProductOptionValues_Value] ON [dbo].[ProductOptionValues] ([Value]) INCLUDE (Id, IdProduct, IdSku, IdOptionType)
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ProductOptionValues]') AND name = N'IdBigString')
	ALTER TABLE [ProductOptionValues]
	ADD [IdBigString] BIGINT NULL,
	CONSTRAINT [FK_ProductOptionValue_ToBigStringValue] FOREIGN KEY ([IdBigString]) REFERENCES [BigStringValues]([IdBigString]) ON DELETE CASCADE