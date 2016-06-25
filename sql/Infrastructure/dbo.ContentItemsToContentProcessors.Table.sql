/****** Object:  Table [dbo].[ContentItemsToContentProcessors]    Script Date: 6/25/2016 3:40:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ContentItemsToContentProcessors]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ContentItemsToContentProcessors](
	[ContentItemId] [int] NOT NULL,
	[ContentItemProcessorId] [int] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_ContentItemsToContentProcessors] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_ContentItemsToContentProcessors_ContentItemId]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ContentItemsToContentProcessors]') AND name = N'IX_ContentItemsToContentProcessors_ContentItemId')
CREATE NONCLUSTERED INDEX [IX_ContentItemsToContentProcessors_ContentItemId] ON [dbo].[ContentItemsToContentProcessors]
(
	[ContentItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentItemsToContentProcessors_ContentItems]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentItemsToContentProcessors]'))
ALTER TABLE [dbo].[ContentItemsToContentProcessors]  WITH CHECK ADD  CONSTRAINT [FK_ContentItemsToContentProcessors_ContentItems] FOREIGN KEY([ContentItemId])
REFERENCES [dbo].[ContentItems] ([Id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentItemsToContentProcessors_ContentItems]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentItemsToContentProcessors]'))
ALTER TABLE [dbo].[ContentItemsToContentProcessors] CHECK CONSTRAINT [FK_ContentItemsToContentProcessors_ContentItems]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentItemsToContentProcessors_ContentProcessors]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentItemsToContentProcessors]'))
ALTER TABLE [dbo].[ContentItemsToContentProcessors]  WITH CHECK ADD  CONSTRAINT [FK_ContentItemsToContentProcessors_ContentProcessors] FOREIGN KEY([ContentItemProcessorId])
REFERENCES [dbo].[ContentProcessors] ([Id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentItemsToContentProcessors_ContentProcessors]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentItemsToContentProcessors]'))
ALTER TABLE [dbo].[ContentItemsToContentProcessors] CHECK CONSTRAINT [FK_ContentItemsToContentProcessors_ContentProcessors]
GO
