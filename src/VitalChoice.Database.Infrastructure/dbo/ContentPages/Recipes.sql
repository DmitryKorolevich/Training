CREATE TABLE [dbo].[Recipes]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Url] NVARCHAR(250) NOT NULL, 
    [Name] NVARCHAR(250) NOT NULL, 
    [ContentItemId] INT NOT NULL, 
    [MasterContentItemId] INT NOT NULL, 
	[StatusCode] INT NOT NULL DEFAULT 1,
    CONSTRAINT [FK_Recipes_ToContentItem] FOREIGN KEY (ContentItemId) REFERENCES ContentItems([Id]), 
    CONSTRAINT [FK_Recipes_ToMasterContentItem] FOREIGN KEY (MasterContentItemId) REFERENCES MasterContentItems([Id]),
	CONSTRAINT [FK_Recipes_ToRecordStatusCode] FOREIGN KEY ([StatusCode]) REFERENCES [RecordStatusCodes]([StatusCode]),
)

GO

CREATE INDEX [IX_Recipes_Url] ON [dbo].[Recipes] ([Url]) WITH (FILLFACTOR = 80); 

GO

CREATE INDEX [IX_Recipes_MasterContentItemId] ON [dbo].[Recipes] ([MasterContentItemId]) WITH (FILLFACTOR = 80); 

GO