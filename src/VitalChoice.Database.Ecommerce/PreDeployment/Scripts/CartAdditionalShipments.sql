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