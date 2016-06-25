/****** Object:  Table [dbo].[CartToSkus]    Script Date: 6/25/2016 2:10:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CartToSkus]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CartToSkus](
	[IdCart] [int] NOT NULL,
	[IdSku] [int] NOT NULL,
	[Amount] [money] NOT NULL,
	[Quantity] [int] NOT NULL,
 CONSTRAINT [PK_CartToSkus] PRIMARY KEY CLUSTERED 
(
	[IdCart] ASC,
	[IdSku] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CartSkusToCart]') AND parent_object_id = OBJECT_ID(N'[dbo].[CartToSkus]'))
ALTER TABLE [dbo].[CartToSkus]  WITH CHECK ADD  CONSTRAINT [FK_CartSkusToCart] FOREIGN KEY([IdCart])
REFERENCES [dbo].[Carts] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CartSkusToCart]') AND parent_object_id = OBJECT_ID(N'[dbo].[CartToSkus]'))
ALTER TABLE [dbo].[CartToSkus] CHECK CONSTRAINT [FK_CartSkusToCart]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CartSkusToSku]') AND parent_object_id = OBJECT_ID(N'[dbo].[CartToSkus]'))
ALTER TABLE [dbo].[CartToSkus]  WITH CHECK ADD  CONSTRAINT [FK_CartSkusToSku] FOREIGN KEY([IdSku])
REFERENCES [dbo].[Skus] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CartSkusToSku]') AND parent_object_id = OBJECT_ID(N'[dbo].[CartToSkus]'))
ALTER TABLE [dbo].[CartToSkus] CHECK CONSTRAINT [FK_CartSkusToSku]
GO
