CREATE TABLE [dbo].[ArticlesToContentCategories]
(
	[ArticleId] INT NOT NULL, 
    [ContentCategoryId] INT NOT NULL, 	
    [Id] INT NOT NULL IDENTITY, 
    CONSTRAINT [PK_ArticlesToContentCategories] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_ArticlesToContentCategories_Recipes] FOREIGN KEY ([ArticleId]) REFERENCES [Recipes]([Id]) ON DELETE CASCADE, 
    CONSTRAINT [FK_ArticlesToContentCategories_ContentCategories] FOREIGN KEY ([ContentCategoryId]) REFERENCES [ContentCategories]([Id]) ON DELETE CASCADE
)

GO

CREATE INDEX [IX_ArticlesToContentCategories_ContentCategoryId] ON [dbo].[ArticlesToContentCategories] ([ContentCategoryId]) WITH (FILLFACTOR = 80); 

GO