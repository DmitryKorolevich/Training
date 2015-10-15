BEGIN TRANSACTION
BEGIN TRY
	DELETE FROM OrderToSkus
	DELETE FROM OrderToGiftCertificates
	DELETE FROM OrderOptionValues
	DELETE FROM Orders
	DELETE FROM OrderPaymentMethodOptionValues
	DELETE FROM OrderPaymentMethods
	DELETE FROM OrderAddressOptionValues
	DELETE FROM OrderAddresses
	DELETE FROM CustomersToOrderNotes
	DELETE FROM CustomersToPaymentMethods
	DELETE FROM CustomerPaymentMethodValues
	DELETE FROM CustomerPaymentMethods
	DELETE FROM AddressOptionValues
	DELETE FROM Addresses
	DELETE FROM CustomerOptionValues
	DECLARE @customers_deleted TABLE (id INT)
	INSERT INTO @customers_deleted
	(id)
	SELECT Id FROM Customers
	DELETE FROM Customers
	DELETE FROM Users WHERE Id IN (SELECT id FROM @customers_deleted)
	SELECT * FROM @customers_deleted
	COMMIT
END TRY
BEGIN CATCH
	ROLLBACK
	SELECT 
        ERROR_NUMBER() AS ErrorNumber,
        ERROR_MESSAGE() AS ErrorMessage;
END CATCH