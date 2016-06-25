/****** Object:  Table [dbo].[PaymentMethodsToCustomerTypes]    Script Date: 6/25/2016 2:12:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PaymentMethodsToCustomerTypes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PaymentMethodsToCustomerTypes](
	[IdPaymentMethod] [int] NOT NULL,
	[IdCustomerType] [int] NOT NULL,
 CONSTRAINT [PK_PaymentMethodsToCustomerTypes] PRIMARY KEY CLUSTERED 
(
	[IdPaymentMethod] ASC,
	[IdCustomerType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PaymentMethodsToCustomerTypes_CustomerTypes]') AND parent_object_id = OBJECT_ID(N'[dbo].[PaymentMethodsToCustomerTypes]'))
ALTER TABLE [dbo].[PaymentMethodsToCustomerTypes]  WITH CHECK ADD  CONSTRAINT [FK_PaymentMethodsToCustomerTypes_CustomerTypes] FOREIGN KEY([IdCustomerType])
REFERENCES [dbo].[CustomerTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PaymentMethodsToCustomerTypes_CustomerTypes]') AND parent_object_id = OBJECT_ID(N'[dbo].[PaymentMethodsToCustomerTypes]'))
ALTER TABLE [dbo].[PaymentMethodsToCustomerTypes] CHECK CONSTRAINT [FK_PaymentMethodsToCustomerTypes_CustomerTypes]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PaymentMethodsToCustomerTypes_PaymentMethods]') AND parent_object_id = OBJECT_ID(N'[dbo].[PaymentMethodsToCustomerTypes]'))
ALTER TABLE [dbo].[PaymentMethodsToCustomerTypes]  WITH CHECK ADD  CONSTRAINT [FK_PaymentMethodsToCustomerTypes_PaymentMethods] FOREIGN KEY([IdPaymentMethod])
REFERENCES [dbo].[PaymentMethods] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PaymentMethodsToCustomerTypes_PaymentMethods]') AND parent_object_id = OBJECT_ID(N'[dbo].[PaymentMethodsToCustomerTypes]'))
ALTER TABLE [dbo].[PaymentMethodsToCustomerTypes] CHECK CONSTRAINT [FK_PaymentMethodsToCustomerTypes_PaymentMethods]
GO
