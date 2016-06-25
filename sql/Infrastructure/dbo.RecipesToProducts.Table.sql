/****** Object:  Table [dbo].[RecipesToProducts]    Script Date: 6/25/2016 3:40:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RecipesToProducts]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[RecipesToProducts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdRecipe] [int] NOT NULL,
	[IdProduct] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_RecipesToProducts_ToRecipe]') AND parent_object_id = OBJECT_ID(N'[dbo].[RecipesToProducts]'))
ALTER TABLE [dbo].[RecipesToProducts]  WITH CHECK ADD  CONSTRAINT [FK_RecipesToProducts_ToRecipe] FOREIGN KEY([IdRecipe])
REFERENCES [dbo].[Recipes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_RecipesToProducts_ToRecipe]') AND parent_object_id = OBJECT_ID(N'[dbo].[RecipesToProducts]'))
ALTER TABLE [dbo].[RecipesToProducts] CHECK CONSTRAINT [FK_RecipesToProducts_ToRecipe]
GO
