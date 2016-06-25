/****** Object:  Table [dbo].[AffiliateOptionTypes]    Script Date: 6/25/2016 2:10:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AffiliateOptionTypes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AffiliateOptionTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[IdLookup] [int] NULL,
	[IdFieldType] [int] NOT NULL,
	[IdObjectType] [int] NULL,
	[DefaultValue] [nvarchar](250) NULL,
 CONSTRAINT [PK_AffiliateOptionTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_AffiliateOptionTypes_Name]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AffiliateOptionTypes]') AND name = N'IX_AffiliateOptionTypes_Name')
CREATE NONCLUSTERED INDEX [IX_AffiliateOptionTypes_Name] ON [dbo].[AffiliateOptionTypes]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_UQ_NameTypeAffiliateOption]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AffiliateOptionTypes]') AND name = N'IX_UQ_NameTypeAffiliateOption')
CREATE UNIQUE NONCLUSTERED INDEX [IX_UQ_NameTypeAffiliateOption] ON [dbo].[AffiliateOptionTypes]
(
	[Name] ASC,
	[IdObjectType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AffiliateOptionTypesToFieldType]') AND parent_object_id = OBJECT_ID(N'[dbo].[AffiliateOptionTypes]'))
ALTER TABLE [dbo].[AffiliateOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_AffiliateOptionTypesToFieldType] FOREIGN KEY([IdFieldType])
REFERENCES [dbo].[FieldTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AffiliateOptionTypesToFieldType]') AND parent_object_id = OBJECT_ID(N'[dbo].[AffiliateOptionTypes]'))
ALTER TABLE [dbo].[AffiliateOptionTypes] CHECK CONSTRAINT [FK_AffiliateOptionTypesToFieldType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AffiliateOptionTypesToLookup]') AND parent_object_id = OBJECT_ID(N'[dbo].[AffiliateOptionTypes]'))
ALTER TABLE [dbo].[AffiliateOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_AffiliateOptionTypesToLookup] FOREIGN KEY([IdLookup])
REFERENCES [dbo].[Lookups] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AffiliateOptionTypesToLookup]') AND parent_object_id = OBJECT_ID(N'[dbo].[AffiliateOptionTypes]'))
ALTER TABLE [dbo].[AffiliateOptionTypes] CHECK CONSTRAINT [FK_AffiliateOptionTypesToLookup]
GO
