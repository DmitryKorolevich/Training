/****** Object:  Table [dbo].[CustomersToPaymentMethods]    Script Date: 6/25/2016 2:10:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomersToPaymentMethods]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CustomersToPaymentMethods](
	[IdCustomer] [int] NOT NULL,
	[IdPaymentMethod] [int] NOT NULL,
 CONSTRAINT [PK_CustomerToPaymentMethods] PRIMARY KEY CLUSTERED 
(
	[IdCustomer] ASC,
	[IdPaymentMethod] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerToPaymentMethods_Customers]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomersToPaymentMethods]'))
ALTER TABLE [dbo].[CustomersToPaymentMethods]  WITH CHECK ADD  CONSTRAINT [FK_CustomerToPaymentMethods_Customers] FOREIGN KEY([IdCustomer])
REFERENCES [dbo].[Customers] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerToPaymentMethods_Customers]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomersToPaymentMethods]'))
ALTER TABLE [dbo].[CustomersToPaymentMethods] CHECK CONSTRAINT [FK_CustomerToPaymentMethods_Customers]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerToPaymentMethods_PaymentMethods]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomersToPaymentMethods]'))
ALTER TABLE [dbo].[CustomersToPaymentMethods]  WITH CHECK ADD  CONSTRAINT [FK_CustomerToPaymentMethods_PaymentMethods] FOREIGN KEY([IdPaymentMethod])
REFERENCES [dbo].[PaymentMethods] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerToPaymentMethods_PaymentMethods]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomersToPaymentMethods]'))
ALTER TABLE [dbo].[CustomersToPaymentMethods] CHECK CONSTRAINT [FK_CustomerToPaymentMethods_PaymentMethods]
GO
