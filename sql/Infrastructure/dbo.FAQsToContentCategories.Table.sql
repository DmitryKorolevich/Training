/****** Object:  Table [dbo].[FAQsToContentCategories]    Script Date: 6/25/2016 3:40:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FAQsToContentCategories]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[FAQsToContentCategories](
	[FAQId] [int] NOT NULL,
	[ContentCategoryId] [int] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_FAQsToContentCategories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_FAQsToContentCategories_ContentCategoryId]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[FAQsToContentCategories]') AND name = N'IX_FAQsToContentCategories_ContentCategoryId')
CREATE NONCLUSTERED INDEX [IX_FAQsToContentCategories_ContentCategoryId] ON [dbo].[FAQsToContentCategories]
(
	[ContentCategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FAQsToContentCategories_ContentCategories]') AND parent_object_id = OBJECT_ID(N'[dbo].[FAQsToContentCategories]'))
ALTER TABLE [dbo].[FAQsToContentCategories]  WITH CHECK ADD  CONSTRAINT [FK_FAQsToContentCategories_ContentCategories] FOREIGN KEY([ContentCategoryId])
REFERENCES [dbo].[ContentCategories] ([Id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FAQsToContentCategories_ContentCategories]') AND parent_object_id = OBJECT_ID(N'[dbo].[FAQsToContentCategories]'))
ALTER TABLE [dbo].[FAQsToContentCategories] CHECK CONSTRAINT [FK_FAQsToContentCategories_ContentCategories]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FAQsToContentCategories_FAQ]') AND parent_object_id = OBJECT_ID(N'[dbo].[FAQsToContentCategories]'))
ALTER TABLE [dbo].[FAQsToContentCategories]  WITH CHECK ADD  CONSTRAINT [FK_FAQsToContentCategories_FAQ] FOREIGN KEY([FAQId])
REFERENCES [dbo].[FAQs] ([Id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FAQsToContentCategories_FAQ]') AND parent_object_id = OBJECT_ID(N'[dbo].[FAQsToContentCategories]'))
ALTER TABLE [dbo].[FAQsToContentCategories] CHECK CONSTRAINT [FK_FAQsToContentCategories_FAQ]
GO
