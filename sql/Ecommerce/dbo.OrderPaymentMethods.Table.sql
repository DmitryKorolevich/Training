/****** Object:  Table [dbo].[OrderPaymentMethods]    Script Date: 6/25/2016 2:12:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderPaymentMethods]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[OrderPaymentMethods](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdObjectType] [int] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[DateEdited] [datetime2](7) NOT NULL,
	[IdEditedBy] [int] NULL,
	[StatusCode] [int] NOT NULL,
	[IdAddress] [int] NULL,
 CONSTRAINT [PK_OrderPaymentMethods] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderPaymentMethodsToStatus]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderPaymentMethods]'))
ALTER TABLE [dbo].[OrderPaymentMethods]  WITH CHECK ADD  CONSTRAINT [FK_OrderPaymentMethodsToStatus] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderPaymentMethodsToStatus]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderPaymentMethods]'))
ALTER TABLE [dbo].[OrderPaymentMethods] CHECK CONSTRAINT [FK_OrderPaymentMethodsToStatus]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderPaymentMethodsToUser]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderPaymentMethods]'))
ALTER TABLE [dbo].[OrderPaymentMethods]  WITH CHECK ADD  CONSTRAINT [FK_OrderPaymentMethodsToUser] FOREIGN KEY([IdEditedBy])
REFERENCES [dbo].[Users] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderPaymentMethodsToUser]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderPaymentMethods]'))
ALTER TABLE [dbo].[OrderPaymentMethods] CHECK CONSTRAINT [FK_OrderPaymentMethodsToUser]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderPaymentMethodToPaymentMethod]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderPaymentMethods]'))
ALTER TABLE [dbo].[OrderPaymentMethods]  WITH CHECK ADD  CONSTRAINT [FK_OrderPaymentMethodToPaymentMethod] FOREIGN KEY([IdObjectType])
REFERENCES [dbo].[PaymentMethods] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderPaymentMethodToPaymentMethod]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderPaymentMethods]'))
ALTER TABLE [dbo].[OrderPaymentMethods] CHECK CONSTRAINT [FK_OrderPaymentMethodToPaymentMethod]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PaymentMethodToAddress]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderPaymentMethods]'))
ALTER TABLE [dbo].[OrderPaymentMethods]  WITH CHECK ADD  CONSTRAINT [FK_PaymentMethodToAddress] FOREIGN KEY([IdAddress])
REFERENCES [dbo].[OrderAddresses] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PaymentMethodToAddress]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderPaymentMethods]'))
ALTER TABLE [dbo].[OrderPaymentMethods] CHECK CONSTRAINT [FK_PaymentMethodToAddress]
GO
