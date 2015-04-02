CREATE TABLE [dbo].[ContentTypes]
(
	[Id] INT NOT NULL, 
    [Name] NVARCHAR(250) NOT NULL, 
    [DefaultMasterContentItemId] INT NULL,
    CONSTRAINT [PK_ContentTypes] PRIMARY KEY ([Id])
)
