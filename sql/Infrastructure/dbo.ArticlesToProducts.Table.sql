/****** Object:  Table [dbo].[ArticlesToProducts]    Script Date: 6/25/2016 3:39:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ArticlesToProducts]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ArticlesToProducts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdArticle] [int] NOT NULL,
	[IdProduct] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ArticlesToProducts_ToRecipe]') AND parent_object_id = OBJECT_ID(N'[dbo].[ArticlesToProducts]'))
ALTER TABLE [dbo].[ArticlesToProducts]  WITH CHECK ADD  CONSTRAINT [FK_ArticlesToProducts_ToRecipe] FOREIGN KEY([IdArticle])
REFERENCES [dbo].[Articles] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ArticlesToProducts_ToRecipe]') AND parent_object_id = OBJECT_ID(N'[dbo].[ArticlesToProducts]'))
ALTER TABLE [dbo].[ArticlesToProducts] CHECK CONSTRAINT [FK_ArticlesToProducts_ToRecipe]
GO
