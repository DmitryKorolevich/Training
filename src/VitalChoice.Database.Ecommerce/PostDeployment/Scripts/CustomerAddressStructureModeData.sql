IF EXISTS(SELECT * FROM Customers WHERE IdProfileAddress = 0)
BEGIN
	UPDATE Customers
	SET IdProfileAddress = a.Id
	FROM Customers AS c
	INNER JOIN Addresses AS a ON a.IdCustomer = c.Id AND a.IdObjectType = 1

	DELETE FROM CustomerOptionValues
	WHERE IdCustomer IN (SELECT Id FROM Customers WHERE IdProfileAddress = 0)

	DELETE FROM Customers
	WHERE IdProfileAddress = 0

	ALTER TABLE Customers WITH CHECK
	ADD CONSTRAINT FK_CustomerToProfileAddress FOREIGN KEY (IdProfileAddress) REFERENCES Addresses (Id)

	INSERT INTO CustomerToShippingAddresses
	(IdCustomer, IdAddress)
	SELECT IdCustomer, Id FROM Addresses
	WHERE IdObjectType = 3

	ALTER TABLE Addresses
	DROP CONSTRAINT FK_Addresses_Customers

	ALTER TABLE Addresses
	DROP COLUMN IdCustomer
END