/****** Object:  Table [dbo].[SkuOptionTypes]    Script Date: 6/25/2016 2:13:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SkuOptionTypes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[SkuOptionTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[IdFieldType] [int] NOT NULL,
	[IdLookup] [int] NULL,
	[IdObjectType] [int] NULL,
	[DefaultValue] [nvarchar](250) NULL,
 CONSTRAINT [PK_SkuOptionTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_SkuOptionTypes_Name]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SkuOptionTypes]') AND name = N'IX_SkuOptionTypes_Name')
CREATE NONCLUSTERED INDEX [IX_SkuOptionTypes_Name] ON [dbo].[SkuOptionTypes]
(
	[Name] ASC
)
INCLUDE ( 	[Id],
	[IdFieldType],
	[IdObjectType],
	[IdLookup],
	[DefaultValue]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_UQ_NameTypeSkuOption]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SkuOptionTypes]') AND name = N'IX_UQ_NameTypeSkuOption')
CREATE UNIQUE NONCLUSTERED INDEX [IX_UQ_NameTypeSkuOption] ON [dbo].[SkuOptionTypes]
(
	[Name] ASC,
	[IdObjectType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SkuOptionTypes_ToFieldType]') AND parent_object_id = OBJECT_ID(N'[dbo].[SkuOptionTypes]'))
ALTER TABLE [dbo].[SkuOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_SkuOptionTypes_ToFieldType] FOREIGN KEY([IdFieldType])
REFERENCES [dbo].[FieldTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SkuOptionTypes_ToFieldType]') AND parent_object_id = OBJECT_ID(N'[dbo].[SkuOptionTypes]'))
ALTER TABLE [dbo].[SkuOptionTypes] CHECK CONSTRAINT [FK_SkuOptionTypes_ToFieldType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SkuOptionTypes_ToLookup]') AND parent_object_id = OBJECT_ID(N'[dbo].[SkuOptionTypes]'))
ALTER TABLE [dbo].[SkuOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_SkuOptionTypes_ToLookup] FOREIGN KEY([IdLookup])
REFERENCES [dbo].[Lookups] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SkuOptionTypes_ToLookup]') AND parent_object_id = OBJECT_ID(N'[dbo].[SkuOptionTypes]'))
ALTER TABLE [dbo].[SkuOptionTypes] CHECK CONSTRAINT [FK_SkuOptionTypes_ToLookup]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SkuOptionTypes_ToProductType]') AND parent_object_id = OBJECT_ID(N'[dbo].[SkuOptionTypes]'))
ALTER TABLE [dbo].[SkuOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_SkuOptionTypes_ToProductType] FOREIGN KEY([IdObjectType])
REFERENCES [dbo].[ProductTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SkuOptionTypes_ToProductType]') AND parent_object_id = OBJECT_ID(N'[dbo].[SkuOptionTypes]'))
ALTER TABLE [dbo].[SkuOptionTypes] CHECK CONSTRAINT [FK_SkuOptionTypes_ToProductType]
GO
