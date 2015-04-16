CREATE TABLE [dbo].[FAQsToContentCategories]
(
	[FAQId] INT NOT NULL, 
    [ContentCategoryId] INT NOT NULL, 	
    [Id] INT NOT NULL IDENTITY, 
    CONSTRAINT [PK_FAQsToContentCategories] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_FAQsToContentCategories_FAQ] FOREIGN KEY ([FAQId]) REFERENCES [FAQs]([Id]) ON DELETE CASCADE, 
    CONSTRAINT [FK_FAQsToContentCategories_ContentCategories] FOREIGN KEY ([ContentCategoryId]) REFERENCES [ContentCategories]([Id]) ON DELETE CASCADE
)

GO

CREATE INDEX [IX_FAQsToContentCategories_ContentCategoryId] ON [dbo].[FAQsToContentCategories] ([ContentCategoryId]) WITH (FILLFACTOR = 80); 

GO