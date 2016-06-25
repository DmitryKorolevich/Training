/****** Object:  Table [dbo].[DiscountsToSkus]    Script Date: 6/25/2016 2:11:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DiscountsToSkus]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DiscountsToSkus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdDiscount] [int] NOT NULL,
	[IdSku] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DiscountsToSkus_ToDiscount]') AND parent_object_id = OBJECT_ID(N'[dbo].[DiscountsToSkus]'))
ALTER TABLE [dbo].[DiscountsToSkus]  WITH CHECK ADD  CONSTRAINT [FK_DiscountsToSkus_ToDiscount] FOREIGN KEY([IdDiscount])
REFERENCES [dbo].[Discounts] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DiscountsToSkus_ToDiscount]') AND parent_object_id = OBJECT_ID(N'[dbo].[DiscountsToSkus]'))
ALTER TABLE [dbo].[DiscountsToSkus] CHECK CONSTRAINT [FK_DiscountsToSkus_ToDiscount]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DiscountsToSkus_ToSku]') AND parent_object_id = OBJECT_ID(N'[dbo].[DiscountsToSkus]'))
ALTER TABLE [dbo].[DiscountsToSkus]  WITH CHECK ADD  CONSTRAINT [FK_DiscountsToSkus_ToSku] FOREIGN KEY([IdSku])
REFERENCES [dbo].[Skus] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DiscountsToSkus_ToSku]') AND parent_object_id = OBJECT_ID(N'[dbo].[DiscountsToSkus]'))
ALTER TABLE [dbo].[DiscountsToSkus] CHECK CONSTRAINT [FK_DiscountsToSkus_ToSku]
GO
