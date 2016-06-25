/****** Object:  Table [dbo].[OrderPaymentMethodOptionValues]    Script Date: 6/25/2016 2:12:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderPaymentMethodOptionValues]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[OrderPaymentMethodOptionValues](
	[IdOptionType] [int] NOT NULL,
	[IdOrderPaymentMethod] [int] NOT NULL,
	[Value] [nvarchar](250) NULL,
 CONSTRAINT [PK_OrderPaymentMethodOptionValues] PRIMARY KEY CLUSTERED 
(
	[IdOrderPaymentMethod] ASC,
	[IdOptionType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_OrderPaymentMethodOptionValues_Value]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OrderPaymentMethodOptionValues]') AND name = N'IX_OrderPaymentMethodOptionValues_Value')
CREATE NONCLUSTERED INDEX [IX_OrderPaymentMethodOptionValues_Value] ON [dbo].[OrderPaymentMethodOptionValues]
(
	[Value] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_OrderPaymentMethodOptionValues_ValuesSearch]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OrderPaymentMethodOptionValues]') AND name = N'IX_OrderPaymentMethodOptionValues_ValuesSearch')
CREATE NONCLUSTERED INDEX [IX_OrderPaymentMethodOptionValues_ValuesSearch] ON [dbo].[OrderPaymentMethodOptionValues]
(
	[Value] ASC,
	[IdOptionType] ASC
)
INCLUDE ( 	[IdOrderPaymentMethod]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderPaymentMethodOptionValuesToCustomerPaymentMethodOptionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderPaymentMethodOptionValues]'))
ALTER TABLE [dbo].[OrderPaymentMethodOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_OrderPaymentMethodOptionValuesToCustomerPaymentMethodOptionType] FOREIGN KEY([IdOptionType])
REFERENCES [dbo].[CustomerPaymentMethodOptionTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderPaymentMethodOptionValuesToCustomerPaymentMethodOptionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderPaymentMethodOptionValues]'))
ALTER TABLE [dbo].[OrderPaymentMethodOptionValues] CHECK CONSTRAINT [FK_OrderPaymentMethodOptionValuesToCustomerPaymentMethodOptionType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderPaymentMethodOptionValuesToOrderPaymentMethod]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderPaymentMethodOptionValues]'))
ALTER TABLE [dbo].[OrderPaymentMethodOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_OrderPaymentMethodOptionValuesToOrderPaymentMethod] FOREIGN KEY([IdOrderPaymentMethod])
REFERENCES [dbo].[OrderPaymentMethods] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderPaymentMethodOptionValuesToOrderPaymentMethod]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderPaymentMethodOptionValues]'))
ALTER TABLE [dbo].[OrderPaymentMethodOptionValues] CHECK CONSTRAINT [FK_OrderPaymentMethodOptionValuesToOrderPaymentMethod]
GO
