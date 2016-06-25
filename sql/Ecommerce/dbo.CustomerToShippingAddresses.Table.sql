/****** Object:  Table [dbo].[CustomerToShippingAddresses]    Script Date: 6/25/2016 2:10:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomerToShippingAddresses]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CustomerToShippingAddresses](
	[IdCustomer] [int] NOT NULL,
	[IdAddress] [int] NOT NULL,
 CONSTRAINT [PK_CustomerToShippingAddresses] PRIMARY KEY CLUSTERED 
(
	[IdCustomer] ASC,
	[IdAddress] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_IdAddress]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CustomerToShippingAddresses]') AND name = N'IX_IdAddress')
CREATE NONCLUSTERED INDEX [IX_IdAddress] ON [dbo].[CustomerToShippingAddresses]
(
	[IdAddress] ASC
)
INCLUDE ( 	[IdCustomer]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerToShippingToCustomer]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerToShippingAddresses]'))
ALTER TABLE [dbo].[CustomerToShippingAddresses]  WITH CHECK ADD  CONSTRAINT [FK_CustomerToShippingToCustomer] FOREIGN KEY([IdCustomer])
REFERENCES [dbo].[Customers] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerToShippingToCustomer]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerToShippingAddresses]'))
ALTER TABLE [dbo].[CustomerToShippingAddresses] CHECK CONSTRAINT [FK_CustomerToShippingToCustomer]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerToShippingToShippingAddress]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerToShippingAddresses]'))
ALTER TABLE [dbo].[CustomerToShippingAddresses]  WITH CHECK ADD  CONSTRAINT [FK_CustomerToShippingToShippingAddress] FOREIGN KEY([IdAddress])
REFERENCES [dbo].[Addresses] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerToShippingToShippingAddress]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerToShippingAddresses]'))
ALTER TABLE [dbo].[CustomerToShippingAddresses] CHECK CONSTRAINT [FK_CustomerToShippingToShippingAddress]
GO
