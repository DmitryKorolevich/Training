CREATE TABLE [dbo].[Articles]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Url] NVARCHAR(250) NOT NULL, 
    [Name] NVARCHAR(250) NOT NULL, 
    [FileUrl] NVARCHAR(250) NULL, 
    [ContentItemId] INT NOT NULL, 
    [MasterContentItemId] INT NOT NULL, 
	[StatusCode] INT NOT NULL DEFAULT 1,
    [PublishedDate] DATETIME NULL, 
    [SubTitle] NVARCHAR(250) NULL, 
    [Author] NVARCHAR(250) NULL, 
    CONSTRAINT [FK_Articles_ToContentItem] FOREIGN KEY (ContentItemId) REFERENCES ContentItems([Id]), 
    CONSTRAINT [FK_Articles_ToMasterContentItem] FOREIGN KEY (MasterContentItemId) REFERENCES MasterContentItems([Id]),
	CONSTRAINT [FK_Articles_ToRecordStatusCode] FOREIGN KEY ([StatusCode]) REFERENCES [RecordStatusCodes]([StatusCode]),
)

GO

CREATE INDEX [IX_Articles_Url] ON [dbo].[Recipes] ([Url]) WITH (FILLFACTOR = 80); 

GO

CREATE INDEX [IX_Articles_MasterContentItemId] ON [dbo].[Recipes] ([MasterContentItemId]) WITH (FILLFACTOR = 80); 

GO