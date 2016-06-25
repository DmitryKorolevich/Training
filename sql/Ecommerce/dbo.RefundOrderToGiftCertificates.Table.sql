/****** Object:  Table [dbo].[RefundOrderToGiftCertificates]    Script Date: 6/25/2016 2:13:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RefundOrderToGiftCertificates]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[RefundOrderToGiftCertificates](
	[IdRefundOrder] [int] NOT NULL,
	[IdOrder] [int] NOT NULL,
	[IdGiftCertificate] [int] NOT NULL,
	[Amount] [money] NOT NULL,
 CONSTRAINT [PK_RefundOrderToGiftCertificates] PRIMARY KEY CLUSTERED 
(
	[IdRefundOrder] DESC,
	[IdOrder] DESC,
	[IdGiftCertificate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_RefundOrderToGiftCertificates_IdRefundOrder]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[RefundOrderToGiftCertificates]') AND name = N'IX_RefundOrderToGiftCertificates_IdRefundOrder')
CREATE NONCLUSTERED INDEX [IX_RefundOrderToGiftCertificates_IdRefundOrder] ON [dbo].[RefundOrderToGiftCertificates]
(
	[IdRefundOrder] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_RefundOrderToGiftCertificateToOrder]') AND parent_object_id = OBJECT_ID(N'[dbo].[RefundOrderToGiftCertificates]'))
ALTER TABLE [dbo].[RefundOrderToGiftCertificates]  WITH CHECK ADD  CONSTRAINT [FK_RefundOrderToGiftCertificateToOrder] FOREIGN KEY([IdRefundOrder])
REFERENCES [dbo].[Orders] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_RefundOrderToGiftCertificateToOrder]') AND parent_object_id = OBJECT_ID(N'[dbo].[RefundOrderToGiftCertificates]'))
ALTER TABLE [dbo].[RefundOrderToGiftCertificates] CHECK CONSTRAINT [FK_RefundOrderToGiftCertificateToOrder]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_RefundOrderToGiftCertificateToOrderToGiftCertificate]') AND parent_object_id = OBJECT_ID(N'[dbo].[RefundOrderToGiftCertificates]'))
ALTER TABLE [dbo].[RefundOrderToGiftCertificates]  WITH CHECK ADD  CONSTRAINT [FK_RefundOrderToGiftCertificateToOrderToGiftCertificate] FOREIGN KEY([IdOrder], [IdGiftCertificate])
REFERENCES [dbo].[OrderToGiftCertificates] ([IdOrder], [IdGiftCertificate])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_RefundOrderToGiftCertificateToOrderToGiftCertificate]') AND parent_object_id = OBJECT_ID(N'[dbo].[RefundOrderToGiftCertificates]'))
ALTER TABLE [dbo].[RefundOrderToGiftCertificates] CHECK CONSTRAINT [FK_RefundOrderToGiftCertificateToOrderToGiftCertificate]
GO
