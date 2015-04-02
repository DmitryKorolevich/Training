CREATE TABLE [dbo].[MasterContentItemsToContentProcessors]
(
	[MasterContentItemId] INT NOT NULL , 
    [ContentProcessorId] INT NOT NULL, 
    [Id] INT NOT NULL IDENTITY, 
    CONSTRAINT [PK_MasterContentItemsToContentProcessors] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_MasterContentItemsToContentProcessors_MasterContentItems] FOREIGN KEY ([MasterContentItemId]) REFERENCES [MasterContentItems]([Id]) ON DELETE CASCADE, 
    CONSTRAINT [FK_MasterContentItemsToContentProcessors_ContentProcessors] FOREIGN KEY ([ContentProcessorId]) REFERENCES [ContentProcessors]([Id]) ON DELETE CASCADE
)

GO

CREATE INDEX [IX_ContentItemsToContentProcessors_MasterContentItemId] ON [dbo].[MasterContentItemsToContentProcessors] ([MasterContentItemId]) WITH (FILLFACTOR = 80); 

GO