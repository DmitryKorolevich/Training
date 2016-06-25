/****** Object:  Table [dbo].[PromotionsToSelectedCategories]    Script Date: 6/25/2016 2:13:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PromotionsToSelectedCategories]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PromotionsToSelectedCategories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdCategory] [int] NOT NULL,
	[IdPromotion] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromotionsToSelectedCategories_ToProductCategory]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromotionsToSelectedCategories]'))
ALTER TABLE [dbo].[PromotionsToSelectedCategories]  WITH CHECK ADD  CONSTRAINT [FK_PromotionsToSelectedCategories_ToProductCategory] FOREIGN KEY([IdCategory])
REFERENCES [dbo].[ProductCategories] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromotionsToSelectedCategories_ToProductCategory]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromotionsToSelectedCategories]'))
ALTER TABLE [dbo].[PromotionsToSelectedCategories] CHECK CONSTRAINT [FK_PromotionsToSelectedCategories_ToProductCategory]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromotionToSelectedCategories_ToDiscount]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromotionsToSelectedCategories]'))
ALTER TABLE [dbo].[PromotionsToSelectedCategories]  WITH CHECK ADD  CONSTRAINT [FK_PromotionToSelectedCategories_ToDiscount] FOREIGN KEY([IdPromotion])
REFERENCES [dbo].[Promotions] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromotionToSelectedCategories_ToDiscount]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromotionsToSelectedCategories]'))
ALTER TABLE [dbo].[PromotionsToSelectedCategories] CHECK CONSTRAINT [FK_PromotionToSelectedCategories_ToDiscount]
GO
