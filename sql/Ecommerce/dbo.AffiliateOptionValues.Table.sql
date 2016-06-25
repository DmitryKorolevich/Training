/****** Object:  Table [dbo].[AffiliateOptionValues]    Script Date: 6/25/2016 2:10:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AffiliateOptionValues]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AffiliateOptionValues](
	[IdOptionType] [int] NOT NULL,
	[IdAffiliate] [int] NOT NULL,
	[Value] [nvarchar](250) NULL,
	[IdBigString] [bigint] NULL,
 CONSTRAINT [PK_AffiliateOptionValues] PRIMARY KEY CLUSTERED 
(
	[IdAffiliate] ASC,
	[IdOptionType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_AffiliateOptionValues_Value]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AffiliateOptionValues]') AND name = N'IX_AffiliateOptionValues_Value')
CREATE NONCLUSTERED INDEX [IX_AffiliateOptionValues_Value] ON [dbo].[AffiliateOptionValues]
(
	[Value] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_AffiliateOptionValues_ValuesSearch]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AffiliateOptionValues]') AND name = N'IX_AffiliateOptionValues_ValuesSearch')
CREATE NONCLUSTERED INDEX [IX_AffiliateOptionValues_ValuesSearch] ON [dbo].[AffiliateOptionValues]
(
	[Value] ASC,
	[IdOptionType] ASC
)
INCLUDE ( 	[IdAffiliate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AffiliateOptionValuesToAffiliate]') AND parent_object_id = OBJECT_ID(N'[dbo].[AffiliateOptionValues]'))
ALTER TABLE [dbo].[AffiliateOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_AffiliateOptionValuesToAffiliate] FOREIGN KEY([IdAffiliate])
REFERENCES [dbo].[Affiliates] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AffiliateOptionValuesToAffiliate]') AND parent_object_id = OBJECT_ID(N'[dbo].[AffiliateOptionValues]'))
ALTER TABLE [dbo].[AffiliateOptionValues] CHECK CONSTRAINT [FK_AffiliateOptionValuesToAffiliate]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AffiliateOptionValuesToAffiliateOptionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[AffiliateOptionValues]'))
ALTER TABLE [dbo].[AffiliateOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_AffiliateOptionValuesToAffiliateOptionType] FOREIGN KEY([IdOptionType])
REFERENCES [dbo].[AffiliateOptionTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AffiliateOptionValuesToAffiliateOptionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[AffiliateOptionValues]'))
ALTER TABLE [dbo].[AffiliateOptionValues] CHECK CONSTRAINT [FK_AffiliateOptionValuesToAffiliateOptionType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AffiliateOptionValuesToBigStringValues]') AND parent_object_id = OBJECT_ID(N'[dbo].[AffiliateOptionValues]'))
ALTER TABLE [dbo].[AffiliateOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_AffiliateOptionValuesToBigStringValues] FOREIGN KEY([IdBigString])
REFERENCES [dbo].[BigStringValues] ([IdBigString])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AffiliateOptionValuesToBigStringValues]') AND parent_object_id = OBJECT_ID(N'[dbo].[AffiliateOptionValues]'))
ALTER TABLE [dbo].[AffiliateOptionValues] CHECK CONSTRAINT [FK_AffiliateOptionValuesToBigStringValues]
GO
