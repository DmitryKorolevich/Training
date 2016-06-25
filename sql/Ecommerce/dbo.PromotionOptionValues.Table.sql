/****** Object:  Table [dbo].[PromotionOptionValues]    Script Date: 6/25/2016 2:12:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PromotionOptionValues]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PromotionOptionValues](
	[IdPromotion] [int] NOT NULL,
	[IdOptionType] [int] NOT NULL,
	[Value] [nvarchar](250) NULL,
 CONSTRAINT [PK_PromotionOptionValues] PRIMARY KEY CLUSTERED 
(
	[IdPromotion] ASC,
	[IdOptionType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_PromotionOptionValues_Value]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PromotionOptionValues]') AND name = N'IX_PromotionOptionValues_Value')
CREATE NONCLUSTERED INDEX [IX_PromotionOptionValues_Value] ON [dbo].[PromotionOptionValues]
(
	[Value] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_PromotionOptionValues_ValuesSearch]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PromotionOptionValues]') AND name = N'IX_PromotionOptionValues_ValuesSearch')
CREATE NONCLUSTERED INDEX [IX_PromotionOptionValues_ValuesSearch] ON [dbo].[PromotionOptionValues]
(
	[Value] ASC,
	[IdOptionType] ASC
)
INCLUDE ( 	[IdPromotion]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromotionOptionValue_ToPromotionOptionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromotionOptionValues]'))
ALTER TABLE [dbo].[PromotionOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_PromotionOptionValue_ToPromotionOptionType] FOREIGN KEY([IdOptionType])
REFERENCES [dbo].[PromotionOptionTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromotionOptionValue_ToPromotionOptionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromotionOptionValues]'))
ALTER TABLE [dbo].[PromotionOptionValues] CHECK CONSTRAINT [FK_PromotionOptionValue_ToPromotionOptionType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromotionOptionValues_ToPromotion]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromotionOptionValues]'))
ALTER TABLE [dbo].[PromotionOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_PromotionOptionValues_ToPromotion] FOREIGN KEY([IdPromotion])
REFERENCES [dbo].[Promotions] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromotionOptionValues_ToPromotion]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromotionOptionValues]'))
ALTER TABLE [dbo].[PromotionOptionValues] CHECK CONSTRAINT [FK_PromotionOptionValues_ToPromotion]
GO
