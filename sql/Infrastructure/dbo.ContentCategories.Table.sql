/****** Object:  Table [dbo].[ContentCategories]    Script Date: 6/25/2016 3:39:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ContentCategories]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ContentCategories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ContentItemId] [int] NOT NULL,
	[MasterContentItemId] [int] NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[FileUrl] [nvarchar](250) NULL,
	[ParentId] [int] NULL,
	[StatusCode] [int] NOT NULL,
	[Url] [nvarchar](250) NOT NULL,
	[Type] [int] NOT NULL,
	[Order] [int] NOT NULL,
	[UserId] [int] NULL,
	[IdOld] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_ContentCategories_IdOld]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ContentCategories]') AND name = N'IX_ContentCategories_IdOld')
CREATE NONCLUSTERED INDEX [IX_ContentCategories_IdOld] ON [dbo].[ContentCategories]
(
	[IdOld] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
/****** Object:  Index [IX_ContentCategories_MasterContentItemId]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ContentCategories]') AND name = N'IX_ContentCategories_MasterContentItemId')
CREATE NONCLUSTERED INDEX [IX_ContentCategories_MasterContentItemId] ON [dbo].[ContentCategories]
(
	[MasterContentItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
/****** Object:  Index [IX_ContentCategories_ParentId]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ContentCategories]') AND name = N'IX_ContentCategories_ParentId')
CREATE NONCLUSTERED INDEX [IX_ContentCategories_ParentId] ON [dbo].[ContentCategories]
(
	[ParentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_ContentCategories_Url]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ContentCategories]') AND name = N'IX_ContentCategories_Url')
CREATE NONCLUSTERED INDEX [IX_ContentCategories_Url] ON [dbo].[ContentCategories]
(
	[Url] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_ContentCategories_Url_StatusCode]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ContentCategories]') AND name = N'IX_ContentCategories_Url_StatusCode')
CREATE NONCLUSTERED INDEX [IX_ContentCategories_Url_StatusCode] ON [dbo].[ContentCategories]
(
	[Type] ASC,
	[Url] ASC,
	[StatusCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__ContentCa__Statu__160F4887]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ContentCategories] ADD  DEFAULT ((1)) FOR [StatusCode]
END

GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentCategories_AspNetUsers]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentCategories]'))
ALTER TABLE [dbo].[ContentCategories]  WITH CHECK ADD  CONSTRAINT [FK_ContentCategories_AspNetUsers] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentCategories_AspNetUsers]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentCategories]'))
ALTER TABLE [dbo].[ContentCategories] CHECK CONSTRAINT [FK_ContentCategories_AspNetUsers]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentCategories_ToContentCategory]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentCategories]'))
ALTER TABLE [dbo].[ContentCategories]  WITH CHECK ADD  CONSTRAINT [FK_ContentCategories_ToContentCategory] FOREIGN KEY([ParentId])
REFERENCES [dbo].[ContentCategories] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentCategories_ToContentCategory]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentCategories]'))
ALTER TABLE [dbo].[ContentCategories] CHECK CONSTRAINT [FK_ContentCategories_ToContentCategory]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentCategories_ToContentItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentCategories]'))
ALTER TABLE [dbo].[ContentCategories]  WITH CHECK ADD  CONSTRAINT [FK_ContentCategories_ToContentItem] FOREIGN KEY([ContentItemId])
REFERENCES [dbo].[ContentItems] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentCategories_ToContentItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentCategories]'))
ALTER TABLE [dbo].[ContentCategories] CHECK CONSTRAINT [FK_ContentCategories_ToContentItem]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentCategories_ToContentTypes]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentCategories]'))
ALTER TABLE [dbo].[ContentCategories]  WITH CHECK ADD  CONSTRAINT [FK_ContentCategories_ToContentTypes] FOREIGN KEY([Type])
REFERENCES [dbo].[ContentTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentCategories_ToContentTypes]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentCategories]'))
ALTER TABLE [dbo].[ContentCategories] CHECK CONSTRAINT [FK_ContentCategories_ToContentTypes]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentCategories_ToMasterContentItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentCategories]'))
ALTER TABLE [dbo].[ContentCategories]  WITH CHECK ADD  CONSTRAINT [FK_ContentCategories_ToMasterContentItem] FOREIGN KEY([MasterContentItemId])
REFERENCES [dbo].[MasterContentItems] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentCategories_ToMasterContentItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentCategories]'))
ALTER TABLE [dbo].[ContentCategories] CHECK CONSTRAINT [FK_ContentCategories_ToMasterContentItem]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentCategories_ToRecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentCategories]'))
ALTER TABLE [dbo].[ContentCategories]  WITH CHECK ADD  CONSTRAINT [FK_ContentCategories_ToRecordStatusCode] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentCategories_ToRecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentCategories]'))
ALTER TABLE [dbo].[ContentCategories] CHECK CONSTRAINT [FK_ContentCategories_ToRecordStatusCode]
GO
