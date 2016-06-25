/****** Object:  Table [dbo].[Promotions]    Script Date: 6/25/2016 2:12:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Promotions]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Promotions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[DateEdited] [datetime2](7) NOT NULL,
	[IdObjectType] [int] NOT NULL,
	[IdEditedBy] [int] NULL,
	[IdAddedBy] [int] NULL,
	[Description] [nvarchar](250) NOT NULL,
	[Assigned] [int] NULL,
	[StartDate] [datetime2](7) NULL,
	[ExpirationDate] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_Promotions_StatusCode]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Promotions]') AND name = N'IX_Promotions_StatusCode')
CREATE NONCLUSTERED INDEX [IX_Promotions_StatusCode] ON [dbo].[Promotions]
(
	[StatusCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Promotion__Statu__39237A9A]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Promotions] ADD  DEFAULT ((1)) FOR [StatusCode]
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Promotion__DateC__3A179ED3]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Promotions] ADD  DEFAULT (sysdatetime()) FOR [DateCreated]
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Promotion__DateE__3B0BC30C]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Promotions] ADD  DEFAULT (sysdatetime()) FOR [DateEdited]
END

GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Promotions_CustomerTypes]') AND parent_object_id = OBJECT_ID(N'[dbo].[Promotions]'))
ALTER TABLE [dbo].[Promotions]  WITH CHECK ADD  CONSTRAINT [FK_Promotions_CustomerTypes] FOREIGN KEY([Assigned])
REFERENCES [dbo].[CustomerTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Promotions_CustomerTypes]') AND parent_object_id = OBJECT_ID(N'[dbo].[Promotions]'))
ALTER TABLE [dbo].[Promotions] CHECK CONSTRAINT [FK_Promotions_CustomerTypes]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Promotions_ToPromotionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[Promotions]'))
ALTER TABLE [dbo].[Promotions]  WITH CHECK ADD  CONSTRAINT [FK_Promotions_ToPromotionType] FOREIGN KEY([IdObjectType])
REFERENCES [dbo].[PromotionTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Promotions_ToPromotionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[Promotions]'))
ALTER TABLE [dbo].[Promotions] CHECK CONSTRAINT [FK_Promotions_ToPromotionType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Promotions_ToRecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[Promotions]'))
ALTER TABLE [dbo].[Promotions]  WITH CHECK ADD  CONSTRAINT [FK_Promotions_ToRecordStatusCode] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Promotions_ToRecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[Promotions]'))
ALTER TABLE [dbo].[Promotions] CHECK CONSTRAINT [FK_Promotions_ToRecordStatusCode]
GO
