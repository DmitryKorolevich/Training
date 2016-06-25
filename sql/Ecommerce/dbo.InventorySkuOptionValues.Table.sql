/****** Object:  Table [dbo].[InventorySkuOptionValues]    Script Date: 6/25/2016 2:11:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InventorySkuOptionValues]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[InventorySkuOptionValues](
	[IdInventorySku] [int] NOT NULL,
	[IdOptionType] [int] NOT NULL,
	[Value] [nvarchar](250) NULL,
 CONSTRAINT [PK_InventorySkuOptionValues] PRIMARY KEY CLUSTERED 
(
	[IdInventorySku] ASC,
	[IdOptionType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_InventorySkuOptionValue_Value]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[InventorySkuOptionValues]') AND name = N'IX_InventorySkuOptionValue_Value')
CREATE NONCLUSTERED INDEX [IX_InventorySkuOptionValue_Value] ON [dbo].[InventorySkuOptionValues]
(
	[Value] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_InventorySkuOptionValue_ValuesSearch]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[InventorySkuOptionValues]') AND name = N'IX_InventorySkuOptionValue_ValuesSearch')
CREATE NONCLUSTERED INDEX [IX_InventorySkuOptionValue_ValuesSearch] ON [dbo].[InventorySkuOptionValues]
(
	[Value] ASC,
	[IdOptionType] ASC
)
INCLUDE ( 	[IdInventorySku]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventorySkuOptionValue_InventorySkuOptionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventorySkuOptionValues]'))
ALTER TABLE [dbo].[InventorySkuOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_InventorySkuOptionValue_InventorySkuOptionType] FOREIGN KEY([IdOptionType])
REFERENCES [dbo].[InventorySkuOptionTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventorySkuOptionValue_InventorySkuOptionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventorySkuOptionValues]'))
ALTER TABLE [dbo].[InventorySkuOptionValues] CHECK CONSTRAINT [FK_InventorySkuOptionValue_InventorySkuOptionType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventorySkuOptionValue_ToInventorySku]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventorySkuOptionValues]'))
ALTER TABLE [dbo].[InventorySkuOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_InventorySkuOptionValue_ToInventorySku] FOREIGN KEY([IdInventorySku])
REFERENCES [dbo].[InventorySkus] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventorySkuOptionValue_ToInventorySku]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventorySkuOptionValues]'))
ALTER TABLE [dbo].[InventorySkuOptionValues] CHECK CONSTRAINT [FK_InventorySkuOptionValue_ToInventorySku]
GO
