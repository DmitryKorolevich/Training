CREATE TABLE [dbo].[ContentProcessors]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Type] NVARCHAR(250) NOT NULL, 
    [Name] NVARCHAR(250) NOT NULL, 
    [Description] NVARCHAR(250) NULL
)
