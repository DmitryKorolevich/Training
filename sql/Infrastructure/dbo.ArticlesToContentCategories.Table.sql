/****** Object:  Table [dbo].[ArticlesToContentCategories]    Script Date: 6/25/2016 3:39:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ArticlesToContentCategories]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ArticlesToContentCategories](
	[ArticleId] [int] NOT NULL,
	[ContentCategoryId] [int] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_ArticlesToContentCategories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_ArticlesToContentCategories_ContentCategoryId]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ArticlesToContentCategories]') AND name = N'IX_ArticlesToContentCategories_ContentCategoryId')
CREATE NONCLUSTERED INDEX [IX_ArticlesToContentCategories_ContentCategoryId] ON [dbo].[ArticlesToContentCategories]
(
	[ContentCategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ArticlesToContentCategories_Articles]') AND parent_object_id = OBJECT_ID(N'[dbo].[ArticlesToContentCategories]'))
ALTER TABLE [dbo].[ArticlesToContentCategories]  WITH CHECK ADD  CONSTRAINT [FK_ArticlesToContentCategories_Articles] FOREIGN KEY([ArticleId])
REFERENCES [dbo].[Articles] ([Id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ArticlesToContentCategories_Articles]') AND parent_object_id = OBJECT_ID(N'[dbo].[ArticlesToContentCategories]'))
ALTER TABLE [dbo].[ArticlesToContentCategories] CHECK CONSTRAINT [FK_ArticlesToContentCategories_Articles]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ArticlesToContentCategories_ContentCategories]') AND parent_object_id = OBJECT_ID(N'[dbo].[ArticlesToContentCategories]'))
ALTER TABLE [dbo].[ArticlesToContentCategories]  WITH CHECK ADD  CONSTRAINT [FK_ArticlesToContentCategories_ContentCategories] FOREIGN KEY([ContentCategoryId])
REFERENCES [dbo].[ContentCategories] ([Id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ArticlesToContentCategories_ContentCategories]') AND parent_object_id = OBJECT_ID(N'[dbo].[ArticlesToContentCategories]'))
ALTER TABLE [dbo].[ArticlesToContentCategories] CHECK CONSTRAINT [FK_ArticlesToContentCategories_ContentCategories]
GO
