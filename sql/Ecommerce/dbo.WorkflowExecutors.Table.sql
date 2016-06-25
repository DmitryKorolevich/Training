/****** Object:  Table [dbo].[WorkflowExecutors]    Script Date: 6/25/2016 2:13:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WorkflowExecutors]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[WorkflowExecutors](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[ImplementationType] [nvarchar](250) NOT NULL,
	[ActionType] [int] NOT NULL,
	[IdOwnedTree] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ_ExecutorTypeAndNameForTree]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[WorkflowExecutors]') AND name = N'UQ_ExecutorTypeAndNameForTree')
CREATE UNIQUE NONCLUSTERED INDEX [UQ_ExecutorTypeAndNameForTree] ON [dbo].[WorkflowExecutors]
(
	[Name] ASC,
	[ImplementationType] ASC,
	[IdOwnedTree] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkflowExecutorTree]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkflowExecutors]'))
ALTER TABLE [dbo].[WorkflowExecutors]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowExecutorTree] FOREIGN KEY([IdOwnedTree])
REFERENCES [dbo].[WorkflowTrees] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkflowExecutorTree]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkflowExecutors]'))
ALTER TABLE [dbo].[WorkflowExecutors] CHECK CONSTRAINT [FK_WorkflowExecutorTree]
GO
