IF OBJECT_ID(N'dbo.WorkflowTrees', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[WorkflowTrees]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[Name] NVARCHAR(50) NOT NULL UNIQUE, 
		[ImplementationType] NVARCHAR(250) NOT NULL,
	)
END