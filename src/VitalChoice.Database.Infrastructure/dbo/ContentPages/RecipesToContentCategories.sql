CREATE TABLE [dbo].[RecipesToContentCategories]
(
	[RecipesId] INT NOT NULL, 
    [ContentCategoryId] INT NOT NULL, 
    CONSTRAINT [PK_RecipesToContentCategories] PRIMARY KEY ([RecipesId], [ContentCategoryId]), 
    CONSTRAINT [FK_RecipesToContentCategories_Recipes] FOREIGN KEY ([RecipesId]) REFERENCES [Recipes]([Id]) ON DELETE CASCADE, 
    CONSTRAINT [FK_RecipesToContentCategories_ContentCategories] FOREIGN KEY ([ContentCategoryId]) REFERENCES [ContentCategories]([Id]) ON DELETE CASCADE
)
