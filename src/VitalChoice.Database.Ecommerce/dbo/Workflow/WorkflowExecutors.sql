CREATE TABLE [dbo].[WorkflowExecutors]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [ImplementationType] NVARCHAR(250) NOT NULL UNIQUE, 
    [ActionType] INT NOT NULL
)