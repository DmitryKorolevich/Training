/****** Object:  Table [dbo].[Discounts]    Script Date: 6/25/2016 2:11:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Discounts]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Discounts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[DateEdited] [datetime2](7) NOT NULL,
	[IdObjectType] [int] NOT NULL,
	[IdEditedBy] [int] NULL,
	[Description] [nvarchar](250) NOT NULL,
	[Code] [nvarchar](250) NOT NULL,
	[Assigned] [int] NULL,
	[StartDate] [datetime2](7) NULL,
	[ExpirationDate] [datetime2](7) NULL,
	[ExcludeSkus] [bit] NOT NULL,
	[ExcludeCategories] [bit] NOT NULL,
	[IdAddedBy] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Discounts_Code]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Discounts]') AND name = N'IX_Discounts_Code')
CREATE NONCLUSTERED INDEX [IX_Discounts_Code] ON [dbo].[Discounts]
(
	[Code] ASC,
	[StatusCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
/****** Object:  Index [IX_Discounts_StatusCode]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Discounts]') AND name = N'IX_Discounts_StatusCode')
CREATE NONCLUSTERED INDEX [IX_Discounts_StatusCode] ON [dbo].[Discounts]
(
	[StatusCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Discounts__Statu__1E6F845E]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Discounts] ADD  DEFAULT ((1)) FOR [StatusCode]
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Discounts__DateC__2057CCD0]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Discounts] ADD  DEFAULT (sysdatetime()) FOR [DateCreated]
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Discounts__DateE__1D7B6025]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Discounts] ADD  DEFAULT (sysdatetime()) FOR [DateEdited]
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Discounts__Exclu__1C873BEC]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Discounts] ADD  DEFAULT ((0)) FOR [ExcludeSkus]
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Discounts__Exclu__1F63A897]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Discounts] ADD  DEFAULT ((0)) FOR [ExcludeCategories]
END

GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Discounts_CustomerTypes]') AND parent_object_id = OBJECT_ID(N'[dbo].[Discounts]'))
ALTER TABLE [dbo].[Discounts]  WITH CHECK ADD  CONSTRAINT [FK_Discounts_CustomerTypes] FOREIGN KEY([Assigned])
REFERENCES [dbo].[CustomerTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Discounts_CustomerTypes]') AND parent_object_id = OBJECT_ID(N'[dbo].[Discounts]'))
ALTER TABLE [dbo].[Discounts] CHECK CONSTRAINT [FK_Discounts_CustomerTypes]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Discounts_ToAddedUser]') AND parent_object_id = OBJECT_ID(N'[dbo].[Discounts]'))
ALTER TABLE [dbo].[Discounts]  WITH CHECK ADD  CONSTRAINT [FK_Discounts_ToAddedUser] FOREIGN KEY([IdAddedBy])
REFERENCES [dbo].[Users] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Discounts_ToAddedUser]') AND parent_object_id = OBJECT_ID(N'[dbo].[Discounts]'))
ALTER TABLE [dbo].[Discounts] CHECK CONSTRAINT [FK_Discounts_ToAddedUser]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Discounts_ToProductType]') AND parent_object_id = OBJECT_ID(N'[dbo].[Discounts]'))
ALTER TABLE [dbo].[Discounts]  WITH CHECK ADD  CONSTRAINT [FK_Discounts_ToProductType] FOREIGN KEY([IdObjectType])
REFERENCES [dbo].[DiscountTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Discounts_ToProductType]') AND parent_object_id = OBJECT_ID(N'[dbo].[Discounts]'))
ALTER TABLE [dbo].[Discounts] CHECK CONSTRAINT [FK_Discounts_ToProductType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Discounts_ToRecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[Discounts]'))
ALTER TABLE [dbo].[Discounts]  WITH CHECK ADD  CONSTRAINT [FK_Discounts_ToRecordStatusCode] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Discounts_ToRecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[Discounts]'))
ALTER TABLE [dbo].[Discounts] CHECK CONSTRAINT [FK_Discounts_ToRecordStatusCode]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Discounts_ToUser]') AND parent_object_id = OBJECT_ID(N'[dbo].[Discounts]'))
ALTER TABLE [dbo].[Discounts]  WITH CHECK ADD  CONSTRAINT [FK_Discounts_ToUser] FOREIGN KEY([IdEditedBy])
REFERENCES [dbo].[Users] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Discounts_ToUser]') AND parent_object_id = OBJECT_ID(N'[dbo].[Discounts]'))
ALTER TABLE [dbo].[Discounts] CHECK CONSTRAINT [FK_Discounts_ToUser]
GO
