/****** Object:  Table [dbo].[RelatedRecipes]    Script Date: 6/25/2016 3:41:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RelatedRecipes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[RelatedRecipes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdRecipe] [int] NOT NULL,
	[Image] [nvarchar](250) NOT NULL,
	[Title] [nvarchar](250) NOT NULL,
	[Url] [nvarchar](250) NOT NULL,
	[Number] [tinyint] NOT NULL,
 CONSTRAINT [PK_RelatedRecipes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON),
 CONSTRAINT [UQ_RelatedRecipes_Number] UNIQUE NONCLUSTERED 
(
	[IdRecipe] ASC,
	[Number] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_RelatedRecipes_Recipes]') AND parent_object_id = OBJECT_ID(N'[dbo].[RelatedRecipes]'))
ALTER TABLE [dbo].[RelatedRecipes]  WITH CHECK ADD  CONSTRAINT [FK_RelatedRecipes_Recipes] FOREIGN KEY([IdRecipe])
REFERENCES [dbo].[Recipes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_RelatedRecipes_Recipes]') AND parent_object_id = OBJECT_ID(N'[dbo].[RelatedRecipes]'))
ALTER TABLE [dbo].[RelatedRecipes] CHECK CONSTRAINT [FK_RelatedRecipes_Recipes]
GO
