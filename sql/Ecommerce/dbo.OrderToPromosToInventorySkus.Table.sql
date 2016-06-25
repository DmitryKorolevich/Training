/****** Object:  Table [dbo].[OrderToPromosToInventorySkus]    Script Date: 6/25/2016 2:12:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderToPromosToInventorySkus]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[OrderToPromosToInventorySkus](
	[IdOrder] [int] NOT NULL,
	[IdSku] [int] NOT NULL,
	[IdInventorySku] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
 CONSTRAINT [PK_OrderToPromosToInventorySkus] PRIMARY KEY CLUSTERED 
(
	[IdOrder] ASC,
	[IdSku] ASC,
	[IdInventorySku] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_OrderToPromosToInventorySkus_IdInventorySku]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OrderToPromosToInventorySkus]') AND name = N'IX_OrderToPromosToInventorySkus_IdInventorySku')
CREATE NONCLUSTERED INDEX [IX_OrderToPromosToInventorySkus_IdInventorySku] ON [dbo].[OrderToPromosToInventorySkus]
(
	[IdInventorySku] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
/****** Object:  Index [IX_OrderToPromosToInventorySkus_IdOrder_IdSku]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OrderToPromosToInventorySkus]') AND name = N'IX_OrderToPromosToInventorySkus_IdOrder_IdSku')
CREATE NONCLUSTERED INDEX [IX_OrderToPromosToInventorySkus_IdOrder_IdSku] ON [dbo].[OrderToPromosToInventorySkus]
(
	[IdOrder] DESC,
	[IdSku] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__OrderToPr__Quant__2DB1C7EE]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[OrderToPromosToInventorySkus] ADD  DEFAULT ((1)) FOR [Quantity]
END

GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderToPromoToInventorySku_ToInventorySku]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderToPromosToInventorySkus]'))
ALTER TABLE [dbo].[OrderToPromosToInventorySkus]  WITH CHECK ADD  CONSTRAINT [FK_OrderToPromoToInventorySku_ToInventorySku] FOREIGN KEY([IdInventorySku])
REFERENCES [dbo].[InventorySkus] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderToPromoToInventorySku_ToInventorySku]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderToPromosToInventorySkus]'))
ALTER TABLE [dbo].[OrderToPromosToInventorySkus] CHECK CONSTRAINT [FK_OrderToPromoToInventorySku_ToInventorySku]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderToPromoToInventorySku_ToSku]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderToPromosToInventorySkus]'))
ALTER TABLE [dbo].[OrderToPromosToInventorySkus]  WITH CHECK ADD  CONSTRAINT [FK_OrderToPromoToInventorySku_ToSku] FOREIGN KEY([IdOrder], [IdSku])
REFERENCES [dbo].[OrderToPromos] ([IdOrder], [IdSku])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderToPromoToInventorySku_ToSku]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderToPromosToInventorySkus]'))
ALTER TABLE [dbo].[OrderToPromosToInventorySkus] CHECK CONSTRAINT [FK_OrderToPromoToInventorySku_ToSku]
GO
