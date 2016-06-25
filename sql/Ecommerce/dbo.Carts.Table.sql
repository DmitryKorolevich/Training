/****** Object:  Table [dbo].[Carts]    Script Date: 6/25/2016 2:10:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Carts]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Carts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CartUid] [uniqueidentifier] NOT NULL,
	[IdCustomer] [int] NULL,
	[IdOrder] [int] NULL,
	[ShipDelayDate] [datetime] NULL,
	[ShippingUpgradeP] [int] NULL,
	[ShippingUpgradeNP] [int] NULL,
	[DiscountCode] [nvarchar](250) NULL,
 CONSTRAINT [PK_Carts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_CARTUID_UQ]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Carts]') AND name = N'IX_CARTUID_UQ')
CREATE UNIQUE NONCLUSTERED INDEX [IX_CARTUID_UQ] ON [dbo].[Carts]
(
	[CartUid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
/****** Object:  Index [IX_CUSTOMERID]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Carts]') AND name = N'IX_CUSTOMERID')
CREATE NONCLUSTERED INDEX [IX_CUSTOMERID] ON [dbo].[Carts]
(
	[IdCustomer] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CartToCustomer]') AND parent_object_id = OBJECT_ID(N'[dbo].[Carts]'))
ALTER TABLE [dbo].[Carts]  WITH CHECK ADD  CONSTRAINT [FK_CartToCustomer] FOREIGN KEY([IdCustomer])
REFERENCES [dbo].[Customers] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CartToCustomer]') AND parent_object_id = OBJECT_ID(N'[dbo].[Carts]'))
ALTER TABLE [dbo].[Carts] CHECK CONSTRAINT [FK_CartToCustomer]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CartToOrder]') AND parent_object_id = OBJECT_ID(N'[dbo].[Carts]'))
ALTER TABLE [dbo].[Carts]  WITH CHECK ADD  CONSTRAINT [FK_CartToOrder] FOREIGN KEY([IdOrder])
REFERENCES [dbo].[Orders] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CartToOrder]') AND parent_object_id = OBJECT_ID(N'[dbo].[Carts]'))
ALTER TABLE [dbo].[Carts] CHECK CONSTRAINT [FK_CartToOrder]
GO
