IF OBJECT_ID(N'[dbo].[CatalogRequestAddresses]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[CatalogRequestAddresses] (
		[Id] INT NOT NULL
			CONSTRAINT PK_CatalogRequestAddresses PRIMARY KEY (Id) IDENTITY,
		[IdCountry] INT NOT NULL
			CONSTRAINT FK_CatalogRequestAddressesToCountry FOREIGN KEY (IdCountry) REFERENCES dbo.Countries (Id),
		[IdState] INT NULL
			CONSTRAINT FK_CatalogRequestAddressesToState FOREIGN KEY (IdState) REFERENCES dbo.States (Id),
		[County] NVARCHAR(250) NULL,		
		[IdObjectType] INT NOT NULL
			CONSTRAINT FK_CatalogRequestAddressesToAddressType FOREIGN KEY (IdObjectType) REFERENCES dbo.AddressTypes (Id),
		[DateCreated] [datetime2](7) NOT NULL,
		[DateEdited] [datetime2](7) NOT NULL,
		[StatusCode] INT NOT NULL
			CONSTRAINT FK_CatalogRequestAddressesToRecordStatusCode FOREIGN KEY (StatusCode) REFERENCES dbo.RecordStatusCodes (StatusCode)
	)
	
	CREATE NONCLUSTERED INDEX IX_CatalogRequestAddresses_StatusCode ON CatalogRequestAddresses (StatusCode)
	
	CREATE TABLE [dbo].[CatalogRequestAddressOptionValues] (
		[IdOptionType] [int] NOT NULL,
		[IdCatalogRequestAddress] [int] NOT NULL,
		[Value] [nvarchar](250) NULL,
		 CONSTRAINT [PK_CatalogRequestAddressOptionValues] PRIMARY KEY CLUSTERED 
		(
			[IdCatalogRequestAddress] ASC,
			[IdOptionType] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[CatalogRequestAddressOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_CatalogRequestAddressOptionValuesToAddressOptionType] FOREIGN KEY([IdOptionType])
	REFERENCES [dbo].[AddressOptionTypes] ([Id])

	ALTER TABLE [dbo].[CatalogRequestAddressOptionValues] CHECK CONSTRAINT [FK_CatalogRequestAddressOptionValuesToAddressOptionType]
	

	ALTER TABLE [dbo].[CatalogRequestAddressOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_CatalogRequestAddressOptionValuesToCatalogRequestAddress] FOREIGN KEY([IdCatalogRequestAddress])
	REFERENCES [dbo].[CatalogRequestAddresses] ([Id])

	ALTER TABLE [dbo].[CatalogRequestAddressOptionValues] CHECK CONSTRAINT [FK_CatalogRequestAddressOptionValuesToCatalogRequestAddress]

END
GO