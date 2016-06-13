IF EXISTS( SELECT * FROM AddressOptionTypes WHERE Name ='Address1' AND IdObjectType IS  NULL)
BEGIN
	
	INSERT AddressOptionTypes
	(Name, IdFieldType, IdLookup, IdObjectType, DefaultValue)	
	VALUES
	('Address1', 4,	NULL, 2, NULL),
	('Address2', 4,	NULL, 2, NULL),
	('City', 4,	NULL, 2, NULL),
	('Zip', 4,	NULL, 2, NULL),
	('Address1', 4,	NULL, 3, NULL),
	('Address2', 4,	NULL, 3, NULL),
	('City', 4,	NULL, 3, NULL),
	('Zip', 4,	NULL, 3 , NULL),
	('Address1', 4,	NULL, 4, NULL),
	('Address2', 4,	NULL, 4, NULL),
	('City', 4,	NULL, 4, NULL),
	('Zip', 4,	NULL, 4, NULL)

	UPDATE v
	SET v.IdOptionType=st.Id
	FROM CatalogRequestAddressOptionValues v
	JOIN CatalogRequestAddresses a ON a.Id=v.IdCatalogRequestAddress
	JOIN AddressOptionTypes t ON v.IdOptionType=t.Id
	JOIN AddressOptionTypes st ON st.Name=t.Name AND st.IdObjectType=a.IdObjectType
	WHERE t.IdObjectType IS NULL AND t.Name IN 
	('Address1', 'Address2', 'City', 'Zip')

	UPDATE v
	SET v.IdOptionType=st.Id
	FROM OrderAddressOptionValues v
	JOIN OrderAddresses a ON a.Id=v.IdOrderAddress
	JOIN AddressOptionTypes t ON v.IdOptionType=t.Id
	JOIN AddressOptionTypes st ON st.Name=t.Name AND st.IdObjectType=a.IdObjectType
	WHERE t.IdObjectType IS NULL AND t.Name IN 
	('Address1', 'Address2', 'City', 'Zip') AND a.IdObjectType IN (2,3)

	UPDATE v
	SET v.IdOptionType=st.Id
	FROM AddressOptionValues v
	JOIN CustomerToShippingAddresses a ON a.IdAddress=v.IdAddress
	JOIN Addresses ad ON a.IdAddress=ad.Id
	JOIN AddressOptionTypes t ON v.IdOptionType=t.Id
	JOIN AddressOptionTypes st ON st.Name=t.Name AND st.IdObjectType=ad.IdObjectType
	WHERE t.IdObjectType IS NULL AND t.Name IN 
	('Address1', 'Address2', 'City', 'Zip') AND ad.IdObjectType IN (2,3)

	UPDATE v
	SET v.IdOptionType=st.Id
	FROM AddressOptionValues v
	JOIN CustomerPaymentMethods a ON a.IdAddress=v.IdAddress
	JOIN Addresses ad ON a.IdAddress=ad.Id
	JOIN AddressOptionTypes t ON v.IdOptionType=t.Id
	JOIN AddressOptionTypes st ON st.Name=t.Name AND st.IdObjectType=ad.IdObjectType
	WHERE t.IdObjectType IS NULL AND t.Name IN 
	('Address1', 'Address2', 'City', 'Zip') AND ad.IdObjectType IN (2,3)

	DELETE v
	FROM AddressOptionValues v
	JOIN AddressOptionTypes t ON v.IdOptionType=t.Id
	WHERE t.IdObjectType IS NULL AND t.Name IN 
	('Address1', 'Address2', 'City', 'Zip')

	DELETE v
	FROM OrderAddressOptionValues v	 
	JOIN AddressOptionTypes t ON v.IdOptionType=t.Id
	WHERE t.IdObjectType IS NULL AND t.Name IN 
	('Address1', 'Address2', 'City', 'Zip')

	DELETE AddressOptionTypes
	WHERE IdObjectType IS NULL AND Name IN
	('Address1', 'Address2', 'City', 'Zip')

END

GO
