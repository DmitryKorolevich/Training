CREATE TABLE [dbo].[ContentItemsToContentProcessors]
(
	[ContentItemId] INT NOT NULL , 
    [ContentItemProcessorId] INT NOT NULL, 
    [Id] INT NOT NULL IDENTITY, 
    CONSTRAINT [PK_ContentItemsToContentProcessors] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_ContentItemsToContentProcessors_ContentItems] FOREIGN KEY ([ContentItemId]) REFERENCES [ContentItems]([Id]) ON DELETE CASCADE, 
    CONSTRAINT [FK_ContentItemsToContentProcessors_ContentProcessors] FOREIGN KEY ([ContentItemProcessorId]) REFERENCES [ContentItemProcessors]([Id]) ON DELETE CASCADE
)

GO

CREATE INDEX [IX_ContentItemsToContentProcessors_ContentItemId] ON [dbo].[ContentItemsToContentProcessors] ([ContentItemId]) WITH (FILLFACTOR = 80); 

GO