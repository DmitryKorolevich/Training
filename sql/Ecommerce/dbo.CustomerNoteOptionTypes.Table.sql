/****** Object:  Table [dbo].[CustomerNoteOptionTypes]    Script Date: 6/25/2016 2:10:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomerNoteOptionTypes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CustomerNoteOptionTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[IdFieldType] [int] NOT NULL,
	[IdLookup] [int] NULL,
	[DefaultValue] [nvarchar](250) NULL,
 CONSTRAINT [PK_CustomerNoteOptionTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_CustomerNoteOptionTypes_Name]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CustomerNoteOptionTypes]') AND name = N'IX_CustomerNoteOptionTypes_Name')
CREATE NONCLUSTERED INDEX [IX_CustomerNoteOptionTypes_Name] ON [dbo].[CustomerNoteOptionTypes]
(
	[Name] ASC
)
INCLUDE ( 	[Id],
	[IdFieldType]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerNoteOptionTypes_FieldType]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerNoteOptionTypes]'))
ALTER TABLE [dbo].[CustomerNoteOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_CustomerNoteOptionTypes_FieldType] FOREIGN KEY([IdFieldType])
REFERENCES [dbo].[FieldTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerNoteOptionTypes_FieldType]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerNoteOptionTypes]'))
ALTER TABLE [dbo].[CustomerNoteOptionTypes] CHECK CONSTRAINT [FK_CustomerNoteOptionTypes_FieldType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerNoteOptionTypes_Lookup]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerNoteOptionTypes]'))
ALTER TABLE [dbo].[CustomerNoteOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_CustomerNoteOptionTypes_Lookup] FOREIGN KEY([IdLookup])
REFERENCES [dbo].[Lookups] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerNoteOptionTypes_Lookup]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerNoteOptionTypes]'))
ALTER TABLE [dbo].[CustomerNoteOptionTypes] CHECK CONSTRAINT [FK_CustomerNoteOptionTypes_Lookup]
GO
