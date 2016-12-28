IF NOT EXISTS(SELECT * FROM OrderReviewRuleOptionTypes)
BEGIN

	INSERT INTO [dbo].[OrderReviewRuleOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'MinOrderTotal', 1, NULL, NULL, NULL),
	(N'Guest', 5, NULL, NULL, 'False'),
	(N'DeliveryInstructionForSearch', 4, NULL, NULL, NULL),
	(N'ZipForSearch', 4, NULL, NULL, NULL),
	(N'SkuForSearch', 4, NULL, NULL, NULL),
	(N'CompareNames', 5, NULL, NULL, 'False'),
	(N'CompareNamesType', 3, NULL, NULL, NULL),
	(N'CompareAddresses', 5, NULL, NULL, 'False'),
	(N'CompareAddressesType', 3, NULL, NULL, NULL),
	(N'ReshipsRefundsCheck', 5, NULL, NULL, 'False'),
	(N'ReshipsRefundsQTY', 3, NULL, NULL, NULL),	
	(N'ReshipsRefundsCheck', 5, NULL, NULL, NULL),
	(N'ReshipsRefundsMonthCount', 5, NULL, NULL, NULL)

END
GO