/****** Object:  Table [dbo].[CatalogRequestAddresses]    Script Date: 6/25/2016 2:10:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatalogRequestAddresses]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CatalogRequestAddresses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdCountry] [int] NOT NULL,
	[IdState] [int] NULL,
	[County] [nvarchar](250) NULL,
	[IdObjectType] [int] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[DateEdited] [datetime2](7) NOT NULL,
	[StatusCode] [int] NOT NULL,
 CONSTRAINT [PK_CatalogRequestAddresses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_CatalogRequestAddresses_StatusCode]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CatalogRequestAddresses]') AND name = N'IX_CatalogRequestAddresses_StatusCode')
CREATE NONCLUSTERED INDEX [IX_CatalogRequestAddresses_StatusCode] ON [dbo].[CatalogRequestAddresses]
(
	[StatusCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CatalogRequestAddressesToAddressType]') AND parent_object_id = OBJECT_ID(N'[dbo].[CatalogRequestAddresses]'))
ALTER TABLE [dbo].[CatalogRequestAddresses]  WITH CHECK ADD  CONSTRAINT [FK_CatalogRequestAddressesToAddressType] FOREIGN KEY([IdObjectType])
REFERENCES [dbo].[AddressTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CatalogRequestAddressesToAddressType]') AND parent_object_id = OBJECT_ID(N'[dbo].[CatalogRequestAddresses]'))
ALTER TABLE [dbo].[CatalogRequestAddresses] CHECK CONSTRAINT [FK_CatalogRequestAddressesToAddressType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CatalogRequestAddressesToCountry]') AND parent_object_id = OBJECT_ID(N'[dbo].[CatalogRequestAddresses]'))
ALTER TABLE [dbo].[CatalogRequestAddresses]  WITH CHECK ADD  CONSTRAINT [FK_CatalogRequestAddressesToCountry] FOREIGN KEY([IdCountry])
REFERENCES [dbo].[Countries] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CatalogRequestAddressesToCountry]') AND parent_object_id = OBJECT_ID(N'[dbo].[CatalogRequestAddresses]'))
ALTER TABLE [dbo].[CatalogRequestAddresses] CHECK CONSTRAINT [FK_CatalogRequestAddressesToCountry]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CatalogRequestAddressesToRecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[CatalogRequestAddresses]'))
ALTER TABLE [dbo].[CatalogRequestAddresses]  WITH CHECK ADD  CONSTRAINT [FK_CatalogRequestAddressesToRecordStatusCode] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CatalogRequestAddressesToRecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[CatalogRequestAddresses]'))
ALTER TABLE [dbo].[CatalogRequestAddresses] CHECK CONSTRAINT [FK_CatalogRequestAddressesToRecordStatusCode]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CatalogRequestAddressesToState]') AND parent_object_id = OBJECT_ID(N'[dbo].[CatalogRequestAddresses]'))
ALTER TABLE [dbo].[CatalogRequestAddresses]  WITH CHECK ADD  CONSTRAINT [FK_CatalogRequestAddressesToState] FOREIGN KEY([IdState])
REFERENCES [dbo].[States] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CatalogRequestAddressesToState]') AND parent_object_id = OBJECT_ID(N'[dbo].[CatalogRequestAddresses]'))
ALTER TABLE [dbo].[CatalogRequestAddresses] CHECK CONSTRAINT [FK_CatalogRequestAddressesToState]
GO
