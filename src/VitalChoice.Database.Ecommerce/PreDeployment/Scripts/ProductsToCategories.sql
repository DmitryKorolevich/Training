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

IF EXISTS (SELECT * FROM sys.columns WHERE object_id =  OBJECT_ID(N'[dbo].[ProductsToCategories]', N'U') AND name='Id')
BEGIN
	DECLARE @drop_sql NVARCHAR(MAX)

	SET @drop_sql = N'ALTER TABLE [dbo].[ProductsToCategories] DROP CONSTRAINT ' + 
		(SELECT name FROM sys.objects WHERE parent_object_id = OBJECT_ID(N'[dbo].[ProductsToCategories]', N'U') AND type = N'PK')

	EXEC (@drop_sql)

	ALTER TABLE [dbo].[ProductsToCategories]
	DROP COLUMN Id

	ALTER TABLE [dbo].[ProductsToCategories]
	ADD CONSTRAINT PK_ProductsToCategories PRIMARY KEY (IdCategory, IdProduct)
END

GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id =  OBJECT_ID(N'[dbo].[ProductsToCategories]', N'U') AND name='Order')
BEGIN

	ALTER TABLE [dbo].[ProductsToCategories]
	ADD [Order] INT NULL
END

GO