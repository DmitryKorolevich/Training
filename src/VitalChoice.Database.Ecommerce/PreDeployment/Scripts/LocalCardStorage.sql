IF OBJECT_ID('dbo.LocalCardStorage') IS NULL
BEGIN
	CREATE TABLE LocalCardStorage
	(
		Id INT NOT NULL IDENTITY(1,1)
		CONSTRAINT PK_LocalCardStorage PRIMARY KEY (Id),
		CreditCardNumber VARBINARY(MAX) NULL,
		IdOrderSource INT NULL
		CONSTRAINT FK_SourceCardToOrder FOREIGN KEY (IdOrderSource) REFERENCES Orders(Id),
		IdOrder INT NULL
		CONSTRAINT FK_CardToOrder FOREIGN KEY (IdOrder) REFERENCES Orders(Id),
		IdCustomer INT NULL
		CONSTRAINT FK_CardToCustomer FOREIGN KEY (IdCustomer) REFERENCES Customers(Id),
		IdCustomerPaymentMethod INT NULL
		CONSTRAINT FK_CardToCustomerPaymentMethod FOREIGN KEY (IdCustomerPaymentMethod) REFERENCES CustomerPaymentMethods(Id),
		IsExported BIT NOT NULL
	)

	CREATE NONCLUSTERED INDEX IX_IdCustomer ON LocalCardStorage
	(
		IdCustomer DESC,
		IdCustomerPaymentMethod DESC
	)

	CREATE UNIQUE INDEX UQ_IdCustomerPaymentMethod ON LocalCardStorage
	(
		IdCustomerPaymentMethod DESC
	)

	CREATE NONCLUSTERED INDEX IX_IdOrder ON LocalCardStorage
	(
		IdOrder DESC
	)

	CREATE NONCLUSTERED INDEX IX_IdOrder ON LocalCardStorage
	(
		IdOrder DESC
	)

	CREATE NONCLUSTERED INDEX IX_IsExported ON LocalCardStorage
	(
		IsExported
	)
	INCLUDE (Id, CreditCardNumber, IdOrderSource, IdOrder, IdCustomer, IdCustomerPaymentMethod, IsExported)
END

IF OBJECT_ID(N'dbo.OrderPaymentMethods') IS NULL
BEGIN
	CREATE TABLE dbo.OrderPaymentMethods
	(
		IdOrder INT NOT NULL,
		CONSTRAINT PK_OrderPaymentMethods_IdOrder PRIMARY KEY CLUSTERED (IdOrder DESC)
	)
END

IF OBJECT_ID(N'dbo.CustomerPaymentMethods') IS NULL
BEGIN
	CREATE TABLE dbo.CustomerPaymentMethods
	(
		IdCustomer INT NOT NULL,
		IdPaymentMethod INT NOT NULL,
		CONSTRAINT PK_CustomerPaymentMethods_IdCustomerIdPaymentMethod PRIMARY KEY CLUSTERED (IdCustomer DESC, IdPaymentMethod DESC)
	)
END