/****** Object:  Table [dbo].[DiscountOptionValues]    Script Date: 6/25/2016 2:11:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DiscountOptionValues]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DiscountOptionValues](
	[IdDiscount] [int] NOT NULL,
	[IdOptionType] [int] NOT NULL,
	[Value] [nvarchar](250) NULL,
 CONSTRAINT [PK_DiscountOptionValues] PRIMARY KEY CLUSTERED 
(
	[IdDiscount] ASC,
	[IdOptionType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_DiscountOptionValues_Value]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[DiscountOptionValues]') AND name = N'IX_DiscountOptionValues_Value')
CREATE NONCLUSTERED INDEX [IX_DiscountOptionValues_Value] ON [dbo].[DiscountOptionValues]
(
	[Value] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_DiscountOptionValues_ValuesSearch]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[DiscountOptionValues]') AND name = N'IX_DiscountOptionValues_ValuesSearch')
CREATE NONCLUSTERED INDEX [IX_DiscountOptionValues_ValuesSearch] ON [dbo].[DiscountOptionValues]
(
	[Value] ASC,
	[IdOptionType] ASC
)
INCLUDE ( 	[IdDiscount]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DiscountOptionValue_ToDiscountOptionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[DiscountOptionValues]'))
ALTER TABLE [dbo].[DiscountOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_DiscountOptionValue_ToDiscountOptionType] FOREIGN KEY([IdOptionType])
REFERENCES [dbo].[DiscountOptionTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DiscountOptionValue_ToDiscountOptionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[DiscountOptionValues]'))
ALTER TABLE [dbo].[DiscountOptionValues] CHECK CONSTRAINT [FK_DiscountOptionValue_ToDiscountOptionType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DiscountOptionValues_ToDiscount]') AND parent_object_id = OBJECT_ID(N'[dbo].[DiscountOptionValues]'))
ALTER TABLE [dbo].[DiscountOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_DiscountOptionValues_ToDiscount] FOREIGN KEY([IdDiscount])
REFERENCES [dbo].[Discounts] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DiscountOptionValues_ToDiscount]') AND parent_object_id = OBJECT_ID(N'[dbo].[DiscountOptionValues]'))
ALTER TABLE [dbo].[DiscountOptionValues] CHECK CONSTRAINT [FK_DiscountOptionValues_ToDiscount]
GO
