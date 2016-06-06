﻿IF OBJECT_ID(N'[dbo].[Orders]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[OrderStatuses] (
		Id INT NOT NULL 
			CONSTRAINT PK_OrderStatuses PRIMARY KEY,
		Name NVARCHAR(50) NOT NULL
	)

	CREATE TABLE [dbo].[OrderTypes] (
		Id INT NOT NULL 
			CONSTRAINT PK_OrderTypes PRIMARY KEY,
		Name NVARCHAR(50) NOT NULL
	)

	CREATE TABLE [dbo].[Orders] (
		[Id] [int] NOT NULL 
			CONSTRAINT PK_Orders PRIMARY KEY (Id) IDENTITY,
		[IdObjectType] [int] NOT NULL
			CONSTRAINT FK_OrdersToOrderType FOREIGN KEY (IdObjectType) REFERENCES dbo.OrderTypes (Id),
		[DateCreated] [datetime2] NOT NULL,
		[DateEdited] [datetime2] NOT NULL,
		[IdEditedBy] [int] NULL
			CONSTRAINT FK_OrdersToUser FOREIGN KEY (IdEditedBy) REFERENCES dbo.Users (Id),
		[StatusCode] INT NOT NULL
			CONSTRAINT FK_OrdersToStatus FOREIGN KEY (StatusCode) REFERENCES dbo.RecordStatusCodes (StatusCode),
		[OrderStatus] INT NOT NULL
			CONSTRAINT FK_OrdersToOrderStatus FOREIGN KEY (OrderStatus) REFERENCES dbo.OrderStatuses (Id),
		[IdCustomer] INT NOT NULL,
			CONSTRAINT FK_OrdersToCustomer FOREIGN KEY (IdCustomer) REFERENCES dbo.Customers (Id),
		[IdDiscount] INT NULL,
			CONSTRAINT FK_OrdersToDiscount FOREIGN KEY (IdDiscount) REFERENCES dbo.Discounts (Id),
		[Total] MONEY NOT NULL,
		[ProductsSubtotal] MONEY NOT NULL,
		[TaxTotal] MONEY NOT NULL,
		[ShippingTotal] MONEY NOT NULL,
		[DiscountTotal] MONEY NOT NULL
	)

	CREATE NONCLUSTERED INDEX IX_Orders_OrderStatus_ObjectStatus_ObjectType_OrderDate ON Orders ([IdObjectType], [DateCreated], [StatusCode], [OrderStatus])

	CREATE TABLE [dbo].[OrderOptionTypes] (
		[Id] INT NOT NULL
			CONSTRAINT PK_OrderOptionTypes PRIMARY KEY (Id) IDENTITY,
		[Name] NVARCHAR(250) NOT NULL,
		[IdLookup] INT NULL
			CONSTRAINT FK_OrderOptionTypesToLookup FOREIGN KEY (IdLookup) REFERENCES dbo.Lookups (Id),
		[IdFieldType] INT NOT NULL
			CONSTRAINT FK_OrderOptionTypesToFieldType FOREIGN KEY (IdFieldType) REFERENCES dbo.FieldTypes (Id),
		[IdObjectType] INT NULL
			CONSTRAINT FK_OrderOptionTypesToOrderType FOREIGN KEY (IdObjectType) REFERENCES dbo.OrderTypes(Id),
		[DefaultValue] NVARCHAR(250) NULL
	)

	CREATE NONCLUSTERED INDEX IX_OrderOptionTypes_Name ON dbo.OrderOptionTypes (Name)

	CREATE TABLE [dbo].[OrderOptionValues] (
		[Id] INT NOT NULL
			CONSTRAINT PK_OrderOptionValues PRIMARY KEY (Id) IDENTITY,
		[IdOptionType] INT NOT NULL
			CONSTRAINT FK_OrderOptionValuesToOrderOptionType FOREIGN KEY (IdOptionType) REFERENCES dbo.OrderOptionTypes (Id),
		[IdOrder] INT NOT NULL
			CONSTRAINT FK_OrderOptionValuesToOrder FOREIGN KEY (IdOrder) REFERENCES dbo.Orders (Id),
		[Value] NVARCHAR(250) NULL
	)

	CREATE NONCLUSTERED INDEX IX_OrderOptionValues_Value ON OrderOptionValues (Value)

	CREATE TABLE OrderToSkus (
		[IdOrder] INT NOT NULL
			CONSTRAINT FK_OrderToSkuToOrder FOREIGN KEY (IdOrder) REFERENCES dbo.Orders (Id),
		[IdSku] INT NOT NULL
			CONSTRAINT PK_SkusOrdered PRIMARY KEY (IdOrder DESC, IdSku)
			CONSTRAINT FK_OrderToSkuToSku FOREIGN KEY (IdSku) REFERENCES dbo.Skus (Id),
		[Amount] MONEY NOT NULL,
		[Quantity] MONEY NOT NULL
	)

	CREATE TABLE OrderToGiftCertificates (
		[IdOrder] INT NOT NULL
			CONSTRAINT FK_OrderToGiftCertificateToOrder FOREIGN KEY (IdOrder) REFERENCES dbo.Orders (Id),
		[IdGiftCertificate] INT NOT NULL
			CONSTRAINT PK_GiftCertificatesInOrder PRIMARY KEY (IdOrder DESC, IdGiftCertificate)
			CONSTRAINT FK_OrderToGiftCertificateToGiftCertificate FOREIGN KEY (IdGiftCertificate) REFERENCES dbo.GiftCertificates (Id),
		[Amount] MONEY NOT NULL,
	)

	CREATE TABLE [dbo].[OrderPaymentMethods] (
		[Id] [int] NOT NULL 
			CONSTRAINT PK_OrderPaymentMethods PRIMARY KEY (Id) IDENTITY,
		[IdObjectType] [int] NOT NULL
			CONSTRAINT FK_OrderPaymentMethodToPaymentMethod FOREIGN KEY (IdObjectType) REFERENCES dbo.PaymentMethods (Id),
		[DateCreated] [datetime2] NOT NULL,
		[DateEdited] [datetime2] NOT NULL,
		[IdEditedBy] [int] NULL
			CONSTRAINT FK_OrderPaymentMethodsToUser FOREIGN KEY (IdEditedBy) REFERENCES dbo.Users (Id),
		[StatusCode] INT NOT NULL
			CONSTRAINT FK_OrderPaymentMethodsToStatus FOREIGN KEY (StatusCode) REFERENCES dbo.RecordStatusCodes (StatusCode),
	)

	CREATE TABLE [dbo].[OrderPaymentMethodOptionTypes] (
		[Id] INT NOT NULL
			CONSTRAINT PK_OrderPaymentMethodOptionTypes PRIMARY KEY (Id) IDENTITY,
		[Name] NVARCHAR(250) NOT NULL,
		[IdLookup] INT NULL
			CONSTRAINT FK_OrderPaymentMethodOptionTypesToLookup FOREIGN KEY (IdLookup) REFERENCES dbo.Lookups (Id),
		[IdFieldType] INT NOT NULL
			CONSTRAINT FK_OrderPaymentMethodOptionTypesToFieldType FOREIGN KEY (IdFieldType) REFERENCES dbo.FieldTypes (Id),
		[IdObjectType] INT NULL
			CONSTRAINT FK_OrderPaymentMethodOptionTypesToPaymentMethod FOREIGN KEY (IdObjectType) REFERENCES dbo.PaymentMethods(Id),
		[DefaultValue] NVARCHAR(250) NULL
	)

	CREATE NONCLUSTERED INDEX IX_OrderPaymentMethodOptionTypes_Name ON dbo.OrderPaymentMethodOptionTypes (Name)

	CREATE TABLE [dbo].[OrderPaymentMethodOptionValues] (
		[Id] INT NOT NULL
			CONSTRAINT PK_OrderPaymentMethodOptionValues PRIMARY KEY (Id) IDENTITY,
		[IdOptionType] INT NOT NULL
			CONSTRAINT FK_OrderPaymentMethodOptionValuesToOrderPaymentMethodOptionType FOREIGN KEY (IdOptionType) REFERENCES dbo.OrderPaymentMethodOptionTypes (Id),
		[IdOrderPaymentMethod] INT NOT NULL
			CONSTRAINT FK_OrderPaymentMethodOptionValuesToOrderPaymentMethod FOREIGN KEY (IdOrderPaymentMethod) REFERENCES dbo.OrderPaymentMethods (Id),
		[Value] NVARCHAR(250) NULL
	)

	CREATE NONCLUSTERED INDEX IX_OrderPaymentMethodOptionValues_Value ON OrderPaymentMethodOptionValues (Value)

	CREATE TABLE [dbo].[OrderAddresses] (
		[Id] INT NOT NULL
			CONSTRAINT PK_OrderAddresses PRIMARY KEY (Id) IDENTITY,
		[IdOrder] INT NOT NULL
			CONSTRAINT FK_OrderAddressToOrder FOREIGN KEY (IdOrder) REFERENCES dbo.Orders (Id),
		[IdCountry] INT NOT NULL
			CONSTRAINT FK_OrderAddressesToCountry FOREIGN KEY (IdCountry) REFERENCES dbo.Countries (Id),
		[IdState] INT NOT NULL
			CONSTRAINT FK_OrderAddressesToState FOREIGN KEY (IdState) REFERENCES dbo.States (Id),
		[IdObjectType] INT NOT NULL
			CONSTRAINT FK_OrderAddressesToAddressType FOREIGN KEY (IdObjectType) REFERENCES dbo.AddressTypes (Id),
		[County] NVARCHAR(250) NOT NULL,
		[DateCreated] [datetime2](7) NOT NULL,
		[DateEdited] [datetime2](7) NOT NULL,
		[IdEditedBy] [int] NULL
			CONSTRAINT FK_OrderAddressesToUser FOREIGN KEY (IdEditedBy) REFERENCES dbo.Users (Id),
		[StatusCode] INT NOT NULL
			CONSTRAINT FK_OrderAddressesToRecordStatusCode FOREIGN KEY (StatusCode) REFERENCES dbo.RecordStatusCodes (StatusCode)
	)

	CREATE TABLE [dbo].[OrderAddressOptionValues] (
		[Id] INT NOT NULL
			CONSTRAINT PK_OrderAddressOptionValues PRIMARY KEY (Id) IDENTITY,
		[IdOptionType] INT NOT NULL
			CONSTRAINT FK_OrderAddressOptionValuesToAddressOptionType FOREIGN KEY (IdOptionType) REFERENCES dbo.AddressOptionTypes (Id),
		[IdOrderAddress] INT NOT NULL
			CONSTRAINT FK_OrderAddressOptionValuesToOrderAddress FOREIGN KEY (IdOrderAddress) REFERENCES dbo.OrderAddresses (Id),
		[Value] NVARCHAR(250) NULL
	)

	CREATE NONCLUSTERED INDEX IX_OrderAddressOptionValues_Value ON OrderAddressOptionValues (Value)

END

IF OBJECT_ID(N'[dbo].[OrderPaymentMethodOptionTypes]', N'U') IS NOT NULL
BEGIN
	ALTER TABLE [OrderPaymentMethodOptionValues]
	DROP CONSTRAINT FK_OrderPaymentMethodOptionValuesToOrderPaymentMethodOptionType

	DROP TABLE [OrderPaymentMethodOptionTypes]

	ALTER TABLE [OrderPaymentMethodOptionValues]
	ADD CONSTRAINT FK_OrderPaymentMethodOptionValuesToCustomerPaymentMethodOptionType FOREIGN KEY (IdOptionType) REFERENCES dbo.CustomerPaymentMethodOptionTypes (Id)
END

IF OBJECT_ID(N'[dbo].[ReshipProblemSkus]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[ReshipProblemSkus] (
		IdOrder INT NOT NULL
			CONSTRAINT FK_ReshipProblemSkusToOrder FOREIGN KEY (IdOrder) REFERENCES dbo.Orders (Id),
		IdSku INT NOT NULL
			CONSTRAINT FK_ReshipProblemSkusToSku FOREIGN KEY (IdSku) REFERENCES dbo.Skus (Id)
			CONSTRAINT PK_ReshipProblemSkus PRIMARY KEY (IdOrder DESC, IdSku)
	)
END

IF OBJECT_ID(N'[dbo].[RefundSkus]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[RefundSkus] (
		IdOrder INT NOT NULL
			CONSTRAINT FK_RefundSkusToOrder FOREIGN KEY (IdOrder) REFERENCES dbo.Orders (Id),
		IdSku INT NOT NULL
			CONSTRAINT FK_RefundSkusToSku FOREIGN KEY (IdSku) REFERENCES dbo.Skus (Id)
			CONSTRAINT PK_RefundSkus PRIMARY KEY (IdOrder DESC, IdSku),
		Redeem INT NOT NULL,
		Quantity INT NOT NULL,
		RefundValue MONEY NOT NULL,
		RefundPercent FLOAT NOT NULL
	)
END

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'IdPaymentMethod' AND [object_id] = OBJECT_ID(N'[dbo].[Orders]', N'U'))
BEGIN

	ALTER TABLE dbo.Orders
	ADD IdPaymentMethod INT NULL
		CONSTRAINT FK_OrderToOrderPaymentMethod FOREIGN KEY (IdPaymentMethod) REFERENCES dbo.OrderPaymentMethods(Id),
	IdShippingAddress INT NULL
		CONSTRAINT FK_OrderToOrderShippingAddress FOREIGN KEY (IdShippingAddress) REFERENCES dbo.OrderAddresses (Id)
END

IF EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'OrderToSkus') AND Name = N'Quantity' AND system_type_id = TYPE_ID(N'MONEY'))
BEGIN
	ALTER TABLE OrderToSkus
	DROP COLUMN Quantity

	ALTER TABLE OrderToSkus
	ADD Quantity INT NULL

	UPDATE OrderToSkus
	SET Quantity = 0

	ALTER TABLE OrderToSkus
	ALTER COLUMN Quantity INT NOT NULL
END

IF EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'OrderAddresses') AND Name = N'County' AND is_nullable = 0)
BEGIN
	ALTER TABLE OrderAddresses
	ALTER COLUMN County NVARCHAR(250) NULL

	ALTER TABLE OrderAddresses
	DROP CONSTRAINT FK_OrderAddressToOrder,
		COLUMN IdOrder
END

IF NOT EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'OrderPaymentMethods') AND Name = N'IdAddress')
BEGIN
	ALTER TABLE OrderPaymentMethods
	ADD IdAddress INT NULL
	CONSTRAINT FK_PaymentMethodToAddress FOREIGN KEY (IdAddress) REFERENCES OrderAddresses (Id)
END

GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'Orders') AND Name = N'IdAddedBy')
BEGIN
	ALTER TABLE Orders
	ADD IdAddedBy INT NULL
		CONSTRAINT FK_OrdersAddedToUser FOREIGN KEY (IdEditedBy) REFERENCES dbo.Users (Id)
END

GO

IF EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'OrderAddresses') AND Name = N'IdState' AND is_nullable = 0)
BEGIN
	ALTER TABLE OrderAddresses
	ALTER COLUMN IdState INT NULL
END

GO

IF EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'Orders') AND Name = N'OrderStatus' AND is_nullable = 0)
BEGIN
	ALTER TABLE Orders
	ALTER COLUMN OrderStatus INT NULL

	ALTER TABLE Orders
	ADD POrderStatus INT NULL
		CONSTRAINT FK_OrdersPOrderStatusToUser FOREIGN KEY (POrderStatus) REFERENCES dbo.OrderStatuses (Id)

	ALTER TABLE Orders
	ADD NPOrderStatus INT NULL
		CONSTRAINT FK_OrdersNPOrderStatusToUser FOREIGN KEY (NPOrderStatus) REFERENCES dbo.OrderStatuses (Id)
END

GO

IF EXISTS(SELECT * FROM OrderStatuses WHERE Name='Ship Delayed')
BEGIN
	UPDATE OrderStatuses
	SET Name='Ship Delay'
	WHERE Name='Ship Delayed' 
END

GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'Orders') AND Name = N'IdOrderSource')
BEGIN

	ALTER TABLE Orders
	ADD IdOrderSource INT NULL
		CONSTRAINT FK_OrderToOrderSource FOREIGN KEY (IdOrderSource) REFERENCES dbo.Orders (Id)

END

GO

IF OBJECT_ID(N'[dbo].[RefundOrderToGiftCertificates]', N'U') IS NULL
BEGIN
	CREATE TABLE RefundOrderToGiftCertificates (
		[IdRefundOrder] INT NOT NULL		
			CONSTRAINT FK_RefundOrderToGiftCertificateToOrder FOREIGN KEY (IdRefundOrder) REFERENCES dbo.Orders (Id),
		[IdOrder] INT NOT NULL,
		[IdGiftCertificate] INT NOT NULL
			CONSTRAINT PK_RefundOrderToGiftCertificates PRIMARY KEY (IdRefundOrder DESC, IdOrder DESC, IdGiftCertificate),
			CONSTRAINT FK_RefundOrderToGiftCertificateToOrderToGiftCertificate FOREIGN KEY (IdOrder, IdGiftCertificate) REFERENCES dbo.OrderToGiftCertificates (IdOrder, IdGiftCertificate),
		[Amount] MONEY NOT NULL,
	)

	CREATE NONCLUSTERED INDEX IX_RefundOrderToGiftCertificates_IdRefundOrder ON RefundOrderToGiftCertificates (IdRefundOrder)

END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'RefundPrice' AND [object_id] = OBJECT_ID(N'[dbo].[RefundSkus]', N'U'))
BEGIN

	ALTER TABLE dbo.RefundSkus
	ADD RefundPrice MONEY NULL

	EXEC('UPDATE dbo.RefundSkus SET RefundPrice=0')

	ALTER TABLE dbo.RefundSkus
	ALTER COLUMN RefundPrice MONEY NOT NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_Orders_IdOrderSource')
BEGIN

	CREATE NONCLUSTERED INDEX IX_Orders_IdOrderSource ON Orders (IdOrderSource)

END

GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_UQ_NameTypeOrderOption')
BEGIN
	CREATE UNIQUE NONCLUSTERED INDEX [IX_UQ_NameTypeOrderOption] ON [dbo].[OrderOptionTypes]
	(
		[Name] ASC,
		[IdObjectType] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
END

GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_UQ_NameTypeAddressOption')
BEGIN
	CREATE UNIQUE NONCLUSTERED INDEX IX_UQ_NameTypeAddressOption ON [dbo].[AddressOptionTypes]
	(
		[Name] ASC,
		[IdObjectType] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
END
GO

IF OBJECT_ID(N'[dbo].[OrderShippingPackages]', N'U') IS NULL
BEGIN

	CREATE TABLE [dbo].[OrderShippingPackages] (
		[IdOrder] [int] NOT NULL		
			CONSTRAINT FK_OrderShippingPackageToOrder FOREIGN KEY (IdOrder) REFERENCES dbo.Orders (Id),
		[IdSku] [int] NOT NULL
			CONSTRAINT PK_OrderShippingPackages PRIMARY KEY (IdOrder DESC, IdSku ASC),
			CONSTRAINT FK_OrderShippingPackageToSku FOREIGN KEY (IdSku) REFERENCES dbo.Skus (Id),
		[DateCreated] [datetime2] NOT NULL,
		[POrderType] [int] NULL,
		[ShipMethodFreightCarrier] nvarchar(500) NOT NULL,
		[ShipMethodFreightService] nvarchar(500) NOT NULL,
		[ShippedDate] [datetime2] NOT NULL,
		[TrackingNumber] nvarchar(500) NOT NULL,
		[UPSServiceCode] nvarchar(500) NOT NULL,
		[IdWarehouse] int NOT NULL
	)
	
	CREATE NONCLUSTERED INDEX IX_OrderShippingPackage_IdOrder ON OrderShippingPackages ([IdOrder] DESC)
	CREATE NONCLUSTERED INDEX IX_OrderShippingPackage_IdOrder_POrderType ON OrderShippingPackages ([IdOrder] DESC, [POrderType])
	CREATE NONCLUSTERED INDEX IX_OrderShippingPackage_ShippedDate ON OrderShippingPackages ([ShippedDate] DESC)
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'PAmount' AND [object_id] = OBJECT_ID(N'[dbo].[OrderToGiftCertificates]', N'U'))
BEGIN

	ALTER TABLE dbo.OrderToGiftCertificates
	ADD PAmount MONEY NULL

	ALTER TABLE dbo.OrderToGiftCertificates
	ADD NPAmount MONEY NULL
END
GO