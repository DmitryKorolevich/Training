/****** Object:  Table [dbo].[ProductReviews]    Script Date: 6/25/2016 2:12:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductReviews]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ProductReviews](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[DateEdited] [datetime2](7) NOT NULL,
	[IdProduct] [int] NOT NULL,
	[CustomerName] [nvarchar](250) NOT NULL,
	[Email] [nvarchar](250) NOT NULL,
	[Title] [nvarchar](250) NOT NULL,
	[Description] [nvarchar](4000) NOT NULL,
	[Rating] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_ProductReviews_IdProduct]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ProductReviews]') AND name = N'IX_ProductReviews_IdProduct')
CREATE NONCLUSTERED INDEX [IX_ProductReviews_IdProduct] ON [dbo].[ProductReviews]
(
	[IdProduct] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
/****** Object:  Index [IX_ProductReviews_IdProductStatusCode]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ProductReviews]') AND name = N'IX_ProductReviews_IdProductStatusCode')
CREATE NONCLUSTERED INDEX [IX_ProductReviews_IdProductStatusCode] ON [dbo].[ProductReviews]
(
	[IdProduct] ASC,
	[StatusCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__ProductRe__Statu__345EC57D]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ProductReviews] ADD  DEFAULT ((1)) FOR [StatusCode]
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__ProductRe__DateC__32767D0B]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ProductReviews] ADD  DEFAULT (sysdatetime()) FOR [DateCreated]
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__ProductRe__DateE__336AA144]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ProductReviews] ADD  DEFAULT (sysdatetime()) FOR [DateEdited]
END

GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ProductReviews_ToProducts]') AND parent_object_id = OBJECT_ID(N'[dbo].[ProductReviews]'))
ALTER TABLE [dbo].[ProductReviews]  WITH CHECK ADD  CONSTRAINT [FK_ProductReviews_ToProducts] FOREIGN KEY([IdProduct])
REFERENCES [dbo].[Products] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ProductReviews_ToProducts]') AND parent_object_id = OBJECT_ID(N'[dbo].[ProductReviews]'))
ALTER TABLE [dbo].[ProductReviews] CHECK CONSTRAINT [FK_ProductReviews_ToProducts]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ProductReviews_ToRecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[ProductReviews]'))
ALTER TABLE [dbo].[ProductReviews]  WITH CHECK ADD  CONSTRAINT [FK_ProductReviews_ToRecordStatusCode] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ProductReviews_ToRecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[ProductReviews]'))
ALTER TABLE [dbo].[ProductReviews] CHECK CONSTRAINT [FK_ProductReviews_ToRecordStatusCode]
GO
