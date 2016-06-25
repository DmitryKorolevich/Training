/****** Object:  Table [dbo].[ContentPagesToContentCategories]    Script Date: 6/25/2016 3:40:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ContentPagesToContentCategories]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ContentPagesToContentCategories](
	[ContentPageId] [int] NOT NULL,
	[ContentCategoryId] [int] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_ContentPagesToContentCategories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_ContentPagesToContentCategories_ContentCategoryId]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ContentPagesToContentCategories]') AND name = N'IX_ContentPagesToContentCategories_ContentCategoryId')
CREATE NONCLUSTERED INDEX [IX_ContentPagesToContentCategories_ContentCategoryId] ON [dbo].[ContentPagesToContentCategories]
(
	[ContentCategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentPagesToContentCategories_ContentCategories]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentPagesToContentCategories]'))
ALTER TABLE [dbo].[ContentPagesToContentCategories]  WITH CHECK ADD  CONSTRAINT [FK_ContentPagesToContentCategories_ContentCategories] FOREIGN KEY([ContentCategoryId])
REFERENCES [dbo].[ContentCategories] ([Id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentPagesToContentCategories_ContentCategories]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentPagesToContentCategories]'))
ALTER TABLE [dbo].[ContentPagesToContentCategories] CHECK CONSTRAINT [FK_ContentPagesToContentCategories_ContentCategories]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentPagesToContentCategories_Recipes]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentPagesToContentCategories]'))
ALTER TABLE [dbo].[ContentPagesToContentCategories]  WITH CHECK ADD  CONSTRAINT [FK_ContentPagesToContentCategories_Recipes] FOREIGN KEY([ContentPageId])
REFERENCES [dbo].[ContentPages] ([Id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentPagesToContentCategories_Recipes]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentPagesToContentCategories]'))
ALTER TABLE [dbo].[ContentPagesToContentCategories] CHECK CONSTRAINT [FK_ContentPagesToContentCategories_Recipes]
GO
