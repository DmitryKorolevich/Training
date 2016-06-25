/****** Object:  Table [dbo].[RecipesToContentCategories]    Script Date: 6/25/2016 3:40:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RecipesToContentCategories]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[RecipesToContentCategories](
	[RecipeId] [int] NOT NULL,
	[ContentCategoryId] [int] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_RecipesToContentCategories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_RecipesToContentCategories_ContentCategoryId]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[RecipesToContentCategories]') AND name = N'IX_RecipesToContentCategories_ContentCategoryId')
CREATE NONCLUSTERED INDEX [IX_RecipesToContentCategories_ContentCategoryId] ON [dbo].[RecipesToContentCategories]
(
	[ContentCategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_RecipesToContentCategories_ContentCategories]') AND parent_object_id = OBJECT_ID(N'[dbo].[RecipesToContentCategories]'))
ALTER TABLE [dbo].[RecipesToContentCategories]  WITH CHECK ADD  CONSTRAINT [FK_RecipesToContentCategories_ContentCategories] FOREIGN KEY([ContentCategoryId])
REFERENCES [dbo].[ContentCategories] ([Id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_RecipesToContentCategories_ContentCategories]') AND parent_object_id = OBJECT_ID(N'[dbo].[RecipesToContentCategories]'))
ALTER TABLE [dbo].[RecipesToContentCategories] CHECK CONSTRAINT [FK_RecipesToContentCategories_ContentCategories]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_RecipesToContentCategories_Recipes]') AND parent_object_id = OBJECT_ID(N'[dbo].[RecipesToContentCategories]'))
ALTER TABLE [dbo].[RecipesToContentCategories]  WITH CHECK ADD  CONSTRAINT [FK_RecipesToContentCategories_Recipes] FOREIGN KEY([RecipeId])
REFERENCES [dbo].[Recipes] ([Id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_RecipesToContentCategories_Recipes]') AND parent_object_id = OBJECT_ID(N'[dbo].[RecipesToContentCategories]'))
ALTER TABLE [dbo].[RecipesToContentCategories] CHECK CONSTRAINT [FK_RecipesToContentCategories_Recipes]
GO
