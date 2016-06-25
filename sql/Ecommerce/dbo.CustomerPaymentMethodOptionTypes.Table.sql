/****** Object:  Table [dbo].[CustomerPaymentMethodOptionTypes]    Script Date: 6/25/2016 2:10:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethodOptionTypes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CustomerPaymentMethodOptionTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[IdObjectType] [int] NOT NULL,
	[IdFieldType] [int] NOT NULL,
	[IdLookup] [int] NULL,
	[DefaultValue] [nvarchar](250) NULL,
 CONSTRAINT [PK_CustomerPaymentMethodOptionTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_CustomerPaymentMethodOptionTypes_Name]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethodOptionTypes]') AND name = N'IX_CustomerPaymentMethodOptionTypes_Name')
CREATE NONCLUSTERED INDEX [IX_CustomerPaymentMethodOptionTypes_Name] ON [dbo].[CustomerPaymentMethodOptionTypes]
(
	[Name] ASC
)
INCLUDE ( 	[Id],
	[IdFieldType]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_UQ_NameTypeCustomerPaymentOption]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethodOptionTypes]') AND name = N'IX_UQ_NameTypeCustomerPaymentOption')
CREATE UNIQUE NONCLUSTERED INDEX [IX_UQ_NameTypeCustomerPaymentOption] ON [dbo].[CustomerPaymentMethodOptionTypes]
(
	[Name] ASC,
	[IdObjectType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerPaymentMethodOptionTypes_FieldType]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethodOptionTypes]'))
ALTER TABLE [dbo].[CustomerPaymentMethodOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_CustomerPaymentMethodOptionTypes_FieldType] FOREIGN KEY([IdFieldType])
REFERENCES [dbo].[FieldTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerPaymentMethodOptionTypes_FieldType]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethodOptionTypes]'))
ALTER TABLE [dbo].[CustomerPaymentMethodOptionTypes] CHECK CONSTRAINT [FK_CustomerPaymentMethodOptionTypes_FieldType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerPaymentMethodOptionTypes_Lookup]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethodOptionTypes]'))
ALTER TABLE [dbo].[CustomerPaymentMethodOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_CustomerPaymentMethodOptionTypes_Lookup] FOREIGN KEY([IdLookup])
REFERENCES [dbo].[Lookups] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerPaymentMethodOptionTypes_Lookup]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethodOptionTypes]'))
ALTER TABLE [dbo].[CustomerPaymentMethodOptionTypes] CHECK CONSTRAINT [FK_CustomerPaymentMethodOptionTypes_Lookup]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerPaymentMethodOptionTypes_PaymentMethod]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethodOptionTypes]'))
ALTER TABLE [dbo].[CustomerPaymentMethodOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_CustomerPaymentMethodOptionTypes_PaymentMethod] FOREIGN KEY([IdObjectType])
REFERENCES [dbo].[PaymentMethods] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerPaymentMethodOptionTypes_PaymentMethod]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethodOptionTypes]'))
ALTER TABLE [dbo].[CustomerPaymentMethodOptionTypes] CHECK CONSTRAINT [FK_CustomerPaymentMethodOptionTypes_PaymentMethod]
GO
