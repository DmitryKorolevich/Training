/****** Object:  Table [dbo].[Recipes]    Script Date: 6/25/2016 3:40:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Recipes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Recipes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Url] [nvarchar](250) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[FileUrl] [nvarchar](250) NULL,
	[ContentItemId] [int] NOT NULL,
	[MasterContentItemId] [int] NOT NULL,
	[StatusCode] [int] NOT NULL,
	[UserId] [int] NULL,
	[AboutChef] [nvarchar](max) NULL,
	[Ingredients] [nvarchar](max) NULL,
	[Directions] [nvarchar](max) NULL,
	[Subtitle] [nvarchar](250) NOT NULL,
	[YoutubeImage] [nvarchar](250) NOT NULL,
	[YoutubeVideo] [nvarchar](250) NOT NULL,
	[IdOld] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_Articles_MasterContentItemId]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Recipes]') AND name = N'IX_Articles_MasterContentItemId')
CREATE NONCLUSTERED INDEX [IX_Articles_MasterContentItemId] ON [dbo].[Recipes]
(
	[MasterContentItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Articles_Url]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Recipes]') AND name = N'IX_Articles_Url')
CREATE NONCLUSTERED INDEX [IX_Articles_Url] ON [dbo].[Recipes]
(
	[Url] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
/****** Object:  Index [IX_ContentPages_MasterContentItemId]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Recipes]') AND name = N'IX_ContentPages_MasterContentItemId')
CREATE NONCLUSTERED INDEX [IX_ContentPages_MasterContentItemId] ON [dbo].[Recipes]
(
	[MasterContentItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
/****** Object:  Index [IX_Recipes_IdOld]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Recipes]') AND name = N'IX_Recipes_IdOld')
CREATE NONCLUSTERED INDEX [IX_Recipes_IdOld] ON [dbo].[Recipes]
(
	[IdOld] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
/****** Object:  Index [IX_Recipes_MasterContentItemId]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Recipes]') AND name = N'IX_Recipes_MasterContentItemId')
CREATE NONCLUSTERED INDEX [IX_Recipes_MasterContentItemId] ON [dbo].[Recipes]
(
	[MasterContentItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Recipes_Url]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Recipes]') AND name = N'IX_Recipes_Url')
CREATE NONCLUSTERED INDEX [IX_Recipes_Url] ON [dbo].[Recipes]
(
	[Url] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Recipes_Url_StatusCode]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Recipes]') AND name = N'IX_Recipes_Url_StatusCode')
CREATE NONCLUSTERED INDEX [IX_Recipes_Url_StatusCode] ON [dbo].[Recipes]
(
	[Url] ASC,
	[StatusCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Recipes__StatusC__2645B050]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Recipes] ADD  DEFAULT ((1)) FOR [StatusCode]
END

GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Recipes_AspNetUsers]') AND parent_object_id = OBJECT_ID(N'[dbo].[Recipes]'))
ALTER TABLE [dbo].[Recipes]  WITH CHECK ADD  CONSTRAINT [FK_Recipes_AspNetUsers] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Recipes_AspNetUsers]') AND parent_object_id = OBJECT_ID(N'[dbo].[Recipes]'))
ALTER TABLE [dbo].[Recipes] CHECK CONSTRAINT [FK_Recipes_AspNetUsers]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Recipes_ToContentItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[Recipes]'))
ALTER TABLE [dbo].[Recipes]  WITH CHECK ADD  CONSTRAINT [FK_Recipes_ToContentItem] FOREIGN KEY([ContentItemId])
REFERENCES [dbo].[ContentItems] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Recipes_ToContentItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[Recipes]'))
ALTER TABLE [dbo].[Recipes] CHECK CONSTRAINT [FK_Recipes_ToContentItem]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Recipes_ToMasterContentItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[Recipes]'))
ALTER TABLE [dbo].[Recipes]  WITH CHECK ADD  CONSTRAINT [FK_Recipes_ToMasterContentItem] FOREIGN KEY([MasterContentItemId])
REFERENCES [dbo].[MasterContentItems] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Recipes_ToMasterContentItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[Recipes]'))
ALTER TABLE [dbo].[Recipes] CHECK CONSTRAINT [FK_Recipes_ToMasterContentItem]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Recipes_ToRecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[Recipes]'))
ALTER TABLE [dbo].[Recipes]  WITH CHECK ADD  CONSTRAINT [FK_Recipes_ToRecordStatusCode] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Recipes_ToRecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[Recipes]'))
ALTER TABLE [dbo].[Recipes] CHECK CONSTRAINT [FK_Recipes_ToRecordStatusCode]
GO
