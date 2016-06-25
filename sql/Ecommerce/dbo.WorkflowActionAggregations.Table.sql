/****** Object:  Table [dbo].[WorkflowActionAggregations]    Script Date: 6/25/2016 2:13:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WorkflowActionAggregations]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[WorkflowActionAggregations](
	[IdParent] [int] NOT NULL,
	[IdAggregate] [int] NOT NULL,
 CONSTRAINT [PK_WorkflowActionAggregation] PRIMARY KEY CLUSTERED 
(
	[IdParent] ASC,
	[IdAggregate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkflowActionAggregationToWorkflowAggregateAction]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkflowActionAggregations]'))
ALTER TABLE [dbo].[WorkflowActionAggregations]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowActionAggregationToWorkflowAggregateAction] FOREIGN KEY([IdAggregate])
REFERENCES [dbo].[WorkflowExecutors] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkflowActionAggregationToWorkflowAggregateAction]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkflowActionAggregations]'))
ALTER TABLE [dbo].[WorkflowActionAggregations] CHECK CONSTRAINT [FK_WorkflowActionAggregationToWorkflowAggregateAction]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkflowActionAggregationToWorkflowParentAction]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkflowActionAggregations]'))
ALTER TABLE [dbo].[WorkflowActionAggregations]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowActionAggregationToWorkflowParentAction] FOREIGN KEY([IdParent])
REFERENCES [dbo].[WorkflowExecutors] ([Id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkflowActionAggregationToWorkflowParentAction]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkflowActionAggregations]'))
ALTER TABLE [dbo].[WorkflowActionAggregations] CHECK CONSTRAINT [FK_WorkflowActionAggregationToWorkflowParentAction]
GO
