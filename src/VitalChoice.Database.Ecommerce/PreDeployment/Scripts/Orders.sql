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

END