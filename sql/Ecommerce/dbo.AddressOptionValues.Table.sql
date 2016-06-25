/****** Object:  Table [dbo].[AddressOptionValues]    Script Date: 6/25/2016 2:10:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AddressOptionValues]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AddressOptionValues](
	[IdAddress] [int] NOT NULL,
	[IdOptionType] [int] NOT NULL,
	[Value] [nvarchar](250) NULL,
 CONSTRAINT [PK_AddressOptionValues] PRIMARY KEY CLUSTERED 
(
	[IdAddress] ASC,
	[IdOptionType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_AddressOptionValues_IdOptionType]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AddressOptionValues]') AND name = N'IX_AddressOptionValues_IdOptionType')
CREATE NONCLUSTERED INDEX [IX_AddressOptionValues_IdOptionType] ON [dbo].[AddressOptionValues]
(
	[IdOptionType] ASC
)
INCLUDE ( 	[IdAddress],
	[Value]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_AddressOptionValues_Value]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AddressOptionValues]') AND name = N'IX_AddressOptionValues_Value')
CREATE NONCLUSTERED INDEX [IX_AddressOptionValues_Value] ON [dbo].[AddressOptionValues]
(
	[Value] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_AddressOptionValues_ValuesSearch]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AddressOptionValues]') AND name = N'IX_AddressOptionValues_ValuesSearch')
CREATE NONCLUSTERED INDEX [IX_AddressOptionValues_ValuesSearch] ON [dbo].[AddressOptionValues]
(
	[Value] ASC,
	[IdOptionType] ASC
)
INCLUDE ( 	[IdAddress]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AddressOptionValues_Address]') AND parent_object_id = OBJECT_ID(N'[dbo].[AddressOptionValues]'))
ALTER TABLE [dbo].[AddressOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_AddressOptionValues_Address] FOREIGN KEY([IdAddress])
REFERENCES [dbo].[Addresses] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AddressOptionValues_Address]') AND parent_object_id = OBJECT_ID(N'[dbo].[AddressOptionValues]'))
ALTER TABLE [dbo].[AddressOptionValues] CHECK CONSTRAINT [FK_AddressOptionValues_Address]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AddressOptionValues_AddressOptionTypes]') AND parent_object_id = OBJECT_ID(N'[dbo].[AddressOptionValues]'))
ALTER TABLE [dbo].[AddressOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_AddressOptionValues_AddressOptionTypes] FOREIGN KEY([IdOptionType])
REFERENCES [dbo].[AddressOptionTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AddressOptionValues_AddressOptionTypes]') AND parent_object_id = OBJECT_ID(N'[dbo].[AddressOptionValues]'))
ALTER TABLE [dbo].[AddressOptionValues] CHECK CONSTRAINT [FK_AddressOptionValues_AddressOptionTypes]
GO
