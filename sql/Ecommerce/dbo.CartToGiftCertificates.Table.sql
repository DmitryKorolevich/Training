/****** Object:  Table [dbo].[CartToGiftCertificates]    Script Date: 6/25/2016 2:10:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CartToGiftCertificates]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CartToGiftCertificates](
	[IdCart] [int] NOT NULL,
	[IdGiftCertificate] [int] NOT NULL,
	[Amount] [money] NOT NULL,
 CONSTRAINT [PK_CartToGiftCertificates] PRIMARY KEY CLUSTERED 
(
	[IdCart] ASC,
	[IdGiftCertificate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CartGiftCertificatesToCart]') AND parent_object_id = OBJECT_ID(N'[dbo].[CartToGiftCertificates]'))
ALTER TABLE [dbo].[CartToGiftCertificates]  WITH CHECK ADD  CONSTRAINT [FK_CartGiftCertificatesToCart] FOREIGN KEY([IdCart])
REFERENCES [dbo].[Carts] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CartGiftCertificatesToCart]') AND parent_object_id = OBJECT_ID(N'[dbo].[CartToGiftCertificates]'))
ALTER TABLE [dbo].[CartToGiftCertificates] CHECK CONSTRAINT [FK_CartGiftCertificatesToCart]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CartGiftCertificatesToGiftCertificate]') AND parent_object_id = OBJECT_ID(N'[dbo].[CartToGiftCertificates]'))
ALTER TABLE [dbo].[CartToGiftCertificates]  WITH CHECK ADD  CONSTRAINT [FK_CartGiftCertificatesToGiftCertificate] FOREIGN KEY([IdGiftCertificate])
REFERENCES [dbo].[GiftCertificates] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CartGiftCertificatesToGiftCertificate]') AND parent_object_id = OBJECT_ID(N'[dbo].[CartToGiftCertificates]'))
ALTER TABLE [dbo].[CartToGiftCertificates] CHECK CONSTRAINT [FK_CartGiftCertificatesToGiftCertificate]
GO
