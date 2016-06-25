/****** Object:  Table [dbo].[DiscountOptionTypes]    Script Date: 6/25/2016 2:11:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DiscountOptionTypes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DiscountOptionTypes](
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
/****** Object:  Index [IX_DiscountOptionTypes_Name]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[DiscountOptionTypes]') AND name = N'IX_DiscountOptionTypes_Name')
CREATE NONCLUSTERED INDEX [IX_DiscountOptionTypes_Name] ON [dbo].[DiscountOptionTypes]
(
	[Name] ASC
)
INCLUDE ( 	[Id],
	[IdFieldType],
	[IdObjectType]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_UQ_NameTypeDiscountOption]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[DiscountOptionTypes]') AND name = N'IX_UQ_NameTypeDiscountOption')
CREATE UNIQUE NONCLUSTERED INDEX [IX_UQ_NameTypeDiscountOption] ON [dbo].[DiscountOptionTypes]
(
	[Name] ASC,
	[IdObjectType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DiscountOptionTypes_ToDiscountType]') AND parent_object_id = OBJECT_ID(N'[dbo].[DiscountOptionTypes]'))
ALTER TABLE [dbo].[DiscountOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_DiscountOptionTypes_ToDiscountType] FOREIGN KEY([IdObjectType])
REFERENCES [dbo].[DiscountTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DiscountOptionTypes_ToDiscountType]') AND parent_object_id = OBJECT_ID(N'[dbo].[DiscountOptionTypes]'))
ALTER TABLE [dbo].[DiscountOptionTypes] CHECK CONSTRAINT [FK_DiscountOptionTypes_ToDiscountType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DiscountOptionTypes_ToFieldType]') AND parent_object_id = OBJECT_ID(N'[dbo].[DiscountOptionTypes]'))
ALTER TABLE [dbo].[DiscountOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_DiscountOptionTypes_ToFieldType] FOREIGN KEY([IdFieldType])
REFERENCES [dbo].[FieldTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DiscountOptionTypes_ToFieldType]') AND parent_object_id = OBJECT_ID(N'[dbo].[DiscountOptionTypes]'))
ALTER TABLE [dbo].[DiscountOptionTypes] CHECK CONSTRAINT [FK_DiscountOptionTypes_ToFieldType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DiscountOptionTypes_ToLookup]') AND parent_object_id = OBJECT_ID(N'[dbo].[DiscountOptionTypes]'))
ALTER TABLE [dbo].[DiscountOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_DiscountOptionTypes_ToLookup] FOREIGN KEY([IdLookup])
REFERENCES [dbo].[Lookups] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DiscountOptionTypes_ToLookup]') AND parent_object_id = OBJECT_ID(N'[dbo].[DiscountOptionTypes]'))
ALTER TABLE [dbo].[DiscountOptionTypes] CHECK CONSTRAINT [FK_DiscountOptionTypes_ToLookup]
GO
