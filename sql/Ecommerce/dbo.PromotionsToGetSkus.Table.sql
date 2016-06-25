/****** Object:  Table [dbo].[PromotionsToGetSkus]    Script Date: 6/25/2016 2:12:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PromotionsToGetSkus]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PromotionsToGetSkus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdPromotion] [int] NOT NULL,
	[IdSku] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Percent] [decimal](5, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromotionsToGetSkus_ToPromotion]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromotionsToGetSkus]'))
ALTER TABLE [dbo].[PromotionsToGetSkus]  WITH CHECK ADD  CONSTRAINT [FK_PromotionsToGetSkus_ToPromotion] FOREIGN KEY([IdPromotion])
REFERENCES [dbo].[Promotions] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromotionsToGetSkus_ToPromotion]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromotionsToGetSkus]'))
ALTER TABLE [dbo].[PromotionsToGetSkus] CHECK CONSTRAINT [FK_PromotionsToGetSkus_ToPromotion]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromotionsToGetSkus_ToSku]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromotionsToGetSkus]'))
ALTER TABLE [dbo].[PromotionsToGetSkus]  WITH CHECK ADD  CONSTRAINT [FK_PromotionsToGetSkus_ToSku] FOREIGN KEY([IdSku])
REFERENCES [dbo].[Skus] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromotionsToGetSkus_ToSku]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromotionsToGetSkus]'))
ALTER TABLE [dbo].[PromotionsToGetSkus] CHECK CONSTRAINT [FK_PromotionsToGetSkus_ToSku]
GO
