/****** Object:  Table [dbo].[ProductsToCategories]    Script Date: 6/25/2016 2:12:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductsToCategories]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ProductsToCategories](
	[IdCategory] [int] NOT NULL,
	[IdProduct] [int] NOT NULL,
	[Order] [int] NULL,
 CONSTRAINT [PK_ProductsToCategories] PRIMARY KEY CLUSTERED 
(
	[IdCategory] ASC,
	[IdProduct] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_IdProduct]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ProductsToCategories]') AND name = N'IX_IdProduct')
CREATE NONCLUSTERED INDEX [IX_IdProduct] ON [dbo].[ProductsToCategories]
(
	[IdProduct] ASC
)
INCLUDE ( 	[IdCategory],
	[Order]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ProductsToCategories_ToProduct]') AND parent_object_id = OBJECT_ID(N'[dbo].[ProductsToCategories]'))
ALTER TABLE [dbo].[ProductsToCategories]  WITH CHECK ADD  CONSTRAINT [FK_ProductsToCategories_ToProduct] FOREIGN KEY([IdProduct])
REFERENCES [dbo].[Products] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ProductsToCategories_ToProduct]') AND parent_object_id = OBJECT_ID(N'[dbo].[ProductsToCategories]'))
ALTER TABLE [dbo].[ProductsToCategories] CHECK CONSTRAINT [FK_ProductsToCategories_ToProduct]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ProductsToCategories_ToProductCategory]') AND parent_object_id = OBJECT_ID(N'[dbo].[ProductsToCategories]'))
ALTER TABLE [dbo].[ProductsToCategories]  WITH CHECK ADD  CONSTRAINT [FK_ProductsToCategories_ToProductCategory] FOREIGN KEY([IdCategory])
REFERENCES [dbo].[ProductCategories] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ProductsToCategories_ToProductCategory]') AND parent_object_id = OBJECT_ID(N'[dbo].[ProductsToCategories]'))
ALTER TABLE [dbo].[ProductsToCategories] CHECK CONSTRAINT [FK_ProductsToCategories_ToProductCategory]
GO
