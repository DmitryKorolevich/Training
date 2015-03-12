CREATE TABLE [dbo].[ContentItemsToContentProcessors]
(
	[ContentItemId] INT NOT NULL, 
    [ContentProcessorId] INT NOT NULL, 
    CONSTRAINT [PK_ContentItemsToContentProcessors] PRIMARY KEY ([ContentProcessorId], [ContentItemId]), 
    CONSTRAINT [FK_ContentItemsToContentProcessors_ContentItems] FOREIGN KEY ([ContentItemId]) REFERENCES [ContentItems]([Id]) ON DELETE CASCADE, 
    CONSTRAINT [FK_ContentItemsToContentProcessors_ContentProcessors] FOREIGN KEY ([ContentProcessorId]) REFERENCES [ContentItemProcessors]([Id]) ON DELETE CASCADE
)
