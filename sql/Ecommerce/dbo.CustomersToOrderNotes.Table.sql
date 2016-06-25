/****** Object:  Table [dbo].[CustomersToOrderNotes]    Script Date: 6/25/2016 2:10:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomersToOrderNotes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CustomersToOrderNotes](
	[IdCustomer] [int] NOT NULL,
	[IdOrderNote] [int] NOT NULL,
 CONSTRAINT [PK_CustomerToOrderNotes] PRIMARY KEY CLUSTERED 
(
	[IdCustomer] ASC,
	[IdOrderNote] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerToOrderNotes_Customers]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomersToOrderNotes]'))
ALTER TABLE [dbo].[CustomersToOrderNotes]  WITH CHECK ADD  CONSTRAINT [FK_CustomerToOrderNotes_Customers] FOREIGN KEY([IdCustomer])
REFERENCES [dbo].[Customers] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerToOrderNotes_Customers]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomersToOrderNotes]'))
ALTER TABLE [dbo].[CustomersToOrderNotes] CHECK CONSTRAINT [FK_CustomerToOrderNotes_Customers]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerToOrderNotes_OrderNotes]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomersToOrderNotes]'))
ALTER TABLE [dbo].[CustomersToOrderNotes]  WITH CHECK ADD  CONSTRAINT [FK_CustomerToOrderNotes_OrderNotes] FOREIGN KEY([IdOrderNote])
REFERENCES [dbo].[OrderNotes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerToOrderNotes_OrderNotes]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomersToOrderNotes]'))
ALTER TABLE [dbo].[CustomersToOrderNotes] CHECK CONSTRAINT [FK_CustomerToOrderNotes_OrderNotes]
GO
