GO

IF OBJECT_ID(N'[dbo].[ArticlesToProducts]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].ArticlesToProducts
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[IdArticle] INT NOT NULL, 
		[IdProduct] INT NOT NULL, 
		CONSTRAINT [FK_ArticlesToProducts_ToRecipe] FOREIGN KEY ([IdArticle]) REFERENCES [Articles]([Id]), 
	)
END

GO