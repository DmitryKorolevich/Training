IF OBJECT_ID(N'[dbo].[Orders]', N'U') IS NULL
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
			CONSTRAINT PK_Orders PRIMARY KEY IDENTITY,
		[IdObjectType] [int] NOT NULL
			CONSTRAINT FK_OrderType FOREIGN KEY REFERENCES dbo.OrderTypes (Id),
		[DateCreated] [datetime2](7) NOT NULL,
		[DateEdited] [datetime2](7) NOT NULL,
		[IdEditedBy] [int] NULL
			CONSTRAINT FK_OrdersToUser FOREIGN KEY REFERENCES dbo.Users (Id),
		[StatusCode] INT NOT NULL
			CONSTRAINT FK_OrdersToStatuses FOREIGN KEY REFERENCES dbo.RecordStatusCodes (StatusCode),
		[OrderStatus] INT NOT NULL
			CONSTRAINT FK_OrdersToOrderStatuses FOREIGN KEY REFERENCES dbo.OrderStatuses (Id),
		[IdCustomer] INT NOT NULL,
			CONSTRAINT FK_OrdersToCustomer FOREIGN KEY REFERENCES dbo.Customers (Id),
		[IdDiscount] INT NULL,
			CONSTRAINT FK_OrdersToDiscount FOREIGN KEY REFERENCES dbo.Discounts (Id),
		[Total] MONEY NOT NULL,
		[ProductsSubtotal] MONEY NOT NULL,
		[TaxTotal] MONEY NOT NULL,
		[ShippingTotal] MONEY NOT NULL,
		[DiscountTotal] MONEY NOT NULL
	)

	CREATE NONCLUSTERED INDEX IX_OrderStatus_ObjectStatus_ObjectType_OrderDate ON Orders ([IdObjectType], [DateCreated], [StatusCode], [OrderStatus])


END