/****** Object:  Table [dbo].[OrderNotesToCustomerTypes]    Script Date: 6/25/2016 2:12:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderNotesToCustomerTypes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[OrderNotesToCustomerTypes](
	[IdOrderNote] [int] NOT NULL,
	[IdCustomerType] [int] NOT NULL,
 CONSTRAINT [PK_OrderNotesToCustomerTypes] PRIMARY KEY CLUSTERED 
(
	[IdOrderNote] ASC,
	[IdCustomerType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderNotesToCustomerTypes_CustomerTypes]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderNotesToCustomerTypes]'))
ALTER TABLE [dbo].[OrderNotesToCustomerTypes]  WITH CHECK ADD  CONSTRAINT [FK_OrderNotesToCustomerTypes_CustomerTypes] FOREIGN KEY([IdCustomerType])
REFERENCES [dbo].[CustomerTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderNotesToCustomerTypes_CustomerTypes]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderNotesToCustomerTypes]'))
ALTER TABLE [dbo].[OrderNotesToCustomerTypes] CHECK CONSTRAINT [FK_OrderNotesToCustomerTypes_CustomerTypes]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderNotesToCustomerTypes_OrderNotes]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderNotesToCustomerTypes]'))
ALTER TABLE [dbo].[OrderNotesToCustomerTypes]  WITH CHECK ADD  CONSTRAINT [FK_OrderNotesToCustomerTypes_OrderNotes] FOREIGN KEY([IdOrderNote])
REFERENCES [dbo].[OrderNotes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderNotesToCustomerTypes_OrderNotes]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderNotesToCustomerTypes]'))
ALTER TABLE [dbo].[OrderNotesToCustomerTypes] CHECK CONSTRAINT [FK_OrderNotesToCustomerTypes_OrderNotes]
GO
