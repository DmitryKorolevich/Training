/****** Object:  Table [dbo].[CustomerOptionTypes]    Script Date: 6/25/2016 2:10:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomerOptionTypes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CustomerOptionTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[IdFieldType] [int] NOT NULL,
	[IdLookup] [int] NULL,
	[IdObjectType] [int] NULL,
	[DefaultValue] [nvarchar](250) NULL,
 CONSTRAINT [PK_CustomerOptionTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_CustomerOptionTypes_Name]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CustomerOptionTypes]') AND name = N'IX_CustomerOptionTypes_Name')
CREATE NONCLUSTERED INDEX [IX_CustomerOptionTypes_Name] ON [dbo].[CustomerOptionTypes]
(
	[Name] ASC
)
INCLUDE ( 	[Id],
	[IdFieldType],
	[IdObjectType]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_UQ_NameTypeCustomerOption]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CustomerOptionTypes]') AND name = N'IX_UQ_NameTypeCustomerOption')
CREATE UNIQUE NONCLUSTERED INDEX [IX_UQ_NameTypeCustomerOption] ON [dbo].[CustomerOptionTypes]
(
	[Name] ASC,
	[IdObjectType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerOptionTypes_CustomerType]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerOptionTypes]'))
ALTER TABLE [dbo].[CustomerOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_CustomerOptionTypes_CustomerType] FOREIGN KEY([IdObjectType])
REFERENCES [dbo].[CustomerTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerOptionTypes_CustomerType]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerOptionTypes]'))
ALTER TABLE [dbo].[CustomerOptionTypes] CHECK CONSTRAINT [FK_CustomerOptionTypes_CustomerType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerOptionTypes_FieldType]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerOptionTypes]'))
ALTER TABLE [dbo].[CustomerOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_CustomerOptionTypes_FieldType] FOREIGN KEY([IdFieldType])
REFERENCES [dbo].[FieldTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerOptionTypes_FieldType]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerOptionTypes]'))
ALTER TABLE [dbo].[CustomerOptionTypes] CHECK CONSTRAINT [FK_CustomerOptionTypes_FieldType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerOptionTypes_Lookup]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerOptionTypes]'))
ALTER TABLE [dbo].[CustomerOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_CustomerOptionTypes_Lookup] FOREIGN KEY([IdLookup])
REFERENCES [dbo].[Lookups] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerOptionTypes_Lookup]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerOptionTypes]'))
ALTER TABLE [dbo].[CustomerOptionTypes] CHECK CONSTRAINT [FK_CustomerOptionTypes_Lookup]
GO
