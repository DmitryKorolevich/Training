/****** Object:  Table [dbo].[ProductOptionValues]    Script Date: 6/25/2016 2:12:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductOptionValues]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ProductOptionValues](
	[IdProduct] [int] NOT NULL,
	[IdOptionType] [int] NOT NULL,
	[Value] [nvarchar](250) NULL,
	[IdBigString] [bigint] NULL,
 CONSTRAINT [PK_ProductOptionValues] PRIMARY KEY CLUSTERED 
(
	[IdProduct] ASC,
	[IdOptionType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_IdBigString]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ProductOptionValues]') AND name = N'IX_IdBigString')
CREATE NONCLUSTERED INDEX [IX_IdBigString] ON [dbo].[ProductOptionValues]
(
	[IdBigString] ASC
)
INCLUDE ( 	[IdProduct],
	[IdOptionType],
	[Value]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_ProductOptionValues_Value]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ProductOptionValues]') AND name = N'IX_ProductOptionValues_Value')
CREATE NONCLUSTERED INDEX [IX_ProductOptionValues_Value] ON [dbo].[ProductOptionValues]
(
	[Value] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_ProductOptionValues_ValuesSearch]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ProductOptionValues]') AND name = N'IX_ProductOptionValues_ValuesSearch')
CREATE NONCLUSTERED INDEX [IX_ProductOptionValues_ValuesSearch] ON [dbo].[ProductOptionValues]
(
	[Value] ASC,
	[IdOptionType] ASC
)
INCLUDE ( 	[IdProduct]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ProductOptionValue_ToBigStringValue]') AND parent_object_id = OBJECT_ID(N'[dbo].[ProductOptionValues]'))
ALTER TABLE [dbo].[ProductOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_ProductOptionValue_ToBigStringValue] FOREIGN KEY([IdBigString])
REFERENCES [dbo].[BigStringValues] ([IdBigString])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ProductOptionValue_ToBigStringValue]') AND parent_object_id = OBJECT_ID(N'[dbo].[ProductOptionValues]'))
ALTER TABLE [dbo].[ProductOptionValues] CHECK CONSTRAINT [FK_ProductOptionValue_ToBigStringValue]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ProductOptionValue_ToProductOptionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[ProductOptionValues]'))
ALTER TABLE [dbo].[ProductOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_ProductOptionValue_ToProductOptionType] FOREIGN KEY([IdOptionType])
REFERENCES [dbo].[ProductOptionTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ProductOptionValue_ToProductOptionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[ProductOptionValues]'))
ALTER TABLE [dbo].[ProductOptionValues] CHECK CONSTRAINT [FK_ProductOptionValue_ToProductOptionType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ProductOptionValues_ToProduct]') AND parent_object_id = OBJECT_ID(N'[dbo].[ProductOptionValues]'))
ALTER TABLE [dbo].[ProductOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_ProductOptionValues_ToProduct] FOREIGN KEY([IdProduct])
REFERENCES [dbo].[Products] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ProductOptionValues_ToProduct]') AND parent_object_id = OBJECT_ID(N'[dbo].[ProductOptionValues]'))
ALTER TABLE [dbo].[ProductOptionValues] CHECK CONSTRAINT [FK_ProductOptionValues_ToProduct]
GO
