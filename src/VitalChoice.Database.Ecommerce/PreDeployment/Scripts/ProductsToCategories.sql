IF OBJECT_ID(N'[dbo].[ProductsToCategories]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[ProductsToCategories]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[IdCategory] INT NOT NULL, 
		[IdProduct] INT NOT NULL, 
		CONSTRAINT [FK_ProductsToCategories_ToProduct] FOREIGN KEY ([IdProduct]) REFERENCES [Products]([Id]), 
		CONSTRAINT [FK_ProductsToCategories_ToProductCategory] FOREIGN KEY ([IdCategory]) REFERENCES [ProductCategories]([Id])
	)
END