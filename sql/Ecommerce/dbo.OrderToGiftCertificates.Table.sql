/****** Object:  Table [dbo].[OrderToGiftCertificates]    Script Date: 6/25/2016 2:12:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderToGiftCertificates]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[OrderToGiftCertificates](
	[IdOrder] [int] NOT NULL,
	[IdGiftCertificate] [int] NOT NULL,
	[Amount] [money] NOT NULL,
	[PAmount] [money] NULL,
	[NPAmount] [money] NULL,
 CONSTRAINT [PK_GiftCertificatesInOrder] PRIMARY KEY CLUSTERED 
(
	[IdOrder] DESC,
	[IdGiftCertificate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderToGiftCertificateToGiftCertificate]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderToGiftCertificates]'))
ALTER TABLE [dbo].[OrderToGiftCertificates]  WITH CHECK ADD  CONSTRAINT [FK_OrderToGiftCertificateToGiftCertificate] FOREIGN KEY([IdGiftCertificate])
REFERENCES [dbo].[GiftCertificates] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderToGiftCertificateToGiftCertificate]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderToGiftCertificates]'))
ALTER TABLE [dbo].[OrderToGiftCertificates] CHECK CONSTRAINT [FK_OrderToGiftCertificateToGiftCertificate]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderToGiftCertificateToOrder]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderToGiftCertificates]'))
ALTER TABLE [dbo].[OrderToGiftCertificates]  WITH CHECK ADD  CONSTRAINT [FK_OrderToGiftCertificateToOrder] FOREIGN KEY([IdOrder])
REFERENCES [dbo].[Orders] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderToGiftCertificateToOrder]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderToGiftCertificates]'))
ALTER TABLE [dbo].[OrderToGiftCertificates] CHECK CONSTRAINT [FK_OrderToGiftCertificateToOrder]
GO
