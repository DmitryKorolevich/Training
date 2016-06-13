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

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Customers]') AND name = N'IX_Email')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Email] ON [dbo].[Customers]
	(
		[Email] ASC, [StatusCode] ASC
	)
	INCLUDE ( 	[Id],
		[IdObjectType],
		[DateCreated],
		[DateEdited],
		[IdEditedBy],
		[IdDefaultPaymentMethod],
		[PublicId],
		[IdAffiliate],
		[IdProfileAddress]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
END
GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Customers]') AND name = N'IX_ProfileAddress')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_ProfileAddress] ON [dbo].[Customers]
	(
		IdProfileAddress DESC
	)
	INCLUDE ( 	[Id],
		[IdObjectType],
		[DateCreated],
		[DateEdited],
		[IdEditedBy],
		[IdDefaultPaymentMethod],
		[StatusCode],
		[PublicId],
		[IdAffiliate],
		Email) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
END
GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CustomerToShippingAddresses]') AND name = N'IX_IdAddress')
BEGIN
	CREATE NONCLUSTERED INDEX IX_IdAddress
	ON [dbo].[CustomerToShippingAddresses] ([IdAddress])
	INCLUDE ([IdCustomer])
END

GO

IF EXISTS(SELECT * FROM sys.columns WHERE Name = 'IdCountry' AND [object_id] = OBJECT_ID('Addresses') AND is_nullable = 0)
BEGIN

ALTER TABLE Addresses
ALTER COLUMN IdCountry INT NULL

END

GO
