/****** Object:  Table [dbo].[SkuOptionValues]    Script Date: 6/25/2016 2:13:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SkuOptionValues]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[SkuOptionValues](
	[IdSku] [int] NOT NULL,
	[IdOptionType] [int] NOT NULL,
	[Value] [nvarchar](250) NULL,
 CONSTRAINT [PK_SkuOptionValues] PRIMARY KEY CLUSTERED 
(
	[IdSku] ASC,
	[IdOptionType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_IdOptionType]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SkuOptionValues]') AND name = N'IX_IdOptionType')
CREATE NONCLUSTERED INDEX [IX_IdOptionType] ON [dbo].[SkuOptionValues]
(
	[IdOptionType] ASC
)
INCLUDE ( 	[IdSku],
	[Value]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_IdSku]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SkuOptionValues]') AND name = N'IX_IdSku')
CREATE NONCLUSTERED INDEX [IX_IdSku] ON [dbo].[SkuOptionValues]
(
	[IdSku] ASC
)
INCLUDE ( 	[IdOptionType],
	[Value]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_SkuOptionValues_Value]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SkuOptionValues]') AND name = N'IX_SkuOptionValues_Value')
CREATE NONCLUSTERED INDEX [IX_SkuOptionValues_Value] ON [dbo].[SkuOptionValues]
(
	[Value] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_SkuOptionValues_ValuesSearch]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SkuOptionValues]') AND name = N'IX_SkuOptionValues_ValuesSearch')
CREATE NONCLUSTERED INDEX [IX_SkuOptionValues_ValuesSearch] ON [dbo].[SkuOptionValues]
(
	[Value] ASC,
	[IdOptionType] ASC
)
INCLUDE ( 	[IdSku]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SkuOptionValue_ToSkuOptionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[SkuOptionValues]'))
ALTER TABLE [dbo].[SkuOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_SkuOptionValue_ToSkuOptionType] FOREIGN KEY([IdOptionType])
REFERENCES [dbo].[SkuOptionTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SkuOptionValue_ToSkuOptionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[SkuOptionValues]'))
ALTER TABLE [dbo].[SkuOptionValues] CHECK CONSTRAINT [FK_SkuOptionValue_ToSkuOptionType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SkuOptionValues_ToSku]') AND parent_object_id = OBJECT_ID(N'[dbo].[SkuOptionValues]'))
ALTER TABLE [dbo].[SkuOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_SkuOptionValues_ToSku] FOREIGN KEY([IdSku])
REFERENCES [dbo].[Skus] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SkuOptionValues_ToSku]') AND parent_object_id = OBJECT_ID(N'[dbo].[SkuOptionValues]'))
ALTER TABLE [dbo].[SkuOptionValues] CHECK CONSTRAINT [FK_SkuOptionValues_ToSku]
GO
