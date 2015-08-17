IF NOT EXISTS(SELECT * FROM OrderOptionTypes)
BEGIN
	INSERT INTO OrderTypes
	(Id, Name)
	VALUES
	(1, 'Normal Order'),
	(2, 'Auto-Ship'),
	(3, 'Drop-Ship'),
	(4, 'Gift-List'),
	(5, 'Reship'),
	(6, 'Refund')

	DECLARE @IdLookupUpgrade INT, @IdLookupPrefShipmethod INT, @IdLookupShipDelay INT
	
	INSERT INTO [dbo].[Lookups]
	([LookupValueType], Name)
	VALUES
	(N'string', N'ShippingUpgrade')

	SET @IdLookupUpgrade = SCOPE_IDENTITY()

	INSERT INTO [dbo].[LookupVariants]
	([Id], [IdLookup], [ValueVariant])
	VALUES
	(1, @IdLookupUpgrade, 'Overnight'),
	(2, @IdLookupUpgrade, '2nd Day')

	INSERT INTO [dbo].[Lookups]
	([LookupValueType], Name)
	VALUES
	(N'string', N'PreferredShipMethod')

	SET @IdLookupPrefShipmethod = SCOPE_IDENTITY()

	INSERT INTO [dbo].[LookupVariants]
	([Id], [IdLookup], [ValueVariant])
	VALUES
	(1, @IdLookupPrefShipmethod, 'Best'),
	(2, @IdLookupPrefShipmethod, 'FedEx'),
	(3, @IdLookupPrefShipmethod, 'UPS'),
	(4, @IdLookupPrefShipmethod, 'USPS'),
	(5, @IdLookupPrefShipmethod, 'OnTrac')

	INSERT INTO [dbo].[Lookups]
	([LookupValueType], Name)
	VALUES
	(N'string', N'ShipDelayType')

	SET @IdLookupShipDelay = SCOPE_IDENTITY()

	INSERT INTO [dbo].[LookupVariants]
	([Id], [IdLookup], [ValueVariant])
	VALUES
	(1, @IdLookupShipDelay, 'WholeOrder'),
	(2, @IdLookupShipDelay, 'Separate')

	INSERT INTO [dbo].[OrderOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'GiftOrder', 5, NULL, 1, N'False'),
	(N'MailOrder', 5, NULL, 1, N'False'),
	(N'ShipDelayType', 3, @IdLookupShipDelay, 1, NULL),
	(N'ShipDelayDateP', 6, NULL, 1, NULL),
	(N'ShipDelayDateNP', 6, NULL, 1, NULL),
	(N'KeyCode', 4, NULL, 1, NULL),
	(N'ShippingUpgradeP', 3, @IdLookupUpgrade, 1, NULL),
	(N'ShippingUpgradeNP', 3, @IdLookupUpgrade, 1, NULL),
	(N'ShippingOverride', 1, NULL, 1, NULL),
	(N'SurchargeOverride', 1, NULL, 1, NULL),
	(N'PreferredShipMethod', 3, @IdLookupPrefShipmethod, 1, N'1'),
	(N'OrderNotes', 4, NULL, 1, NULL),
	(N'GiftMessage', 4, NULL, 1, NULL),
	(N'DeliveryInstructions', 4, NULL, 1, NULL),
	(N'PoNumber', 4, NULL, 3, NULL)

	INSERT INTO [dbo].[OrderOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'GiftOrder', 5, NULL, 2, N'False'),
	(N'MailOrder', 5, NULL, 2, N'False'),
	(N'ShipDelayType', 3, @IdLookupShipDelay, 2, NULL),
	(N'ShipDelayDateP', 6, NULL, 2, NULL),
	(N'ShipDelayDateNP', 6, NULL, 2, NULL),
	(N'KeyCode', 4, NULL, 2, NULL),
	(N'ShippingUpgradeP', 3, @IdLookupUpgrade, 2, NULL),
	(N'ShippingUpgradeNP', 3, @IdLookupUpgrade, 2, NULL),
	(N'ShippingOverride', 1, NULL, 2, NULL),
	(N'SurchargeOverride', 1, NULL, 2, NULL),
	(N'PreferredShipMethod', 3, @IdLookupPrefShipmethod, 2, N'1'),
	(N'OrderNotes', 4, NULL, 2, NULL),
	(N'GiftMessage', 4, NULL, 2, NULL),
	(N'DeliveryInstructions', 4, NULL, 2, NULL),
	(N'PoNumber', 4, NULL, 2, NULL),
	(N'AutoShipFrequency', 3, NULL, 2, NULL)

	INSERT INTO [dbo].[OrderOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'ShipDelayType', 3, @IdLookupShipDelay, 3, NULL),
	(N'ShipDelayDate', 6, NULL, 3, NULL),
	(N'KeyCode', 4, NULL, 3, NULL),
	(N'OrderNotes', 4, NULL, 3, NULL),
	(N'GiftMessage', 4, NULL, 3, NULL),
	(N'PoNumber', 4, NULL, 3, NULL)

	INSERT INTO [dbo].[OrderOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'ShipDelayType', 3, @IdLookupShipDelay, 4, NULL),
	(N'ShipDelayDate', 6, NULL, 4, NULL),
	(N'KeyCode', 4, NULL, 4, NULL),
	(N'OrderNotes', 4, NULL, 4, NULL),
	(N'GiftMessage', 4, NULL, 4, NULL)

	DECLARE @IdLookupServiceCode INT

	INSERT INTO [dbo].[Lookups]
	([LookupValueType], Name)
	VALUES
	(N'string', N'ServiceCode')

	SET @IdLookupServiceCode = SCOPE_IDENTITY()

	INSERT INTO [dbo].[LookupVariants]
	([Id], [IdLookup], [ValueVariant])
	VALUES
	(1, @IdLookupServiceCode, N'Carrier - Damage'),
	(2, @IdLookupServiceCode, N'Carrier Late'),
	(3, @IdLookupServiceCode, N'Carrier Not Received'),
	(4, @IdLookupServiceCode, N'Entry - Agent'),
	(5, @IdLookupServiceCode, N'Entry - Customer'),
	(6, @IdLookupServiceCode, N'Misc See Note'),
	(7, @IdLookupServiceCode, N'Pick/Pack Other'),
	(8, @IdLookupServiceCode, N'Pick/Pack Wrong SKU'),
	(9, @IdLookupServiceCode, N'Product - NAE'),
	(10, @IdLookupServiceCode, N'Product - Quality'),
	(11, @IdLookupServiceCode, N'Product Labeling/PrePack'),
	(12, @IdLookupServiceCode, N'Technical Issue'),
	(13, @IdLookupServiceCode, N'Customer Accommodation'),
	(14, @IdLookupServiceCode, N'Thawed (Not Late)'),
	(15, @IdLookupServiceCode, N'Drop Ship')

	INSERT INTO [dbo].[OrderOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'ShipDelayType', 3, @IdLookupShipDelay, 5, NULL),
	(N'ShipDelayDateP', 6, NULL, 5, NULL),
	(N'ShipDelayDateNP', 6, NULL, 5, NULL),
	(N'ServiceCode', 3, @IdLookupServiceCode, 5, NULL),
	(N'ShippingUpgradeP', 3, @IdLookupUpgrade, 5, NULL),
	(N'ShippingUpgradeNP', 3, @IdLookupUpgrade, 5, NULL),
	(N'ShippingOverride', 1, NULL, 5, NULL),
	(N'SurchargeOverride', 1, NULL, 5, NULL),
	(N'OrderNotes', 4, NULL, 5, NULL),
	(N'ReturnAssociated', 5, NULL, 5, N'False')

	INSERT INTO [dbo].[OrderOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'ServiceCode', 3, @IdLookupServiceCode, 6, NULL),	
	(N'OrderNotes', 4, NULL, 6, NULL),
	(N'ReturnAssociated', 5, NULL, 6, N'False'),
	(N'ShippingRefunded', 5, NULL, 6, NULL),
	(N'ManualRefundOverride', 1, NULL, 6, NULL),
	(N'IdOrderRefunded', 3, NULL, 6, NULL)
END

GO

IF NOT EXISTS(SELECT * FROM OrderStatuses)
BEGIN

	INSERT INTO [dbo].[OrderStatuses]
	([Id], [Name])
	VALUES
	(1, N'Incomplete'),	
	(2, N'Processed'),	
	(3, N'Shipped'),	
	(4, N'Cancelled'),	
	(5, N'Exported'),	
	(6, N'Ship Delayed'),	
	(7, N'On Hold')

END 

GO