/****** Object:  Table [dbo].[AffiliatePayments]    Script Date: 6/25/2016 2:10:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AffiliatePayments]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AffiliatePayments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdAffiliate] [int] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[Amount] [money] NOT NULL,
 CONSTRAINT [PK_AffiliatePayments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AffiliatePaymentToAffiliate]') AND parent_object_id = OBJECT_ID(N'[dbo].[AffiliatePayments]'))
ALTER TABLE [dbo].[AffiliatePayments]  WITH CHECK ADD  CONSTRAINT [FK_AffiliatePaymentToAffiliate] FOREIGN KEY([IdAffiliate])
REFERENCES [dbo].[Affiliates] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AffiliatePaymentToAffiliate]') AND parent_object_id = OBJECT_ID(N'[dbo].[AffiliatePayments]'))
ALTER TABLE [dbo].[AffiliatePayments] CHECK CONSTRAINT [FK_AffiliatePaymentToAffiliate]
GO
