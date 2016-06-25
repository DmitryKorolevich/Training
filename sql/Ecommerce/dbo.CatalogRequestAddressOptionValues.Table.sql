/****** Object:  Table [dbo].[CatalogRequestAddressOptionValues]    Script Date: 6/25/2016 2:10:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatalogRequestAddressOptionValues]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CatalogRequestAddressOptionValues](
	[IdOptionType] [int] NOT NULL,
	[IdCatalogRequestAddress] [int] NOT NULL,
	[Value] [nvarchar](250) NULL,
 CONSTRAINT [PK_CatalogRequestAddressOptionValues] PRIMARY KEY CLUSTERED 
(
	[IdCatalogRequestAddress] ASC,
	[IdOptionType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CatalogRequestAddressOptionValuesToAddressOptionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[CatalogRequestAddressOptionValues]'))
ALTER TABLE [dbo].[CatalogRequestAddressOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_CatalogRequestAddressOptionValuesToAddressOptionType] FOREIGN KEY([IdOptionType])
REFERENCES [dbo].[AddressOptionTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CatalogRequestAddressOptionValuesToAddressOptionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[CatalogRequestAddressOptionValues]'))
ALTER TABLE [dbo].[CatalogRequestAddressOptionValues] CHECK CONSTRAINT [FK_CatalogRequestAddressOptionValuesToAddressOptionType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CatalogRequestAddressOptionValuesToCatalogRequestAddress]') AND parent_object_id = OBJECT_ID(N'[dbo].[CatalogRequestAddressOptionValues]'))
ALTER TABLE [dbo].[CatalogRequestAddressOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_CatalogRequestAddressOptionValuesToCatalogRequestAddress] FOREIGN KEY([IdCatalogRequestAddress])
REFERENCES [dbo].[CatalogRequestAddresses] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CatalogRequestAddressOptionValuesToCatalogRequestAddress]') AND parent_object_id = OBJECT_ID(N'[dbo].[CatalogRequestAddressOptionValues]'))
ALTER TABLE [dbo].[CatalogRequestAddressOptionValues] CHECK CONSTRAINT [FK_CatalogRequestAddressOptionValuesToCatalogRequestAddress]
GO
