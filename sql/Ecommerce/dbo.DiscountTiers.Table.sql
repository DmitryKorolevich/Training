/****** Object:  Table [dbo].[DiscountTiers]    Script Date: 6/25/2016 2:11:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DiscountTiers]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DiscountTiers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdDiscount] [int] NOT NULL,
	[From] [money] NOT NULL,
	[To] [money] NULL,
	[IdDiscountType] [int] NOT NULL,
	[Percent] [decimal](5, 2) NULL,
	[Amount] [money] NULL,
	[Order] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DiscountTiers_ToDiscount]') AND parent_object_id = OBJECT_ID(N'[dbo].[DiscountTiers]'))
ALTER TABLE [dbo].[DiscountTiers]  WITH CHECK ADD  CONSTRAINT [FK_DiscountTiers_ToDiscount] FOREIGN KEY([IdDiscount])
REFERENCES [dbo].[Discounts] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DiscountTiers_ToDiscount]') AND parent_object_id = OBJECT_ID(N'[dbo].[DiscountTiers]'))
ALTER TABLE [dbo].[DiscountTiers] CHECK CONSTRAINT [FK_DiscountTiers_ToDiscount]
GO
