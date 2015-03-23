CREATE TABLE [dbo].[Recipes]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Url] NVARCHAR(250) NOT NULL, 
    [Name] NVARCHAR(250) NOT NULL, 
    [ContentItemId] INT NOT NULL, 
    [MasterContentItemId] INT NOT NULL, 
	[StatusCode] INT NOT NULL DEFAULT 1,
    CONSTRAINT [FK_Recipes_ToContentItem] FOREIGN KEY (ContentItemId) REFERENCES ContentItems([Id]), 
    CONSTRAINT [FK_Recipes_ToMasterContentItem] FOREIGN KEY (MasterContentItemId) REFERENCES MasterContentItems([Id]),
	CONSTRAINT [FK_Recipes_ToRecordStatusCode] FOREIGN KEY ([StatusCode]) REFERENCES [RecordStatusCodes]([StatusCode]),
)
