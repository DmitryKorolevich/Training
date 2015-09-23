IF OBJECT_ID(N'dbo.WorkflowExecutors', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[WorkflowExecutors]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY,
		[Name] NVARCHAR(50) NOT NULL,
		[ImplementationType] NVARCHAR(250) NOT NULL,
		[ActionType] INT NOT NULL
	)
END

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

IF OBJECT_ID(N'dbo.WorkflowTrees', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[WorkflowTrees]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[Name] NVARCHAR(50) NOT NULL UNIQUE, 
		[ImplementationType] NVARCHAR(250) NOT NULL,
	)
END

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

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[WorkflowTrees]') AND name = N'IX_WorkflowTrees_Name')
	CREATE INDEX [IX_WorkflowTrees_Name] ON [dbo].[WorkflowTrees] ([Name]) INCLUDE(Id, [ImplementationType]);

IF OBJECT_ID(N'dbo.WorkflowActionDependencies', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[WorkflowActionDependencies]
	(
		[IdParent] INT NOT NULL, 
		[IdDependent] INT NOT NULL, 
		CONSTRAINT [PK_WorkflowActionDependency] PRIMARY KEY (IdParent, IdDependent),
		CONSTRAINT [FK_WorkflowActionDependencyToWorkflowParentAction] FOREIGN KEY (IdParent) REFERENCES [WorkflowExecutors] (Id),
		CONSTRAINT [FK_WorkflowActionDependencyToWorkflowDependentAction] FOREIGN KEY (IdDependent) REFERENCES [WorkflowExecutors] (Id)
	)
END

IF EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[WorkflowTreeActions]') AND name = N'Id')
BEGIN

	DECLARE @pk_name NVARCHAR(250)

	SELECT @pk_name = name FROM sys.key_constraints WHERE type = 'PK' AND OBJECT_NAME(parent_object_id) = N'WorkflowTreeActions'

	EXEC('ALTER TABLE [WorkflowTreeActions] DROP CONSTRAINT ' + @pk_name)

	ALTER TABLE [WorkflowTreeActions]
	DROP COLUMN Id

	ALTER TABLE [WorkflowTreeActions]
	DROP CONSTRAINT [FK_WorkflowTreeActions_ToExecutor]

	ALTER TABLE [WorkflowTreeActions]
	DROP CONSTRAINT [FK_WorkflowTreeActions_ToTree]

	ALTER TABLE [WorkflowTreeActions]
	ADD CONSTRAINT PK_WorkflowTreeAction PRIMARY KEY (IdTree, IdExecutor)

	ALTER TABLE [WorkflowTreeActions] 
	ADD CONSTRAINT [FK_WorkflowTreeActions_ToTree] FOREIGN KEY ([IdTree]) REFERENCES [WorkflowTrees]([Id]) ON DELETE CASCADE, 
		CONSTRAINT [FK_WorkflowTreeActions_ToExecutor] FOREIGN KEY ([IdExecutor]) REFERENCES [WorkflowExecutors]([Id]) ON DELETE CASCADE

	SELECT @pk_name = name FROM sys.key_constraints WHERE type = 'PK' AND OBJECT_NAME(parent_object_id) = N'WorkflowResolverPaths'

	EXEC('ALTER TABLE [WorkflowResolverPaths] DROP CONSTRAINT ' + @pk_name)

	ALTER TABLE [WorkflowResolverPaths]
	DROP COLUMN Id

	ALTER TABLE [WorkflowResolverPaths]
	DROP CONSTRAINT [FK_WorkflowResolverPaths_ToExecutor]

	ALTER TABLE [WorkflowResolverPaths]
	DROP CONSTRAINT [FK_WorkflowResolverPaths_ToResolver]

	ALTER TABLE [WorkflowResolverPaths]
	ADD Id INT NOT NULL IDENTITY
	CONSTRAINT PK_WorkflowResolverPath PRIMARY KEY (Id)

	ALTER TABLE [WorkflowResolverPaths]
	ADD CONSTRAINT [FK_WorkflowResolverPath_ToResolver] FOREIGN KEY ([IdResolver]) REFERENCES [WorkflowExecutors]([Id]) ON DELETE CASCADE, 
		CONSTRAINT [FK_WorkflowResolverPath_ToExecutor] FOREIGN KEY ([IdExecutor]) REFERENCES [WorkflowExecutors]([Id])


	ALTER TABLE WorkflowExecutors
	ADD CONSTRAINT UQ_ExecutorName UNIQUE (Name)


	ALTER TABLE [WorkflowActionDependencies]
	DROP CONSTRAINT [FK_WorkflowActionDependencyToWorkflowParentAction],
		 CONSTRAINT	[FK_WorkflowActionDependencyToWorkflowDependentAction]

	ALTER TABLE [WorkflowActionDependencies]
	ADD CONSTRAINT [FK_WorkflowActionDependencyToWorkflowParentAction] FOREIGN KEY (IdParent) REFERENCES [WorkflowExecutors] (Id) ON DELETE CASCADE,
		CONSTRAINT [FK_WorkflowActionDependencyToWorkflowDependentAction] FOREIGN KEY (IdDependent) REFERENCES [WorkflowExecutors] (Id)
END

IF OBJECT_ID(N'dbo.WorkflowActionAggregations', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[WorkflowActionAggregations]
	(
		[IdParent] INT NOT NULL, 
		[IdAggregate] INT NOT NULL, 
		CONSTRAINT [PK_WorkflowActionAggregation] PRIMARY KEY (IdParent, IdAggregate),
		CONSTRAINT [FK_WorkflowActionAggregationToWorkflowParentAction] FOREIGN KEY (IdParent) REFERENCES [WorkflowExecutors] (Id) ON DELETE CASCADE,
		CONSTRAINT [FK_WorkflowActionAggregationToWorkflowAggregateAction] FOREIGN KEY (IdAggregate) REFERENCES [WorkflowExecutors] (Id)
	)
END