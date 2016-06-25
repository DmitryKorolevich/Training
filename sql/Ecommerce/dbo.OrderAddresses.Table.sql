/****** Object:  Table [dbo].[OrderAddresses]    Script Date: 6/25/2016 2:11:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderAddresses]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[OrderAddresses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdCountry] [int] NOT NULL,
	[IdState] [int] NULL,
	[IdObjectType] [int] NOT NULL,
	[County] [nvarchar](250) NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[DateEdited] [datetime2](7) NOT NULL,
	[IdEditedBy] [int] NULL,
	[StatusCode] [int] NOT NULL,
 CONSTRAINT [PK_OrderAddresses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderAddressesToAddressType]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderAddresses]'))
ALTER TABLE [dbo].[OrderAddresses]  WITH CHECK ADD  CONSTRAINT [FK_OrderAddressesToAddressType] FOREIGN KEY([IdObjectType])
REFERENCES [dbo].[AddressTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderAddressesToAddressType]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderAddresses]'))
ALTER TABLE [dbo].[OrderAddresses] CHECK CONSTRAINT [FK_OrderAddressesToAddressType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderAddressesToCountry]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderAddresses]'))
ALTER TABLE [dbo].[OrderAddresses]  WITH CHECK ADD  CONSTRAINT [FK_OrderAddressesToCountry] FOREIGN KEY([IdCountry])
REFERENCES [dbo].[Countries] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderAddressesToCountry]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderAddresses]'))
ALTER TABLE [dbo].[OrderAddresses] CHECK CONSTRAINT [FK_OrderAddressesToCountry]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderAddressesToRecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderAddresses]'))
ALTER TABLE [dbo].[OrderAddresses]  WITH CHECK ADD  CONSTRAINT [FK_OrderAddressesToRecordStatusCode] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderAddressesToRecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderAddresses]'))
ALTER TABLE [dbo].[OrderAddresses] CHECK CONSTRAINT [FK_OrderAddressesToRecordStatusCode]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderAddressesToState]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderAddresses]'))
ALTER TABLE [dbo].[OrderAddresses]  WITH CHECK ADD  CONSTRAINT [FK_OrderAddressesToState] FOREIGN KEY([IdState])
REFERENCES [dbo].[States] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderAddressesToState]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderAddresses]'))
ALTER TABLE [dbo].[OrderAddresses] CHECK CONSTRAINT [FK_OrderAddressesToState]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderAddressesToUser]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderAddresses]'))
ALTER TABLE [dbo].[OrderAddresses]  WITH CHECK ADD  CONSTRAINT [FK_OrderAddressesToUser] FOREIGN KEY([IdEditedBy])
REFERENCES [dbo].[Users] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderAddressesToUser]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderAddresses]'))
ALTER TABLE [dbo].[OrderAddresses] CHECK CONSTRAINT [FK_OrderAddressesToUser]
GO
