/****** Object:  Table [dbo].[CustomerPaymentMethodValues]    Script Date: 6/25/2016 2:10:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethodValues]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CustomerPaymentMethodValues](
	[IdCustomerPaymentMethod] [int] NOT NULL,
	[IdOptionType] [int] NOT NULL,
	[Value] [nvarchar](250) NULL,
 CONSTRAINT [PK_CustomerPaymentMethodValues] PRIMARY KEY CLUSTERED 
(
	[IdCustomerPaymentMethod] ASC,
	[IdOptionType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_CustomerPaymentMethodValues_Value]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethodValues]') AND name = N'IX_CustomerPaymentMethodValues_Value')
CREATE NONCLUSTERED INDEX [IX_CustomerPaymentMethodValues_Value] ON [dbo].[CustomerPaymentMethodValues]
(
	[Value] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_CustomerPaymentMethodValues_ValuesSearch]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethodValues]') AND name = N'IX_CustomerPaymentMethodValues_ValuesSearch')
CREATE NONCLUSTERED INDEX [IX_CustomerPaymentMethodValues_ValuesSearch] ON [dbo].[CustomerPaymentMethodValues]
(
	[Value] ASC,
	[IdOptionType] ASC
)
INCLUDE ( 	[IdCustomerPaymentMethod]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerPaymentMethodValues_CustomerPaymentMethodOptionTypes]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethodValues]'))
ALTER TABLE [dbo].[CustomerPaymentMethodValues]  WITH CHECK ADD  CONSTRAINT [FK_CustomerPaymentMethodValues_CustomerPaymentMethodOptionTypes] FOREIGN KEY([IdOptionType])
REFERENCES [dbo].[CustomerPaymentMethodOptionTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerPaymentMethodValues_CustomerPaymentMethodOptionTypes]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethodValues]'))
ALTER TABLE [dbo].[CustomerPaymentMethodValues] CHECK CONSTRAINT [FK_CustomerPaymentMethodValues_CustomerPaymentMethodOptionTypes]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerPaymentMethodValues_CustomerPaymentMethods]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethodValues]'))
ALTER TABLE [dbo].[CustomerPaymentMethodValues]  WITH CHECK ADD  CONSTRAINT [FK_CustomerPaymentMethodValues_CustomerPaymentMethods] FOREIGN KEY([IdCustomerPaymentMethod])
REFERENCES [dbo].[CustomerPaymentMethods] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerPaymentMethodValues_CustomerPaymentMethods]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethodValues]'))
ALTER TABLE [dbo].[CustomerPaymentMethodValues] CHECK CONSTRAINT [FK_CustomerPaymentMethodValues_CustomerPaymentMethods]
GO
