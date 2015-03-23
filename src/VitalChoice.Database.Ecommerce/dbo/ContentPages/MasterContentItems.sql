CREATE TABLE [dbo].[MasterContentItems]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(250) NOT NULL, 
    [Type] INT NOT NULL, 
    [Template] NVARCHAR(MAX) NOT NULL
)
