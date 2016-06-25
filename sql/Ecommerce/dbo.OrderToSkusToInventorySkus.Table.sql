/****** Object:  Table [dbo].[OrderToSkusToInventorySkus]    Script Date: 6/25/2016 2:12:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderToSkusToInventorySkus]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[OrderToSkusToInventorySkus](
	[IdOrder] [int] NOT NULL,
	[IdSku] [int] NOT NULL,
	[IdInventorySku] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
 CONSTRAINT [PK_OrderToSkusToInventorySkus] PRIMARY KEY CLUSTERED 
(
	[IdOrder] ASC,
	[IdSku] ASC,
	[IdInventorySku] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_OrderToSkusToInventorySkus_IdInventorySku]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OrderToSkusToInventorySkus]') AND name = N'IX_OrderToSkusToInventorySkus_IdInventorySku')
CREATE NONCLUSTERED INDEX [IX_OrderToSkusToInventorySkus_IdInventorySku] ON [dbo].[OrderToSkusToInventorySkus]
(
	[IdInventorySku] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
/****** Object:  Index [IX_OrderToSkusToInventorySkus_IdOrder_IdSku]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OrderToSkusToInventorySkus]') AND name = N'IX_OrderToSkusToInventorySkus_IdOrder_IdSku')
CREATE NONCLUSTERED INDEX [IX_OrderToSkusToInventorySkus_IdOrder_IdSku] ON [dbo].[OrderToSkusToInventorySkus]
(
	[IdOrder] DESC,
	[IdSku] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__OrderToSk__Quant__2EA5EC27]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[OrderToSkusToInventorySkus] ADD  DEFAULT ((1)) FOR [Quantity]
END

GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderToSkuToInventorySku_ToInventorySku]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderToSkusToInventorySkus]'))
ALTER TABLE [dbo].[OrderToSkusToInventorySkus]  WITH CHECK ADD  CONSTRAINT [FK_OrderToSkuToInventorySku_ToInventorySku] FOREIGN KEY([IdInventorySku])
REFERENCES [dbo].[InventorySkus] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderToSkuToInventorySku_ToInventorySku]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderToSkusToInventorySkus]'))
ALTER TABLE [dbo].[OrderToSkusToInventorySkus] CHECK CONSTRAINT [FK_OrderToSkuToInventorySku_ToInventorySku]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderToSkuToInventorySku_ToSku]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderToSkusToInventorySkus]'))
ALTER TABLE [dbo].[OrderToSkusToInventorySkus]  WITH CHECK ADD  CONSTRAINT [FK_OrderToSkuToInventorySku_ToSku] FOREIGN KEY([IdOrder], [IdSku])
REFERENCES [dbo].[OrderToSkus] ([IdOrder], [IdSku])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderToSkuToInventorySku_ToSku]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderToSkusToInventorySkus]'))
ALTER TABLE [dbo].[OrderToSkusToInventorySkus] CHECK CONSTRAINT [FK_OrderToSkuToInventorySku_ToSku]
GO
