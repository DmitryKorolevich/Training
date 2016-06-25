/****** Object:  Table [dbo].[PromotionsToBuySkus]    Script Date: 6/25/2016 2:12:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PromotionsToBuySkus]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PromotionsToBuySkus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdPromotion] [int] NOT NULL,
	[IdSku] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromotionsToBuySkus_ToPromotion]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromotionsToBuySkus]'))
ALTER TABLE [dbo].[PromotionsToBuySkus]  WITH CHECK ADD  CONSTRAINT [FK_PromotionsToBuySkus_ToPromotion] FOREIGN KEY([IdPromotion])
REFERENCES [dbo].[Promotions] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromotionsToBuySkus_ToPromotion]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromotionsToBuySkus]'))
ALTER TABLE [dbo].[PromotionsToBuySkus] CHECK CONSTRAINT [FK_PromotionsToBuySkus_ToPromotion]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromotionsToBuySkus_ToSku]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromotionsToBuySkus]'))
ALTER TABLE [dbo].[PromotionsToBuySkus]  WITH CHECK ADD  CONSTRAINT [FK_PromotionsToBuySkus_ToSku] FOREIGN KEY([IdSku])
REFERENCES [dbo].[Skus] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromotionsToBuySkus_ToSku]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromotionsToBuySkus]'))
ALTER TABLE [dbo].[PromotionsToBuySkus] CHECK CONSTRAINT [FK_PromotionsToBuySkus_ToSku]
GO
