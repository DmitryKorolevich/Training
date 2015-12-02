IF OBJECT_ID(N'dbo.OrderPaymentMethods') IS NULL
BEGIN
	CREATE TABLE dbo.OrderPaymentMethods
	(
		IdOrder INT NOT NULL,
		CreditCardNumber VARBINARY(MAX) NOT NULL,
		CONSTRAINT PK_OrderPaymentMethods_IdOrder PRIMARY KEY CLUSTERED (IdOrder DESC)
	)
END

IF OBJECT_ID(N'dbo.CustomerPaymentMethods') IS NULL
BEGIN
	CREATE TABLE dbo.CustomerPaymentMethods
	(
		IdCustomer INT NOT NULL,
		IdPaymentMethod INT NOT NULL,
		CreditCardNumber VARBINARY(MAX) NOT NULL,
		CONSTRAINT PK_CustomerPaymentMethods_IdCustomerIdPaymentMethod PRIMARY KEY CLUSTERED (IdCustomer DESC, IdPaymentMethod DESC)
	)
END