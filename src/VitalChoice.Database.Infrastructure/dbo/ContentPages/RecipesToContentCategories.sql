CREATE TABLE [dbo].[RecipesToContentCategories]
(
	[RecipeId] INT NOT NULL, 
    [ContentCategoryId] INT NOT NULL, 	
    [Id] INT NOT NULL IDENTITY, 
    CONSTRAINT [PK_RecipesToContentCategories] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_RecipesToContentCategories_Recipes] FOREIGN KEY ([RecipeId]) REFERENCES [Recipes]([Id]) ON DELETE CASCADE, 
    CONSTRAINT [FK_RecipesToContentCategories_ContentCategories] FOREIGN KEY ([ContentCategoryId]) REFERENCES [ContentCategories]([Id]) ON DELETE CASCADE
)
