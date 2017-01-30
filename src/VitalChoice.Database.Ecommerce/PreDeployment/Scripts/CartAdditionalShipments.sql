IF OBJECT_ID(N'[dbo].[CartAdditionalShipments]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[CartAdditionalShipments]
	(
		[Id] INT NOT NULL IDENTITY(1,1),
		[IdOrder] INT NOT NULL,
		[Name] NVARCHAR(250) NOT NULL,
		[IsGiftOrder] BIT NOT NULL,
		[GiftMessage] NVARCHAR(250) NULL,
		[IdShippingAddress] INT NOT NULL,
		CONSTRAINT PK_CartAdditionalShipments PRIMARY KEY CLUSTERED (Id DESC),		
		CONSTRAINT FK_CartAdditionalShipments_OrderAddresses FOREIGN KEY (IdShippingAddress) REFERENCES OrderAddresses(Id),
		CONSTRAINT FK_CartAdditionalShipments_Orders FOREIGN KEY (IdOrder) REFERENCES Orders(Id),
	);

	CREATE NONCLUSTERED INDEX IX_IdOrder ON CartAdditionalShipments
	(
		IdOrder DESC
	)

	CREATE TABLE [dbo].[CartAdditionalShipmentsToSkus]
	(
		[IdCartAdditionalShipment] INT NOT NULL,
		[IdSku] INT NOT NULL,
		[Amount] MONEY NOT NULL,
		[Quantity] INT NOT NULL,
		CONSTRAINT PK_CartAdditionalShipmentsToSkus PRIMARY KEY CLUSTERED (IdCartAdditionalShipment DESC, IdSku ASC),		
		CONSTRAINT FK_CartAdditionalShipmentsToSkus_CartAdditionalShipments FOREIGN KEY (IdCartAdditionalShipment) REFERENCES CartAdditionalShipments(Id),
		CONSTRAINT FK_CartAdditionalShipmentsToSkus_Skus FOREIGN KEY (IdSku) REFERENCES Skus(Id),
	);

	CREATE NONCLUSTERED INDEX IX_IdCartAdditionalShipment ON CartAdditionalShipmentsToSkus
	(
		IdCartAdditionalShipment DESC
	)
END

GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = 'DiscountCode' AND [object_id] = OBJECT_ID(N'[dbo].[CartAdditionalShipments]', N'U'))
BEGIN
	
	ALTER TABLE CartAdditionalShipments
	ADD DiscountCode NVARCHAR(250) NULL

	ALTER TABLE CartAdditionalShipments
	ADD ShipDelayDate datetime2 NULL

	ALTER TABLE CartAdditionalShipments
	ADD ShippingUpgradeP INT NULL

	ALTER TABLE CartAdditionalShipments
	ADD ShippingUpgradeNP INT NULL
	
	CREATE TABLE [dbo].[CartAdditionalShipmentsToGiftCertificates](
		[IdCartAdditionalShipment] [int] NOT NULL,
		[IdGiftCertificate] [int] NOT NULL,
	 CONSTRAINT [PK_CartAdditionalShipmentsToGiftCertificates] PRIMARY KEY CLUSTERED 
	(
		[IdCartAdditionalShipment] ASC,
		[IdGiftCertificate] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[CartAdditionalShipmentsToGiftCertificates]  WITH CHECK ADD  CONSTRAINT [FK_CartAdditionalShipmentsToGiftCertificatesToCartAdditionalShipments] FOREIGN KEY([IdCartAdditionalShipment])
	REFERENCES [dbo].[CartAdditionalShipments] ([Id])

	ALTER TABLE [dbo].[CartAdditionalShipmentsToGiftCertificates] CHECK CONSTRAINT [FK_CartAdditionalShipmentsToGiftCertificatesToCartAdditionalShipments]

	ALTER TABLE [dbo].[CartAdditionalShipmentsToGiftCertificates]  WITH CHECK ADD  CONSTRAINT [FK_CartAdditionalShipmentsToGiftCertificatesToGiftCertificate] FOREIGN KEY([IdGiftCertificate])
	REFERENCES [dbo].[GiftCertificates] ([Id])

	ALTER TABLE [dbo].[CartAdditionalShipmentsToGiftCertificates] CHECK CONSTRAINT [FK_CartAdditionalShipmentsToGiftCertificatesToGiftCertificate]

END

GO