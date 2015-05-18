IF OBJECT_ID(N'dbo.WorkflowResolverPaths', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[WorkflowResolverPaths]
	(
		[Id] INT NOT NULL PRIMARY KEY, 
		[Name] NVARCHAR(50) NOT NULL, 
		[Path] INT NOT NULL, 
		[IdResolver] INT NOT NULL, 
		[IdExecutor] INT NOT NULL, 
		CONSTRAINT [FK_WorkflowResolverPaths_ToResolver] FOREIGN KEY ([IdResolver]) REFERENCES [WorkflowExecutors]([Id]), 
		CONSTRAINT [FK_WorkflowResolverPaths_ToExecutor] FOREIGN KEY ([IdExecutor]) REFERENCES [WorkflowExecutors]([Id])
	)
END