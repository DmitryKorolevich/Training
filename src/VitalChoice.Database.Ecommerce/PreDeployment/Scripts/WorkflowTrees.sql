IF OBJECT_ID(N'dbo.WorkflowTrees', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[WorkflowTrees]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[Name] NVARCHAR(50) NOT NULL UNIQUE, 
		[ImplementationType] NVARCHAR(250) NOT NULL,
	)
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[WorkflowTrees]') AND name = N'IX_WorkflowTrees_Name')
	CREATE INDEX [IX_WorkflowTrees_Name] ON [dbo].[WorkflowTrees] ([Name]) INCLUDE(Id, [ImplementationType]);