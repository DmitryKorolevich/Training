/****** Object:  Table [dbo].[DiscountToSelectedCategories]    Script Date: 6/25/2016 2:11:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DiscountToSelectedCategories]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DiscountToSelectedCategories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdCategory] [int] NOT NULL,
	[IdDiscount] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DDiscountToSelectedCategories_ToProductCategory]') AND parent_object_id = OBJECT_ID(N'[dbo].[DiscountToSelectedCategories]'))
ALTER TABLE [dbo].[DiscountToSelectedCategories]  WITH CHECK ADD  CONSTRAINT [FK_DDiscountToSelectedCategories_ToProductCategory] FOREIGN KEY([IdCategory])
REFERENCES [dbo].[ProductCategories] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DDiscountToSelectedCategories_ToProductCategory]') AND parent_object_id = OBJECT_ID(N'[dbo].[DiscountToSelectedCategories]'))
ALTER TABLE [dbo].[DiscountToSelectedCategories] CHECK CONSTRAINT [FK_DDiscountToSelectedCategories_ToProductCategory]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DiscountToSelectedCategories_ToDiscount]') AND parent_object_id = OBJECT_ID(N'[dbo].[DiscountToSelectedCategories]'))
ALTER TABLE [dbo].[DiscountToSelectedCategories]  WITH CHECK ADD  CONSTRAINT [FK_DiscountToSelectedCategories_ToDiscount] FOREIGN KEY([IdDiscount])
REFERENCES [dbo].[Discounts] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DiscountToSelectedCategories_ToDiscount]') AND parent_object_id = OBJECT_ID(N'[dbo].[DiscountToSelectedCategories]'))
ALTER TABLE [dbo].[DiscountToSelectedCategories] CHECK CONSTRAINT [FK_DiscountToSelectedCategories_ToDiscount]
GO
