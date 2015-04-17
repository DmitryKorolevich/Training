CREATE TABLE [dbo].[ContentPagesToContentCategories]
(
	[ContentPageId] INT NOT NULL, 
    [ContentCategoryId] INT NOT NULL, 	
    [Id] INT NOT NULL IDENTITY, 
    CONSTRAINT [PK_ContentPagesToContentCategories] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_ContentPagesToContentCategories_Recipes] FOREIGN KEY ([ContentPageId]) REFERENCES [ContentPages]([Id]) ON DELETE CASCADE, 
    CONSTRAINT [FK_ContentPagesToContentCategories_ContentCategories] FOREIGN KEY ([ContentCategoryId]) REFERENCES [ContentCategories]([Id]) ON DELETE CASCADE
)

GO

CREATE INDEX [IX_ContentPagesToContentCategories_ContentCategoryId] ON [dbo].[ContentPagesToContentCategories] ([ContentCategoryId]) WITH (FILLFACTOR = 80); 

GO