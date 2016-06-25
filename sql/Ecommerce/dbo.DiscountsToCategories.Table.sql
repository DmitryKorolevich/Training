/****** Object:  Table [dbo].[DiscountsToCategories]    Script Date: 6/25/2016 2:11:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DiscountsToCategories]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DiscountsToCategories](
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
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DiscountsToCategories_ToDiscount]') AND parent_object_id = OBJECT_ID(N'[dbo].[DiscountsToCategories]'))
ALTER TABLE [dbo].[DiscountsToCategories]  WITH CHECK ADD  CONSTRAINT [FK_DiscountsToCategories_ToDiscount] FOREIGN KEY([IdDiscount])
REFERENCES [dbo].[Discounts] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DiscountsToCategories_ToDiscount]') AND parent_object_id = OBJECT_ID(N'[dbo].[DiscountsToCategories]'))
ALTER TABLE [dbo].[DiscountsToCategories] CHECK CONSTRAINT [FK_DiscountsToCategories_ToDiscount]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DiscountsToCategories_ToProductCategory]') AND parent_object_id = OBJECT_ID(N'[dbo].[DiscountsToCategories]'))
ALTER TABLE [dbo].[DiscountsToCategories]  WITH CHECK ADD  CONSTRAINT [FK_DiscountsToCategories_ToProductCategory] FOREIGN KEY([IdCategory])
REFERENCES [dbo].[ProductCategories] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DiscountsToCategories_ToProductCategory]') AND parent_object_id = OBJECT_ID(N'[dbo].[DiscountsToCategories]'))
ALTER TABLE [dbo].[DiscountsToCategories] CHECK CONSTRAINT [FK_DiscountsToCategories_ToProductCategory]
GO
