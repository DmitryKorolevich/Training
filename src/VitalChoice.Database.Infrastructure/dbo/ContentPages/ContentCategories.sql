CREATE TABLE [dbo].[ContentCategories]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ContentItemId] INT NOT NULL, 
	[MasterContentItemId] INT NOT NULL, 
	[Name] NVARCHAR(250) NOT NULL, 
    [ParentId] INT NULL, 
    [StatusCode] NCHAR(1) NOT NULL DEFAULT 'N', 
	[Url] NVARCHAR(250) NOT NULL UNIQUE, 
    CONSTRAINT [FK_ContentCategories_ToContentCategory] FOREIGN KEY ([ParentId]) REFERENCES [ContentCategories]([Id]), 
    CONSTRAINT [FK_ContentCategories_ToMasterContentItem] FOREIGN KEY ([MasterContentItemId]) REFERENCES [MasterContentItems]([Id]),
	CONSTRAINT [FK_ContentCategories_ToRecordStatusCode] FOREIGN KEY ([StatusCode]) REFERENCES [RecordStatusCodes]([StatusCode])
)
GO
--CREATE TRIGGER to resolve what type of inserted/update category vs parent id