/****** Object:  Table [dbo].[OrderAddressOptionValues]    Script Date: 6/25/2016 2:11:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderAddressOptionValues]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[OrderAddressOptionValues](
	[IdOptionType] [int] NOT NULL,
	[IdOrderAddress] [int] NOT NULL,
	[Value] [nvarchar](250) NULL,
 CONSTRAINT [PK_OrderAddressOptionValues] PRIMARY KEY CLUSTERED 
(
	[IdOrderAddress] ASC,
	[IdOptionType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_OrderAddressOptionValues_Value]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OrderAddressOptionValues]') AND name = N'IX_OrderAddressOptionValues_Value')
CREATE NONCLUSTERED INDEX [IX_OrderAddressOptionValues_Value] ON [dbo].[OrderAddressOptionValues]
(
	[Value] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_OrderAddressOptionValues_ValuesSearch]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OrderAddressOptionValues]') AND name = N'IX_OrderAddressOptionValues_ValuesSearch')
CREATE NONCLUSTERED INDEX [IX_OrderAddressOptionValues_ValuesSearch] ON [dbo].[OrderAddressOptionValues]
(
	[Value] ASC,
	[IdOptionType] ASC
)
INCLUDE ( 	[IdOrderAddress]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderAddressOptionValuesToAddressOptionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderAddressOptionValues]'))
ALTER TABLE [dbo].[OrderAddressOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_OrderAddressOptionValuesToAddressOptionType] FOREIGN KEY([IdOptionType])
REFERENCES [dbo].[AddressOptionTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderAddressOptionValuesToAddressOptionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderAddressOptionValues]'))
ALTER TABLE [dbo].[OrderAddressOptionValues] CHECK CONSTRAINT [FK_OrderAddressOptionValuesToAddressOptionType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderAddressOptionValuesToOrderAddress]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderAddressOptionValues]'))
ALTER TABLE [dbo].[OrderAddressOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_OrderAddressOptionValuesToOrderAddress] FOREIGN KEY([IdOrderAddress])
REFERENCES [dbo].[OrderAddresses] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderAddressOptionValuesToOrderAddress]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderAddressOptionValues]'))
ALTER TABLE [dbo].[OrderAddressOptionValues] CHECK CONSTRAINT [FK_OrderAddressOptionValuesToOrderAddress]
GO
