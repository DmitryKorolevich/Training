/****** Object:  Table [dbo].[MasterContentItemsToContentProcessors]    Script Date: 6/25/2016 3:40:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MasterContentItemsToContentProcessors]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MasterContentItemsToContentProcessors](
	[MasterContentItemId] [int] NOT NULL,
	[ContentProcessorId] [int] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_MasterContentItemsToContentProcessors] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_ContentItemsToContentProcessors_MasterContentItemId]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[MasterContentItemsToContentProcessors]') AND name = N'IX_ContentItemsToContentProcessors_MasterContentItemId')
CREATE NONCLUSTERED INDEX [IX_ContentItemsToContentProcessors_MasterContentItemId] ON [dbo].[MasterContentItemsToContentProcessors]
(
	[MasterContentItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MasterContentItemsToContentProcessors_ContentProcessors]') AND parent_object_id = OBJECT_ID(N'[dbo].[MasterContentItemsToContentProcessors]'))
ALTER TABLE [dbo].[MasterContentItemsToContentProcessors]  WITH CHECK ADD  CONSTRAINT [FK_MasterContentItemsToContentProcessors_ContentProcessors] FOREIGN KEY([ContentProcessorId])
REFERENCES [dbo].[ContentProcessors] ([Id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MasterContentItemsToContentProcessors_ContentProcessors]') AND parent_object_id = OBJECT_ID(N'[dbo].[MasterContentItemsToContentProcessors]'))
ALTER TABLE [dbo].[MasterContentItemsToContentProcessors] CHECK CONSTRAINT [FK_MasterContentItemsToContentProcessors_ContentProcessors]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MasterContentItemsToContentProcessors_MasterContentItems]') AND parent_object_id = OBJECT_ID(N'[dbo].[MasterContentItemsToContentProcessors]'))
ALTER TABLE [dbo].[MasterContentItemsToContentProcessors]  WITH CHECK ADD  CONSTRAINT [FK_MasterContentItemsToContentProcessors_MasterContentItems] FOREIGN KEY([MasterContentItemId])
REFERENCES [dbo].[MasterContentItems] ([Id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MasterContentItemsToContentProcessors_MasterContentItems]') AND parent_object_id = OBJECT_ID(N'[dbo].[MasterContentItemsToContentProcessors]'))
ALTER TABLE [dbo].[MasterContentItemsToContentProcessors] CHECK CONSTRAINT [FK_MasterContentItemsToContentProcessors_MasterContentItems]
GO
