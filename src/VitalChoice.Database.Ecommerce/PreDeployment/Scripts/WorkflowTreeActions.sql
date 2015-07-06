IF OBJECT_ID(N'dbo.WorkflowTreeActions', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[WorkflowTreeActions]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[IdTree] INT NOT NULL, 
		[IdExecutor] INT NOT NULL, 
		CONSTRAINT [FK_WorkflowTreeActions_ToTree] FOREIGN KEY ([IdTree]) REFERENCES [WorkflowTrees]([Id]), 
		CONSTRAINT [FK_WorkflowTreeActions_ToExecutor] FOREIGN KEY ([IdExecutor]) REFERENCES [WorkflowExecutors]([Id])
	)
END