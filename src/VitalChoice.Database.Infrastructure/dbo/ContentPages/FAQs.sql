CREATE TABLE [dbo].[FAQs]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Url] NVARCHAR(250) NOT NULL, 
    [Name] NVARCHAR(250) NOT NULL, 
    [FileUrl] NVARCHAR(250) NULL, 
    [ContentItemId] INT NOT NULL, 
    [MasterContentItemId] INT NOT NULL, 
	[StatusCode] INT NOT NULL DEFAULT 1,
    CONSTRAINT [FK_FAQs_ToContentItem] FOREIGN KEY (ContentItemId) REFERENCES ContentItems([Id]), 
    CONSTRAINT [FK_FAQs_ToMasterContentItem] FOREIGN KEY (MasterContentItemId) REFERENCES MasterContentItems([Id]),
	CONSTRAINT [FK_FAQs_ToRecordStatusCode] FOREIGN KEY ([StatusCode]) REFERENCES [RecordStatusCodes]([StatusCode]),
)

GO

CREATE INDEX [IX_FAQs_Url] ON [dbo].[FAQs] ([Url]) WITH (FILLFACTOR = 80); 

GO

CREATE INDEX [IX_FAQs_MasterContentItemId] ON [dbo].[FAQs] ([MasterContentItemId]) WITH (FILLFACTOR = 80); 

GO