/****** Object:  Table [dbo].[Skus]    Script Date: 6/25/2016 2:13:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Skus]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Skus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdProduct] [int] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[DateEdited] [datetime2](7) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[Price] [money] NOT NULL,
	[WholesalePrice] [money] NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Hidden] [bit] NOT NULL,
	[Order] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_SkuProduct]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Skus]') AND name = N'IX_SkuProduct')
CREATE NONCLUSTERED INDEX [IX_SkuProduct] ON [dbo].[Skus]
(
	[IdProduct] ASC
)
INCLUDE ( 	[Id],
	[DateCreated],
	[DateEdited],
	[StatusCode],
	[Price],
	[WholesalePrice],
	[Code],
	[Hidden],
	[Order]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Skus_Code]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Skus]') AND name = N'IX_Skus_Code')
CREATE NONCLUSTERED INDEX [IX_Skus_Code] ON [dbo].[Skus]
(
	[Code] ASC,
	[StatusCode] ASC
)
INCLUDE ( 	[Id],
	[IdProduct],
	[DateCreated],
	[DateEdited],
	[Price],
	[WholesalePrice]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Skus__DateCreate__3BFFE745]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Skus] ADD  DEFAULT (sysdatetime()) FOR [DateCreated]
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Skus__DateEdited__3FD07829]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Skus] ADD  DEFAULT (sysdatetime()) FOR [DateEdited]
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Skus__StatusCode__3DE82FB7]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Skus] ADD  DEFAULT ((1)) FOR [StatusCode]
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Skus__Price__41B8C09B]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Skus] ADD  DEFAULT ((0)) FOR [Price]
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Skus__WholesaleP__3EDC53F0]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Skus] ADD  DEFAULT ((0)) FOR [WholesalePrice]
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Skus__Hidden__3CF40B7E]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Skus] ADD  DEFAULT ((0)) FOR [Hidden]
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Skus__Order__40C49C62]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Skus] ADD  DEFAULT ((0)) FOR [Order]
END

GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Skus_ToProduct]') AND parent_object_id = OBJECT_ID(N'[dbo].[Skus]'))
ALTER TABLE [dbo].[Skus]  WITH CHECK ADD  CONSTRAINT [FK_Skus_ToProduct] FOREIGN KEY([IdProduct])
REFERENCES [dbo].[Products] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Skus_ToProduct]') AND parent_object_id = OBJECT_ID(N'[dbo].[Skus]'))
ALTER TABLE [dbo].[Skus] CHECK CONSTRAINT [FK_Skus_ToProduct]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Skus_ToRecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[Skus]'))
ALTER TABLE [dbo].[Skus]  WITH CHECK ADD  CONSTRAINT [FK_Skus_ToRecordStatusCode] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Skus_ToRecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[Skus]'))
ALTER TABLE [dbo].[Skus] CHECK CONSTRAINT [FK_Skus_ToRecordStatusCode]
GO
