/****** Object:  Table [dbo].[OrderToPromos]    Script Date: 6/25/2016 2:12:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderToPromos]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[OrderToPromos](
	[IdOrder] [int] NOT NULL,
	[IdSku] [int] NOT NULL,
	[IdPromo] [int] NULL,
	[Amount] [money] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Disabled] [bit] NOT NULL,
 CONSTRAINT [PK_PromoOrdered] PRIMARY KEY CLUSTERED 
(
	[IdOrder] DESC,
	[IdSku] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_OrderToPromos_IdOrder]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OrderToPromos]') AND name = N'IX_OrderToPromos_IdOrder')
CREATE NONCLUSTERED INDEX [IX_OrderToPromos_IdOrder] ON [dbo].[OrderToPromos]
(
	[IdOrder] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderToPromoToOrder]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderToPromos]'))
ALTER TABLE [dbo].[OrderToPromos]  WITH CHECK ADD  CONSTRAINT [FK_OrderToPromoToOrder] FOREIGN KEY([IdOrder])
REFERENCES [dbo].[Orders] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderToPromoToOrder]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderToPromos]'))
ALTER TABLE [dbo].[OrderToPromos] CHECK CONSTRAINT [FK_OrderToPromoToOrder]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderToPromoToPromo]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderToPromos]'))
ALTER TABLE [dbo].[OrderToPromos]  WITH CHECK ADD  CONSTRAINT [FK_OrderToPromoToPromo] FOREIGN KEY([IdPromo])
REFERENCES [dbo].[Promotions] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderToPromoToPromo]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderToPromos]'))
ALTER TABLE [dbo].[OrderToPromos] CHECK CONSTRAINT [FK_OrderToPromoToPromo]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderToPromoToSku]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderToPromos]'))
ALTER TABLE [dbo].[OrderToPromos]  WITH CHECK ADD  CONSTRAINT [FK_OrderToPromoToSku] FOREIGN KEY([IdSku])
REFERENCES [dbo].[Skus] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderToPromoToSku]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderToPromos]'))
ALTER TABLE [dbo].[OrderToPromos] CHECK CONSTRAINT [FK_OrderToPromoToSku]
GO
