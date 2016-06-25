/****** Object:  Table [dbo].[OrderShippingPackages]    Script Date: 6/25/2016 2:12:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderShippingPackages]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[OrderShippingPackages](
	[IdOrder] [int] NOT NULL,
	[IdSku] [int] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[POrderType] [int] NULL,
	[ShipMethodFreightCarrier] [nvarchar](500) NOT NULL,
	[ShipMethodFreightService] [nvarchar](500) NOT NULL,
	[ShippedDate] [datetime2](7) NOT NULL,
	[TrackingNumber] [nvarchar](500) NOT NULL,
	[UPSServiceCode] [nvarchar](500) NOT NULL,
	[IdWarehouse] [int] NOT NULL,
 CONSTRAINT [PK_OrderShippingPackages] PRIMARY KEY CLUSTERED 
(
	[IdOrder] DESC,
	[IdSku] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_OrderShippingPackage_IdOrder]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OrderShippingPackages]') AND name = N'IX_OrderShippingPackage_IdOrder')
CREATE NONCLUSTERED INDEX [IX_OrderShippingPackage_IdOrder] ON [dbo].[OrderShippingPackages]
(
	[IdOrder] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
/****** Object:  Index [IX_OrderShippingPackage_IdOrder_POrderType]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OrderShippingPackages]') AND name = N'IX_OrderShippingPackage_IdOrder_POrderType')
CREATE NONCLUSTERED INDEX [IX_OrderShippingPackage_IdOrder_POrderType] ON [dbo].[OrderShippingPackages]
(
	[IdOrder] DESC,
	[POrderType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
/****** Object:  Index [IX_OrderShippingPackage_ShippedDate]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OrderShippingPackages]') AND name = N'IX_OrderShippingPackage_ShippedDate')
CREATE NONCLUSTERED INDEX [IX_OrderShippingPackage_ShippedDate] ON [dbo].[OrderShippingPackages]
(
	[ShippedDate] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderShippingPackageToOrder]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderShippingPackages]'))
ALTER TABLE [dbo].[OrderShippingPackages]  WITH CHECK ADD  CONSTRAINT [FK_OrderShippingPackageToOrder] FOREIGN KEY([IdOrder])
REFERENCES [dbo].[Orders] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderShippingPackageToOrder]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderShippingPackages]'))
ALTER TABLE [dbo].[OrderShippingPackages] CHECK CONSTRAINT [FK_OrderShippingPackageToOrder]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderShippingPackageToSku]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderShippingPackages]'))
ALTER TABLE [dbo].[OrderShippingPackages]  WITH CHECK ADD  CONSTRAINT [FK_OrderShippingPackageToSku] FOREIGN KEY([IdSku])
REFERENCES [dbo].[Skus] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderShippingPackageToSku]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderShippingPackages]'))
ALTER TABLE [dbo].[OrderShippingPackages] CHECK CONSTRAINT [FK_OrderShippingPackageToSku]
GO
