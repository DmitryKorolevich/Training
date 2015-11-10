IF NOT EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'Customers') AND name='IdProfileAddress')
BEGIN
	ALTER TABLE Customers
	ADD IdProfileAddress INT NULL
END
GO
IF OBJECT_ID(N'CustomerToShippingAddresses') IS NULL
BEGIN
	CREATE TABLE CustomerToShippingAddresses
	(
		IdCustomer INT NOT NULL
		CONSTRAINT FK_CustomerToShippingToCustomer FOREIGN KEY (IdCustomer) REFERENCES Customers (Id),
		IdAddress INT NOT NULL
		CONSTRAINT FK_CustomerToShippingToShippingAddress FOREIGN KEY (IdAddress) REFERENCES Addresses (Id),
		CONSTRAINT PK_CustomerToShippingAddresses PRIMARY KEY (IdCustomer, IdAddress)
	)
END
GO