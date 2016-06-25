/****** Object:  Table [dbo].[Articles]    Script Date: 6/25/2016 3:39:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Articles]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Articles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Url] [nvarchar](250) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[FileUrl] [nvarchar](250) NULL,
	[ContentItemId] [int] NOT NULL,
	[MasterContentItemId] [int] NOT NULL,
	[StatusCode] [int] NOT NULL,
	[PublishedDate] [datetime] NULL,
	[SubTitle] [nvarchar](250) NULL,
	[Author] [nvarchar](250) NULL,
	[UserId] [int] NULL,
	[IdOld] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_Articles_IdOld]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Articles]') AND name = N'IX_Articles_IdOld')
CREATE NONCLUSTERED INDEX [IX_Articles_IdOld] ON [dbo].[Articles]
(
	[IdOld] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Articles_Url_StatusCode]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Articles]') AND name = N'IX_Articles_Url_StatusCode')
CREATE NONCLUSTERED INDEX [IX_Articles_Url_StatusCode] ON [dbo].[Articles]
(
	[Url] ASC,
	[StatusCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Articles__Status__151B244E]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Articles] ADD  DEFAULT ((1)) FOR [StatusCode]
END

GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Articles_AspNetUsers]') AND parent_object_id = OBJECT_ID(N'[dbo].[Articles]'))
ALTER TABLE [dbo].[Articles]  WITH CHECK ADD  CONSTRAINT [FK_Articles_AspNetUsers] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Articles_AspNetUsers]') AND parent_object_id = OBJECT_ID(N'[dbo].[Articles]'))
ALTER TABLE [dbo].[Articles] CHECK CONSTRAINT [FK_Articles_AspNetUsers]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Articles_ToContentItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[Articles]'))
ALTER TABLE [dbo].[Articles]  WITH CHECK ADD  CONSTRAINT [FK_Articles_ToContentItem] FOREIGN KEY([ContentItemId])
REFERENCES [dbo].[ContentItems] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Articles_ToContentItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[Articles]'))
ALTER TABLE [dbo].[Articles] CHECK CONSTRAINT [FK_Articles_ToContentItem]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Articles_ToMasterContentItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[Articles]'))
ALTER TABLE [dbo].[Articles]  WITH CHECK ADD  CONSTRAINT [FK_Articles_ToMasterContentItem] FOREIGN KEY([MasterContentItemId])
REFERENCES [dbo].[MasterContentItems] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Articles_ToMasterContentItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[Articles]'))
ALTER TABLE [dbo].[Articles] CHECK CONSTRAINT [FK_Articles_ToMasterContentItem]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Articles_ToRecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[Articles]'))
ALTER TABLE [dbo].[Articles]  WITH CHECK ADD  CONSTRAINT [FK_Articles_ToRecordStatusCode] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Articles_ToRecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[Articles]'))
ALTER TABLE [dbo].[Articles] CHECK CONSTRAINT [FK_Articles_ToRecordStatusCode]
GO
