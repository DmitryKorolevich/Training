/****** Object:  Table [dbo].[InventoryCategories]    Script Date: 6/25/2016 2:11:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InventoryCategories]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[InventoryCategories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[ParentId] [int] NULL,
	[StatusCode] [int] NOT NULL,
	[Order] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_InventoryCategories_ParentId]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[InventoryCategories]') AND name = N'IX_InventoryCategories_ParentId')
CREATE NONCLUSTERED INDEX [IX_InventoryCategories_ParentId] ON [dbo].[InventoryCategories]
(
	[ParentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Inventory__Statu__251C81ED]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[InventoryCategories] ADD  DEFAULT ((1)) FOR [StatusCode]
END

GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventoryCategories_RecordStatusCodes]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventoryCategories]'))
ALTER TABLE [dbo].[InventoryCategories]  WITH CHECK ADD  CONSTRAINT [FK_InventoryCategories_RecordStatusCodes] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventoryCategories_RecordStatusCodes]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventoryCategories]'))
ALTER TABLE [dbo].[InventoryCategories] CHECK CONSTRAINT [FK_InventoryCategories_RecordStatusCodes]
GO
