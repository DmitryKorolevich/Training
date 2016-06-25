/****** Object:  Table [dbo].[OrderToSkus]    Script Date: 6/25/2016 2:12:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderToSkus]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[OrderToSkus](
	[IdOrder] [int] NOT NULL,
	[IdSku] [int] NOT NULL,
	[Amount] [money] NOT NULL,
	[Quantity] [int] NOT NULL,
 CONSTRAINT [PK_SkusOrdered] PRIMARY KEY CLUSTERED 
(
	[IdOrder] DESC,
	[IdSku] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_OrderToSkus_IdOrder]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OrderToSkus]') AND name = N'IX_OrderToSkus_IdOrder')
CREATE NONCLUSTERED INDEX [IX_OrderToSkus_IdOrder] ON [dbo].[OrderToSkus]
(
	[IdOrder] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderToSkuToOrder]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderToSkus]'))
ALTER TABLE [dbo].[OrderToSkus]  WITH CHECK ADD  CONSTRAINT [FK_OrderToSkuToOrder] FOREIGN KEY([IdOrder])
REFERENCES [dbo].[Orders] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderToSkuToOrder]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderToSkus]'))
ALTER TABLE [dbo].[OrderToSkus] CHECK CONSTRAINT [FK_OrderToSkuToOrder]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderToSkuToSku]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderToSkus]'))
ALTER TABLE [dbo].[OrderToSkus]  WITH CHECK ADD  CONSTRAINT [FK_OrderToSkuToSku] FOREIGN KEY([IdSku])
REFERENCES [dbo].[Skus] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderToSkuToSku]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderToSkus]'))
ALTER TABLE [dbo].[OrderToSkus] CHECK CONSTRAINT [FK_OrderToSkuToSku]
GO
