/****** Object:  Table [dbo].[GiftCertificates]    Script Date: 6/25/2016 2:11:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GiftCertificates]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[GiftCertificates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](250) NOT NULL,
	[Created] [datetime] NOT NULL,
	[Balance] [money] NOT NULL,
	[FirstName] [nvarchar](250) NULL,
	[LastName] [nvarchar](250) NULL,
	[Email] [nvarchar](250) NULL,
	[StatusCode] [int] NOT NULL,
	[GCType] [int] NOT NULL,
	[UserId] [int] NULL,
	[PublicId] [uniqueidentifier] NOT NULL,
	[IdOrder] [int] NULL,
	[IdSku] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON),
UNIQUE NONCLUSTERED 
(
	[PublicId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_GiftCertificates_Email]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[GiftCertificates]') AND name = N'IX_GiftCertificates_Email')
CREATE NONCLUSTERED INDEX [IX_GiftCertificates_Email] ON [dbo].[GiftCertificates]
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_GiftCertificates_FirstName]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[GiftCertificates]') AND name = N'IX_GiftCertificates_FirstName')
CREATE NONCLUSTERED INDEX [IX_GiftCertificates_FirstName] ON [dbo].[GiftCertificates]
(
	[FirstName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_GiftCertificates_LastName]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[GiftCertificates]') AND name = N'IX_GiftCertificates_LastName')
CREATE NONCLUSTERED INDEX [IX_GiftCertificates_LastName] ON [dbo].[GiftCertificates]
(
	[LastName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_GiftCertificates_ParentId]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[GiftCertificates]') AND name = N'IX_GiftCertificates_ParentId')
CREATE NONCLUSTERED INDEX [IX_GiftCertificates_ParentId] ON [dbo].[GiftCertificates]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__GiftCerti__Creat__24285DB4]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[GiftCertificates] ADD  DEFAULT (getdate()) FOR [Created]
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__GiftCerti__Statu__214BF109]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[GiftCertificates] ADD  DEFAULT ((1)) FOR [StatusCode]
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__GiftCerti__GCTyp__22401542]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[GiftCertificates] ADD  DEFAULT ((1)) FOR [GCType]
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__GiftCerti__Publi__2334397B]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[GiftCertificates] ADD  DEFAULT (newsequentialid()) FOR [PublicId]
END

GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_GiftCertificates_Orders]') AND parent_object_id = OBJECT_ID(N'[dbo].[GiftCertificates]'))
ALTER TABLE [dbo].[GiftCertificates]  WITH CHECK ADD  CONSTRAINT [FK_GiftCertificates_Orders] FOREIGN KEY([IdOrder])
REFERENCES [dbo].[Orders] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_GiftCertificates_Orders]') AND parent_object_id = OBJECT_ID(N'[dbo].[GiftCertificates]'))
ALTER TABLE [dbo].[GiftCertificates] CHECK CONSTRAINT [FK_GiftCertificates_Orders]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_GiftCertificates_RecordStatusCodes]') AND parent_object_id = OBJECT_ID(N'[dbo].[GiftCertificates]'))
ALTER TABLE [dbo].[GiftCertificates]  WITH CHECK ADD  CONSTRAINT [FK_GiftCertificates_RecordStatusCodes] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_GiftCertificates_RecordStatusCodes]') AND parent_object_id = OBJECT_ID(N'[dbo].[GiftCertificates]'))
ALTER TABLE [dbo].[GiftCertificates] CHECK CONSTRAINT [FK_GiftCertificates_RecordStatusCodes]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_GiftCertificatesToSku]') AND parent_object_id = OBJECT_ID(N'[dbo].[GiftCertificates]'))
ALTER TABLE [dbo].[GiftCertificates]  WITH CHECK ADD  CONSTRAINT [FK_GiftCertificatesToSku] FOREIGN KEY([IdSku])
REFERENCES [dbo].[Skus] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_GiftCertificatesToSku]') AND parent_object_id = OBJECT_ID(N'[dbo].[GiftCertificates]'))
ALTER TABLE [dbo].[GiftCertificates] CHECK CONSTRAINT [FK_GiftCertificatesToSku]
GO
