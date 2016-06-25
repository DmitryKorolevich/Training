/****** Object:  Table [dbo].[ContentPages]    Script Date: 6/25/2016 3:40:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ContentPages]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ContentPages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Url] [nvarchar](250) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[FileUrl] [nvarchar](250) NULL,
	[ContentItemId] [int] NOT NULL,
	[MasterContentItemId] [int] NOT NULL,
	[StatusCode] [int] NOT NULL,
	[Assigned] [int] NOT NULL,
	[UserId] [int] NULL,
	[IdOld] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_ContentPages_IdOld]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ContentPages]') AND name = N'IX_ContentPages_IdOld')
CREATE NONCLUSTERED INDEX [IX_ContentPages_IdOld] ON [dbo].[ContentPages]
(
	[IdOld] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_ContentPages_Url]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ContentPages]') AND name = N'IX_ContentPages_Url')
CREATE NONCLUSTERED INDEX [IX_ContentPages_Url] ON [dbo].[ContentPages]
(
	[Url] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_ContentPages_Url_StatusCode]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ContentPages]') AND name = N'IX_ContentPages_Url_StatusCode')
CREATE NONCLUSTERED INDEX [IX_ContentPages_Url_StatusCode] ON [dbo].[ContentPages]
(
	[Url] ASC,
	[StatusCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__ContentPa__Statu__19DFD96B]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ContentPages] ADD  DEFAULT ((1)) FOR [StatusCode]
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__ContentPa__Assig__18EBB532]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ContentPages] ADD  DEFAULT ((1)) FOR [Assigned]
END

GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentPages_AspNetUsers]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentPages]'))
ALTER TABLE [dbo].[ContentPages]  WITH CHECK ADD  CONSTRAINT [FK_ContentPages_AspNetUsers] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentPages_AspNetUsers]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentPages]'))
ALTER TABLE [dbo].[ContentPages] CHECK CONSTRAINT [FK_ContentPages_AspNetUsers]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentPages_ToContentItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentPages]'))
ALTER TABLE [dbo].[ContentPages]  WITH CHECK ADD  CONSTRAINT [FK_ContentPages_ToContentItem] FOREIGN KEY([ContentItemId])
REFERENCES [dbo].[ContentItems] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentPages_ToContentItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentPages]'))
ALTER TABLE [dbo].[ContentPages] CHECK CONSTRAINT [FK_ContentPages_ToContentItem]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentPages_ToMasterContentItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentPages]'))
ALTER TABLE [dbo].[ContentPages]  WITH CHECK ADD  CONSTRAINT [FK_ContentPages_ToMasterContentItem] FOREIGN KEY([MasterContentItemId])
REFERENCES [dbo].[MasterContentItems] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentPages_ToMasterContentItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentPages]'))
ALTER TABLE [dbo].[ContentPages] CHECK CONSTRAINT [FK_ContentPages_ToMasterContentItem]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentPages_ToRecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentPages]'))
ALTER TABLE [dbo].[ContentPages]  WITH CHECK ADD  CONSTRAINT [FK_ContentPages_ToRecordStatusCode] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentPages_ToRecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentPages]'))
ALTER TABLE [dbo].[ContentPages] CHECK CONSTRAINT [FK_ContentPages_ToRecordStatusCode]
GO
