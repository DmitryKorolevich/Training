﻿CREATE TABLE [dbo].[ContentCategories]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ContentItemId] INT NULL, 
	[MasterContentItemId] INT NOT NULL, 
	[Name] NVARCHAR(250) NOT NULL, 
    [ParentId] INT NULL, 
    [StatusCode] INT NOT NULL DEFAULT 1, 
	[Url] NVARCHAR(250) NOT NULL UNIQUE, 
    CONSTRAINT [FK_ContentCategories_ToContentCategory] FOREIGN KEY ([ParentId]) REFERENCES [ContentCategories]([Id]), 
    CONSTRAINT [FK_ContentCategories_ToMasterContentItem] FOREIGN KEY ([MasterContentItemId]) REFERENCES [MasterContentItems]([Id]),
    CONSTRAINT [FK_ContentCategories_ToContentItem] FOREIGN KEY ([ContentItemId]) REFERENCES [ContentItems]([Id]),
	CONSTRAINT [FK_ContentCategories_ToRecordStatusCode] FOREIGN KEY ([StatusCode]) REFERENCES [RecordStatusCodes]([StatusCode])
)

GO

CREATE INDEX [IX_ContentCategories_Url] ON [dbo].[ContentCategories] ([Url]) WITH (FILLFACTOR = 80); 

GO

CREATE INDEX [IX_ContentCategories_MasterContentItemId] ON [dbo].[ContentCategories] ([MasterContentItemId]) WITH (FILLFACTOR = 80); 

GO

CREATE INDEX [IX_ContentCategories_ParentId] ON [dbo].[ContentCategories] ([ParentId]) WITH (FILLFACTOR = 80); 

GO

--CREATE TRIGGER to resolve what type of inserted/update category vs parent id