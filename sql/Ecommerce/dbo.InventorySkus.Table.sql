/****** Object:  Table [dbo].[InventorySkus]    Script Date: 6/25/2016 2:11:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InventorySkus]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[InventorySkus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[DateEdited] [datetime2](7) NOT NULL,
	[IdEditedBy] [int] NULL,
	[StatusCode] [int] NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Description] [nvarchar](250) NOT NULL,
	[IdInventorySkuCategory] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Inventory__DateC__2704CA5F]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[InventorySkus] ADD  DEFAULT (sysdatetime()) FOR [DateCreated]
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Inventory__DateE__27F8EE98]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[InventorySkus] ADD  DEFAULT (sysdatetime()) FOR [DateEdited]
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Inventory__Statu__28ED12D1]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[InventorySkus] ADD  DEFAULT ((1)) FOR [StatusCode]
END

GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventorySku_ToInventorySkuCategory]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventorySkus]'))
ALTER TABLE [dbo].[InventorySkus]  WITH CHECK ADD  CONSTRAINT [FK_InventorySku_ToInventorySkuCategory] FOREIGN KEY([IdInventorySkuCategory])
REFERENCES [dbo].[InventorySkuCategories] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventorySku_ToInventorySkuCategory]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventorySkus]'))
ALTER TABLE [dbo].[InventorySkus] CHECK CONSTRAINT [FK_InventorySku_ToInventorySkuCategory]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventorySkus_ToRecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventorySkus]'))
ALTER TABLE [dbo].[InventorySkus]  WITH CHECK ADD  CONSTRAINT [FK_InventorySkus_ToRecordStatusCode] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventorySkus_ToRecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventorySkus]'))
ALTER TABLE [dbo].[InventorySkus] CHECK CONSTRAINT [FK_InventorySkus_ToRecordStatusCode]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventorySkusToUser]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventorySkus]'))
ALTER TABLE [dbo].[InventorySkus]  WITH CHECK ADD  CONSTRAINT [FK_InventorySkusToUser] FOREIGN KEY([IdEditedBy])
REFERENCES [dbo].[Users] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventorySkusToUser]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventorySkus]'))
ALTER TABLE [dbo].[InventorySkus] CHECK CONSTRAINT [FK_InventorySkusToUser]
GO
