/****** Object:  Table [dbo].[FAQs]    Script Date: 6/25/2016 3:40:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FAQs]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[FAQs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Url] [nvarchar](250) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[FileUrl] [nvarchar](250) NULL,
	[ContentItemId] [int] NOT NULL,
	[MasterContentItemId] [int] NOT NULL,
	[StatusCode] [int] NOT NULL,
	[UserId] [int] NULL,
	[IdOld] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_FAQs_IdOld]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[FAQs]') AND name = N'IX_FAQs_IdOld')
CREATE NONCLUSTERED INDEX [IX_FAQs_IdOld] ON [dbo].[FAQs]
(
	[IdOld] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
/****** Object:  Index [IX_FAQs_MasterContentItemId]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[FAQs]') AND name = N'IX_FAQs_MasterContentItemId')
CREATE NONCLUSTERED INDEX [IX_FAQs_MasterContentItemId] ON [dbo].[FAQs]
(
	[MasterContentItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_FAQs_Url]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[FAQs]') AND name = N'IX_FAQs_Url')
CREATE NONCLUSTERED INDEX [IX_FAQs_Url] ON [dbo].[FAQs]
(
	[Url] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_FAQs_Url_StatusCode]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[FAQs]') AND name = N'IX_FAQs_Url_StatusCode')
CREATE NONCLUSTERED INDEX [IX_FAQs_Url_StatusCode] ON [dbo].[FAQs]
(
	[Url] ASC,
	[StatusCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__FAQs__StatusCode__1AD3FDA4]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[FAQs] ADD  DEFAULT ((1)) FOR [StatusCode]
END

GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FAQs_AspNetUsers]') AND parent_object_id = OBJECT_ID(N'[dbo].[FAQs]'))
ALTER TABLE [dbo].[FAQs]  WITH CHECK ADD  CONSTRAINT [FK_FAQs_AspNetUsers] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FAQs_AspNetUsers]') AND parent_object_id = OBJECT_ID(N'[dbo].[FAQs]'))
ALTER TABLE [dbo].[FAQs] CHECK CONSTRAINT [FK_FAQs_AspNetUsers]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FAQs_ToContentItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[FAQs]'))
ALTER TABLE [dbo].[FAQs]  WITH CHECK ADD  CONSTRAINT [FK_FAQs_ToContentItem] FOREIGN KEY([ContentItemId])
REFERENCES [dbo].[ContentItems] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FAQs_ToContentItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[FAQs]'))
ALTER TABLE [dbo].[FAQs] CHECK CONSTRAINT [FK_FAQs_ToContentItem]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FAQs_ToMasterContentItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[FAQs]'))
ALTER TABLE [dbo].[FAQs]  WITH CHECK ADD  CONSTRAINT [FK_FAQs_ToMasterContentItem] FOREIGN KEY([MasterContentItemId])
REFERENCES [dbo].[MasterContentItems] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FAQs_ToMasterContentItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[FAQs]'))
ALTER TABLE [dbo].[FAQs] CHECK CONSTRAINT [FK_FAQs_ToMasterContentItem]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FAQs_ToRecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[FAQs]'))
ALTER TABLE [dbo].[FAQs]  WITH CHECK ADD  CONSTRAINT [FK_FAQs_ToRecordStatusCode] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FAQs_ToRecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[FAQs]'))
ALTER TABLE [dbo].[FAQs] CHECK CONSTRAINT [FK_FAQs_ToRecordStatusCode]
GO
