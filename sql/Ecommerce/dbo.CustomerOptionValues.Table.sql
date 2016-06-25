/****** Object:  Table [dbo].[CustomerOptionValues]    Script Date: 6/25/2016 2:10:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomerOptionValues]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CustomerOptionValues](
	[IdCustomer] [int] NOT NULL,
	[IdOptionType] [int] NOT NULL,
	[Value] [nvarchar](250) NULL,
	[IdBigString] [bigint] NULL,
 CONSTRAINT [PK_CustomerOptionValues] PRIMARY KEY CLUSTERED 
(
	[IdCustomer] ASC,
	[IdOptionType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_CustomerOptionValues_Value]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CustomerOptionValues]') AND name = N'IX_CustomerOptionValues_Value')
CREATE NONCLUSTERED INDEX [IX_CustomerOptionValues_Value] ON [dbo].[CustomerOptionValues]
(
	[Value] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_CustomerOptionValues_ValuesSearch]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CustomerOptionValues]') AND name = N'IX_CustomerOptionValues_ValuesSearch')
CREATE NONCLUSTERED INDEX [IX_CustomerOptionValues_ValuesSearch] ON [dbo].[CustomerOptionValues]
(
	[Value] ASC,
	[IdOptionType] ASC
)
INCLUDE ( 	[IdCustomer]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
/****** Object:  Index [IX_IdBigString]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CustomerOptionValues]') AND name = N'IX_IdBigString')
CREATE NONCLUSTERED INDEX [IX_IdBigString] ON [dbo].[CustomerOptionValues]
(
	[IdBigString] ASC
)
INCLUDE ( 	[IdCustomer],
	[IdOptionType]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_IdOptionType]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CustomerOptionValues]') AND name = N'IX_IdOptionType')
CREATE NONCLUSTERED INDEX [IX_IdOptionType] ON [dbo].[CustomerOptionValues]
(
	[IdOptionType] ASC
)
INCLUDE ( 	[IdCustomer],
	[Value],
	[IdBigString]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerOptionValue_ToBigStringValue]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerOptionValues]'))
ALTER TABLE [dbo].[CustomerOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_CustomerOptionValue_ToBigStringValue] FOREIGN KEY([IdBigString])
REFERENCES [dbo].[BigStringValues] ([IdBigString])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerOptionValue_ToBigStringValue]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerOptionValues]'))
ALTER TABLE [dbo].[CustomerOptionValues] CHECK CONSTRAINT [FK_CustomerOptionValue_ToBigStringValue]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerOptionValues_Customer]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerOptionValues]'))
ALTER TABLE [dbo].[CustomerOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_CustomerOptionValues_Customer] FOREIGN KEY([IdCustomer])
REFERENCES [dbo].[Customers] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerOptionValues_Customer]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerOptionValues]'))
ALTER TABLE [dbo].[CustomerOptionValues] CHECK CONSTRAINT [FK_CustomerOptionValues_Customer]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerOptionValues_CustomerOptionTypes]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerOptionValues]'))
ALTER TABLE [dbo].[CustomerOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_CustomerOptionValues_CustomerOptionTypes] FOREIGN KEY([IdOptionType])
REFERENCES [dbo].[CustomerOptionTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerOptionValues_CustomerOptionTypes]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerOptionValues]'))
ALTER TABLE [dbo].[CustomerOptionValues] CHECK CONSTRAINT [FK_CustomerOptionValues_CustomerOptionTypes]
GO
