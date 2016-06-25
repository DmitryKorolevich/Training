/****** Object:  Table [dbo].[PromotionOptionTypes]    Script Date: 6/25/2016 2:12:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PromotionOptionTypes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PromotionOptionTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[IdFieldType] [int] NOT NULL,
	[IdLookup] [int] NULL,
	[IdObjectType] [int] NULL,
	[DefaultValue] [nvarchar](250) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_PromotionOptionTypes_Name]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PromotionOptionTypes]') AND name = N'IX_PromotionOptionTypes_Name')
CREATE NONCLUSTERED INDEX [IX_PromotionOptionTypes_Name] ON [dbo].[PromotionOptionTypes]
(
	[Name] ASC
)
INCLUDE ( 	[Id],
	[IdFieldType],
	[IdObjectType]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_UQ_NameTypePromotionOption]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PromotionOptionTypes]') AND name = N'IX_UQ_NameTypePromotionOption')
CREATE UNIQUE NONCLUSTERED INDEX [IX_UQ_NameTypePromotionOption] ON [dbo].[PromotionOptionTypes]
(
	[Name] ASC,
	[IdObjectType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromotionOptionTypes_ToFieldType]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromotionOptionTypes]'))
ALTER TABLE [dbo].[PromotionOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_PromotionOptionTypes_ToFieldType] FOREIGN KEY([IdFieldType])
REFERENCES [dbo].[FieldTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromotionOptionTypes_ToFieldType]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromotionOptionTypes]'))
ALTER TABLE [dbo].[PromotionOptionTypes] CHECK CONSTRAINT [FK_PromotionOptionTypes_ToFieldType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromotionOptionTypes_ToLookup]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromotionOptionTypes]'))
ALTER TABLE [dbo].[PromotionOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_PromotionOptionTypes_ToLookup] FOREIGN KEY([IdLookup])
REFERENCES [dbo].[Lookups] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromotionOptionTypes_ToLookup]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromotionOptionTypes]'))
ALTER TABLE [dbo].[PromotionOptionTypes] CHECK CONSTRAINT [FK_PromotionOptionTypes_ToLookup]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromotionOptionTypes_ToPromotionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromotionOptionTypes]'))
ALTER TABLE [dbo].[PromotionOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_PromotionOptionTypes_ToPromotionType] FOREIGN KEY([IdObjectType])
REFERENCES [dbo].[PromotionTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromotionOptionTypes_ToPromotionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromotionOptionTypes]'))
ALTER TABLE [dbo].[PromotionOptionTypes] CHECK CONSTRAINT [FK_PromotionOptionTypes_ToPromotionType]
GO
