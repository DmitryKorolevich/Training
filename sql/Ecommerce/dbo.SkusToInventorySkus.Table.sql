/****** Object:  Table [dbo].[SkusToInventorySkus]    Script Date: 6/25/2016 2:13:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SkusToInventorySkus]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[SkusToInventorySkus](
	[IdSku] [int] NOT NULL,
	[IdInventorySku] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
 CONSTRAINT [PK_SkusToInventorySkus] PRIMARY KEY CLUSTERED 
(
	[IdSku] ASC,
	[IdInventorySku] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_SkusToInventorySkus_IdInventorySku]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SkusToInventorySkus]') AND name = N'IX_SkusToInventorySkus_IdInventorySku')
CREATE NONCLUSTERED INDEX [IX_SkusToInventorySkus_IdInventorySku] ON [dbo].[SkusToInventorySkus]
(
	[IdInventorySku] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
/****** Object:  Index [IX_SkusToInventorySkus_IdSku]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SkusToInventorySkus]') AND name = N'IX_SkusToInventorySkus_IdSku')
CREATE NONCLUSTERED INDEX [IX_SkusToInventorySkus_IdSku] ON [dbo].[SkusToInventorySkus]
(
	[IdSku] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__SkusToInv__Quant__42ACE4D4]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[SkusToInventorySkus] ADD  DEFAULT ((1)) FOR [Quantity]
END

GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SkusToInventorySku_ToInventorySku]') AND parent_object_id = OBJECT_ID(N'[dbo].[SkusToInventorySkus]'))
ALTER TABLE [dbo].[SkusToInventorySkus]  WITH CHECK ADD  CONSTRAINT [FK_SkusToInventorySku_ToInventorySku] FOREIGN KEY([IdInventorySku])
REFERENCES [dbo].[InventorySkus] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SkusToInventorySku_ToInventorySku]') AND parent_object_id = OBJECT_ID(N'[dbo].[SkusToInventorySkus]'))
ALTER TABLE [dbo].[SkusToInventorySkus] CHECK CONSTRAINT [FK_SkusToInventorySku_ToInventorySku]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SkusToInventorySku_ToSku]') AND parent_object_id = OBJECT_ID(N'[dbo].[SkusToInventorySkus]'))
ALTER TABLE [dbo].[SkusToInventorySkus]  WITH CHECK ADD  CONSTRAINT [FK_SkusToInventorySku_ToSku] FOREIGN KEY([IdSku])
REFERENCES [dbo].[Skus] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SkusToInventorySku_ToSku]') AND parent_object_id = OBJECT_ID(N'[dbo].[SkusToInventorySkus]'))
ALTER TABLE [dbo].[SkusToInventorySkus] CHECK CONSTRAINT [FK_SkusToInventorySku_ToSku]
GO
