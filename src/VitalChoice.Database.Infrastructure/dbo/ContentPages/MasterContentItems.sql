CREATE TABLE [dbo].[MasterContentItems]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(250) NOT NULL UNIQUE, 
    [TypeId] INT NOT NULL, 
    [Template] NVARCHAR(MAX) NOT NULL, 
    [Created] DATETIME NOT NULL DEFAULT GETDATE(), 
    [Updated] DATETIME NOT NULL DEFAULT GETDATE(), 
    [StatusCode] INT NOT NULL DEFAULT 1,	
    CONSTRAINT [FK_MasterContentItems_ToContentTypes] FOREIGN KEY (TypeId) REFERENCES ContentTypes([Id]),
)


