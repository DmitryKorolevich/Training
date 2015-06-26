GO

IF OBJECT_ID(N'[dbo].[RecipesToProducts]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[RecipesToProducts]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[IdRecipe] INT NOT NULL, 
		[IdProduct] INT NOT NULL, 
		CONSTRAINT [FK_RecipesToProducts_ToRecipe] FOREIGN KEY ([IdRecipe]) REFERENCES [Recipes]([Id]), 
	)
END

GO