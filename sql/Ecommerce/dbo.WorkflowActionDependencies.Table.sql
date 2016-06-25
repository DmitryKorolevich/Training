/****** Object:  Table [dbo].[WorkflowActionDependencies]    Script Date: 6/25/2016 2:13:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WorkflowActionDependencies]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[WorkflowActionDependencies](
	[IdParent] [int] NOT NULL,
	[IdDependent] [int] NOT NULL,
 CONSTRAINT [PK_WorkflowActionDependency] PRIMARY KEY CLUSTERED 
(
	[IdParent] ASC,
	[IdDependent] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkflowActionDependencyToWorkflowDependentAction]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkflowActionDependencies]'))
ALTER TABLE [dbo].[WorkflowActionDependencies]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowActionDependencyToWorkflowDependentAction] FOREIGN KEY([IdDependent])
REFERENCES [dbo].[WorkflowExecutors] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkflowActionDependencyToWorkflowDependentAction]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkflowActionDependencies]'))
ALTER TABLE [dbo].[WorkflowActionDependencies] CHECK CONSTRAINT [FK_WorkflowActionDependencyToWorkflowDependentAction]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkflowActionDependencyToWorkflowParentAction]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkflowActionDependencies]'))
ALTER TABLE [dbo].[WorkflowActionDependencies]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowActionDependencyToWorkflowParentAction] FOREIGN KEY([IdParent])
REFERENCES [dbo].[WorkflowExecutors] ([Id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkflowActionDependencyToWorkflowParentAction]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkflowActionDependencies]'))
ALTER TABLE [dbo].[WorkflowActionDependencies] CHECK CONSTRAINT [FK_WorkflowActionDependencyToWorkflowParentAction]
GO
