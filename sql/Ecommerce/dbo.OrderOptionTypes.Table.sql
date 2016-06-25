/****** Object:  Table [dbo].[OrderOptionTypes]    Script Date: 6/25/2016 2:12:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderOptionTypes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[OrderOptionTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[IdLookup] [int] NULL,
	[IdFieldType] [int] NOT NULL,
	[IdObjectType] [int] NULL,
	[DefaultValue] [nvarchar](250) NULL,
 CONSTRAINT [PK_OrderOptionTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_OrderOptionTypes_Name]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OrderOptionTypes]') AND name = N'IX_OrderOptionTypes_Name')
CREATE NONCLUSTERED INDEX [IX_OrderOptionTypes_Name] ON [dbo].[OrderOptionTypes]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_UQ_NameTypeOrderOption]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OrderOptionTypes]') AND name = N'IX_UQ_NameTypeOrderOption')
CREATE UNIQUE NONCLUSTERED INDEX [IX_UQ_NameTypeOrderOption] ON [dbo].[OrderOptionTypes]
(
	[Name] ASC,
	[IdObjectType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderOptionTypesToFieldType]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderOptionTypes]'))
ALTER TABLE [dbo].[OrderOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_OrderOptionTypesToFieldType] FOREIGN KEY([IdFieldType])
REFERENCES [dbo].[FieldTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderOptionTypesToFieldType]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderOptionTypes]'))
ALTER TABLE [dbo].[OrderOptionTypes] CHECK CONSTRAINT [FK_OrderOptionTypesToFieldType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderOptionTypesToLookup]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderOptionTypes]'))
ALTER TABLE [dbo].[OrderOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_OrderOptionTypesToLookup] FOREIGN KEY([IdLookup])
REFERENCES [dbo].[Lookups] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderOptionTypesToLookup]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderOptionTypes]'))
ALTER TABLE [dbo].[OrderOptionTypes] CHECK CONSTRAINT [FK_OrderOptionTypesToLookup]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderOptionTypesToOrderType]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderOptionTypes]'))
ALTER TABLE [dbo].[OrderOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_OrderOptionTypesToOrderType] FOREIGN KEY([IdObjectType])
REFERENCES [dbo].[OrderTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderOptionTypesToOrderType]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderOptionTypes]'))
ALTER TABLE [dbo].[OrderOptionTypes] CHECK CONSTRAINT [FK_OrderOptionTypesToOrderType]
GO
