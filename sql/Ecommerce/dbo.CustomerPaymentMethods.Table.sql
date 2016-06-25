/****** Object:  Table [dbo].[CustomerPaymentMethods]    Script Date: 6/25/2016 2:10:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethods]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CustomerPaymentMethods](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdObjectType] [int] NOT NULL,
	[IdAddress] [int] NULL,
	[IdCustomer] [int] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[DateEdited] [datetime2](7) NOT NULL,
	[IdEditedBy] [int] NULL,
	[StatusCode] [int] NOT NULL,
 CONSTRAINT [PK_CustomerPaymentMethods] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_IdCustomer]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethods]') AND name = N'IX_IdCustomer')
CREATE NONCLUSTERED INDEX [IX_IdCustomer] ON [dbo].[CustomerPaymentMethods]
(
	[IdCustomer] ASC
)
INCLUDE ( 	[DateCreated],
	[DateEdited],
	[Id],
	[IdAddress],
	[IdEditedBy],
	[IdObjectType],
	[StatusCode]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerPaymentMethods_Addresses_EditedBy]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethods]'))
ALTER TABLE [dbo].[CustomerPaymentMethods]  WITH CHECK ADD  CONSTRAINT [FK_CustomerPaymentMethods_Addresses_EditedBy] FOREIGN KEY([IdAddress])
REFERENCES [dbo].[Addresses] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerPaymentMethods_Addresses_EditedBy]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethods]'))
ALTER TABLE [dbo].[CustomerPaymentMethods] CHECK CONSTRAINT [FK_CustomerPaymentMethods_Addresses_EditedBy]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerPaymentMethods_Customers]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethods]'))
ALTER TABLE [dbo].[CustomerPaymentMethods]  WITH CHECK ADD  CONSTRAINT [FK_CustomerPaymentMethods_Customers] FOREIGN KEY([IdCustomer])
REFERENCES [dbo].[Customers] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerPaymentMethods_Customers]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethods]'))
ALTER TABLE [dbo].[CustomerPaymentMethods] CHECK CONSTRAINT [FK_CustomerPaymentMethods_Customers]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerPaymentMethods_PaymentMethod]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethods]'))
ALTER TABLE [dbo].[CustomerPaymentMethods]  WITH CHECK ADD  CONSTRAINT [FK_CustomerPaymentMethods_PaymentMethod] FOREIGN KEY([IdObjectType])
REFERENCES [dbo].[PaymentMethods] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerPaymentMethods_PaymentMethod]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethods]'))
ALTER TABLE [dbo].[CustomerPaymentMethods] CHECK CONSTRAINT [FK_CustomerPaymentMethods_PaymentMethod]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerPaymentMethods_RecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethods]'))
ALTER TABLE [dbo].[CustomerPaymentMethods]  WITH CHECK ADD  CONSTRAINT [FK_CustomerPaymentMethods_RecordStatusCode] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerPaymentMethods_RecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethods]'))
ALTER TABLE [dbo].[CustomerPaymentMethods] CHECK CONSTRAINT [FK_CustomerPaymentMethods_RecordStatusCode]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerPaymentMethods_Users_EditedBy]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethods]'))
ALTER TABLE [dbo].[CustomerPaymentMethods]  WITH CHECK ADD  CONSTRAINT [FK_CustomerPaymentMethods_Users_EditedBy] FOREIGN KEY([IdEditedBy])
REFERENCES [dbo].[Users] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerPaymentMethods_Users_EditedBy]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethods]'))
ALTER TABLE [dbo].[CustomerPaymentMethods] CHECK CONSTRAINT [FK_CustomerPaymentMethods_Users_EditedBy]
GO
