DELETE AddressOptionValues
WHERE IdAddress IN
(SELECT Id FROM Addresses
WHERE IdCustomer IN
(SELECT Id FROM Customers))

DELETE CustomerOptionValues
WHERE IdCustomer IN
(SELECT Id FROM Customers)

DELETE CustomerPaymentMethodValues

DELETE CustomerPaymentMethods

DELETE Addresses
WHERE IdCustomer IN
(SELECT Id FROM Customers)

DELETE CustomersToPaymentMethods

DELETE CustomerNoteOptionValues

DELETE CustomerNotes

DELETE CustomerFiles

DELETE CustomersToOrderNotes

DELETE Customers
