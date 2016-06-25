/****** Object:  Table [dbo].[AddressOptionTypes]    Script Date: 6/25/2016 2:10:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AddressOptionTypes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AddressOptionTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[IdFieldType] [int] NOT NULL,
	[IdLookup] [int] NULL,
	[IdObjectType] [int] NULL,
	[DefaultValue] [nvarchar](250) NULL,
 CONSTRAINT [PK_AddressOptionTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_AddressOptionTypes_Name]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AddressOptionTypes]') AND name = N'IX_AddressOptionTypes_Name')
CREATE NONCLUSTERED INDEX [IX_AddressOptionTypes_Name] ON [dbo].[AddressOptionTypes]
(
	[Name] ASC
)
INCLUDE ( 	[Id],
	[IdFieldType],
	[IdObjectType]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_UQ_NameTypeAddressOption]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AddressOptionTypes]') AND name = N'IX_UQ_NameTypeAddressOption')
CREATE UNIQUE NONCLUSTERED INDEX [IX_UQ_NameTypeAddressOption] ON [dbo].[AddressOptionTypes]
(
	[Name] ASC,
	[IdObjectType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AddressOptionTypes_AddressType]') AND parent_object_id = OBJECT_ID(N'[dbo].[AddressOptionTypes]'))
ALTER TABLE [dbo].[AddressOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_AddressOptionTypes_AddressType] FOREIGN KEY([IdObjectType])
REFERENCES [dbo].[AddressTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AddressOptionTypes_AddressType]') AND parent_object_id = OBJECT_ID(N'[dbo].[AddressOptionTypes]'))
ALTER TABLE [dbo].[AddressOptionTypes] CHECK CONSTRAINT [FK_AddressOptionTypes_AddressType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AddressOptionTypes_FieldType]') AND parent_object_id = OBJECT_ID(N'[dbo].[AddressOptionTypes]'))
ALTER TABLE [dbo].[AddressOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_AddressOptionTypes_FieldType] FOREIGN KEY([IdFieldType])
REFERENCES [dbo].[FieldTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AddressOptionTypes_FieldType]') AND parent_object_id = OBJECT_ID(N'[dbo].[AddressOptionTypes]'))
ALTER TABLE [dbo].[AddressOptionTypes] CHECK CONSTRAINT [FK_AddressOptionTypes_FieldType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AddressOptionTypes_Lookup]') AND parent_object_id = OBJECT_ID(N'[dbo].[AddressOptionTypes]'))
ALTER TABLE [dbo].[AddressOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_AddressOptionTypes_Lookup] FOREIGN KEY([IdLookup])
REFERENCES [dbo].[Lookups] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AddressOptionTypes_Lookup]') AND parent_object_id = OBJECT_ID(N'[dbo].[AddressOptionTypes]'))
ALTER TABLE [dbo].[AddressOptionTypes] CHECK CONSTRAINT [FK_AddressOptionTypes_Lookup]
GO
