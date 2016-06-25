/****** Object:  Table [dbo].[WorkflowResolverPaths]    Script Date: 6/25/2016 2:13:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WorkflowResolverPaths]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[WorkflowResolverPaths](
	[Name] [nvarchar](50) NOT NULL,
	[Path] [int] NOT NULL,
	[IdResolver] [int] NOT NULL,
	[IdExecutor] [int] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_WorkflowResolverPath] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkflowResolverPath_ToExecutor]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkflowResolverPaths]'))
ALTER TABLE [dbo].[WorkflowResolverPaths]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowResolverPath_ToExecutor] FOREIGN KEY([IdExecutor])
REFERENCES [dbo].[WorkflowExecutors] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkflowResolverPath_ToExecutor]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkflowResolverPaths]'))
ALTER TABLE [dbo].[WorkflowResolverPaths] CHECK CONSTRAINT [FK_WorkflowResolverPath_ToExecutor]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkflowResolverPath_ToResolver]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkflowResolverPaths]'))
ALTER TABLE [dbo].[WorkflowResolverPaths]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowResolverPath_ToResolver] FOREIGN KEY([IdResolver])
REFERENCES [dbo].[WorkflowExecutors] ([Id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkflowResolverPath_ToResolver]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkflowResolverPaths]'))
ALTER TABLE [dbo].[WorkflowResolverPaths] CHECK CONSTRAINT [FK_WorkflowResolverPath_ToResolver]
GO
