CREATE TABLE [dbo].[ContentPages]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[ContentItemId] INT NOT NULL, 
    [MasterContentItemId] INT NOT NULL, 
    [Name] NVARCHAR(250) NOT NULL, 
    [Url] NVARCHAR(250) NOT NULL, 
	[StatusCode] INT NOT NULL DEFAULT 1,
    CONSTRAINT [FK_ContentPages_ToContentItem] FOREIGN KEY (ContentItemId) REFERENCES ContentItems([Id]), 
    CONSTRAINT [FK_ContentPages_ToMasterContentItem] FOREIGN KEY (MasterContentItemId) REFERENCES MasterContentItems([Id]),
	CONSTRAINT [FK_ContentPages_ToRecordStatusCode] FOREIGN KEY ([StatusCode]) REFERENCES [RecordStatusCodes]([StatusCode]),
)
