CREATE TABLE [dbo].[ContentPages]
(
	[Id] INT NOT NULL PRIMARY KEY,
    [Url] NVARCHAR(250) NOT NULL, 
    [Name] NVARCHAR(250) NOT NULL, 
    [FileUrl] NVARCHAR(250) NULL, 
    [ContentItemId] INT NOT NULL, 
    [MasterContentItemId] INT NOT NULL, 
	[StatusCode] INT NOT NULL DEFAULT 1,
    [Assigned] INT NOT NULL DEFAULT 1, 
    CONSTRAINT [FK_ContentPages_ToContentItem] FOREIGN KEY (ContentItemId) REFERENCES ContentItems([Id]), 
    CONSTRAINT [FK_ContentPages_ToMasterContentItem] FOREIGN KEY (MasterContentItemId) REFERENCES MasterContentItems([Id]),
	CONSTRAINT [FK_ContentPages_ToRecordStatusCode] FOREIGN KEY ([StatusCode]) REFERENCES [RecordStatusCodes]([StatusCode]),
)
GO

CREATE INDEX [IX_ContentPages_Url] ON [dbo].[ContentPages] ([Url]) WITH (FILLFACTOR = 80); 

GO

CREATE INDEX [IX_ContentPages_MasterContentItemId] ON [dbo].[Recipes] ([MasterContentItemId]) WITH (FILLFACTOR = 80); 

GO
