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

IF NOT EXISTS(SELECT [Id] FROM [dbo].[OrderOptionTypes] Where Name='AlaskaHawaiiSurcharge')
BEGIN
	INSERT INTO [dbo].[OrderOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'AlaskaHawaiiSurcharge', 1, NULL, 1, NULL),
	(N'CanadaSurcharge', 1, NULL, 1, NULL),	
	(N'StandardShippingCharges', 1, NULL, 1, NULL)

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[OrderOptionTypes] Where Name='OrderType')
BEGIN

	DECLARE @IdLookupOrderTypes INT, @IdLookupPOrderTypes INT
		
	INSERT INTO [dbo].[Lookups]
	([LookupValueType], Name)
	VALUES
	(N'string', N'OrderTypes')
	
	SET @IdLookupOrderTypes = SCOPE_IDENTITY()

	INSERT INTO [dbo].[LookupVariants]
	([Id], [IdLookup], [ValueVariant])
	VALUES
	(1, @IdLookupOrderTypes, 'Web'),
	(2, @IdLookupOrderTypes, 'Phone'),
	(3, @IdLookupOrderTypes, 'Mail Order')
			
	INSERT INTO [dbo].[Lookups]
	([LookupValueType], Name)
	VALUES
	(N'string', N'POrderTypes')
	
	SET @IdLookupPOrderTypes = SCOPE_IDENTITY()
	
	INSERT INTO [dbo].[LookupVariants]
	([Id], [IdLookup], [ValueVariant])
	VALUES
	(1, @IdLookupPOrderTypes, 'P Orders'),
	(2, @IdLookupPOrderTypes, 'NP Orders'),
	(3, @IdLookupPOrderTypes, 'P/NP Orders'),
	(4, @IdLookupPOrderTypes, 'Other')

	INSERT INTO [dbo].[OrderOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'OrderType', 3, @IdLookupOrderTypes, NULL, NULL),
	(N'POrderType', 3, @IdLookupPOrderTypes, NULL, NULL)

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[OrderOptionTypes] Where Name='ShipDelayDate' AND IdObjectType=1)
BEGIN

	INSERT INTO [dbo].[OrderOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'ShipDelayDate', 6, NULL, 1, NULL)

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[OrderOptionTypes] Where Name='AutoShipFrequency' AND IdObjectType=1)
BEGIN

	INSERT INTO [dbo].[OrderOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'AutoShipFrequency', 3, NULL, 1, NULL)

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[OrderOptionTypes] Where Name='IgnoneMinimumPerishableThreshold' AND IdObjectType=1)
BEGIN

	INSERT INTO [dbo].[OrderOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'IgnoneMinimumPerishableThreshold', 5, NULL, 1, 'False')

END

GO

IF NOT EXISTS(SELECT * FROM OrderOptionTypes WHERE [IdObjectType]=4 AND [Name]=N'GiftOrder')
BEGIN

	INSERT INTO [dbo].[OrderOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'GiftOrder', 5, NULL, 4, N'True'),
	(N'MailOrder', 5, NULL, 4, N'False'),
	(N'ShipDelayDateP', 6, NULL, 4, NULL),
	(N'ShipDelayDateNP', 6, NULL, 4, NULL),
	(N'ShippingUpgradeP', 3, (SELECT Id FROM [dbo].[Lookups] WHERE Name='ShippingUpgrade'), 4, NULL),
	(N'ShippingUpgradeNP', 3, (SELECT Id FROM [dbo].[Lookups] WHERE Name='ShippingUpgrade'), 4, NULL),
	(N'ShippingOverride', 1, NULL, 4, NULL),
	(N'SurchargeOverride', 1, NULL, 4, NULL),
	(N'PreferredShipMethod', 3, (SELECT Id FROM [dbo].[Lookups] WHERE Name='PreferredShipMethod'), 4, N'1'),
	(N'DeliveryInstructions', 4, NULL, 4, NULL),	
	(N'AlaskaHawaiiSurcharge', 1, NULL, 4, NULL),
	(N'CanadaSurcharge', 1, NULL, 4, NULL),	
	(N'StandardShippingCharges', 1, NULL, 4, NULL)

END
GO

IF ((SELECT count(*) FROM OrderOptionTypes WHERE [Name]=N'PoNumber' AND [IdObjectType]=3)>1)
BEGIN
	DELETE OrderOptionTypes
	WHERE [Id]=(SELECT TOP 1 [Id] FROM OrderOptionTypes WHERE [Name]=N'PoNumber' AND [IdObjectType]=3)

	INSERT INTO [dbo].[OrderOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'PoNumber', 4, NULL, 1, NULL)
END
GO

IF NOT EXISTS(SELECT * FROM OrderOptionTypes WHERE Name=N'IdDiscountTier')
BEGIN

	INSERT INTO [dbo].[OrderOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'IdDiscountTier', 3, NULL, NULL, NULL)

END
GO

IF EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'OrderToPromos') AND Name = N'Disabled' AND is_nullable = 1)
BEGIN
	UPDATE OrderToPromos
	SET [Disabled] = 0

	ALTER TABLE OrderToPromos
	ALTER COLUMN [Disabled] BIT NOT NULL
END
GO

IF NOT EXISTS(SELECT * FROM OrderOptionTypes WHERE Name='ConfirmationEmailSent')
BEGIN

	INSERT INTO [dbo].[OrderOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'ConfirmationEmailSent', 5, NULL, NULL, 'False')

END
GO

IF EXISTS(SELECT * FROM OrderTypes WHERE Name='Normal Order')
BEGIN

UPDATE OrderTypes 
SET Name='Standard'
WHERE Name='Normal Order'

END

GO

IF EXISTS(SELECT * FROM Lookups WHERE Name='ServiceCode')
BEGIN

	UPDATE Lookups
	SET Name='ServiceCodes',
	[Description]='Service Codes'
	WHERE Name='ServiceCode'	

END

GO

IF EXISTS(SELECT [Id] FROM [dbo].[OrderOptionTypes] WHERE Name = 'DeliveryInstructions' OR Name=N'PreferredShipMethod')
BEGIN

	DELETE OrderOptionValues
	WHERE IdOptionType IN
	(SELECT Id FROM [dbo].[OrderOptionTypes] 
	WHERE Name = 'DeliveryInstructions' OR Name=N'PreferredShipMethod')

	DELETE [dbo].[OrderOptionTypes] 
	WHERE Name = 'DeliveryInstructions' OR Name=N'PreferredShipMethod'
END

GO

IF NOT EXISTS(SELECT * FROM Lookups WHERE Name='RefundRedeemOptions')
BEGIN
	DECLARE @IdLookup INT
	
	INSERT INTO [dbo].[Lookups]
	([LookupValueType], Name)
	VALUES
	(N'string', N'RefundRedeemOptions')
		
	SET @IdLookup = SCOPE_IDENTITY()
	
	INSERT INTO [dbo].[LookupVariants]
	([Id], [IdLookup], [ValueVariant], [Order])
	VALUES
	(1, @IdLookup, 'Refund',1),
	(2, @IdLookup, 'Return',2),
	(3, @IdLookup, 'Discount',3),
	(4, @IdLookup, 'Other',4)
END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[OrderOptionTypes] WHERE Name = 'AutoTotal' AND [IdObjectType]=6)
BEGIN
	INSERT INTO [dbo].[OrderOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'AutoTotal', 1, NULL, 6, NULL)
END


IF NOT EXISTS(SELECT [Id] FROM [dbo].[OrderOptionTypes] WHERE Name = N'Guest' AND IdObjectType=1)
BEGIN
	INSERT INTO [dbo].[OrderOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'Guest', 5, NULL, 1, N'False')
END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[OrderOptionTypes] Where Name='LastAutoShipDate' AND IdObjectType=2)
BEGIN

	INSERT INTO [dbo].[OrderOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'LastAutoShipDate', 6, NULL, 2, NULL)

END

GO

IF NOT EXISTS(SELECT [Id] FROM OrderTypes WHERE Id = 7)
BEGIN
	INSERT INTO OrderTypes
	(Id, Name)
	VALUES
	(7, 'Auto-Ship Order')

	INSERT INTO OrderOptionTypes ([Name]
      ,[IdLookup]
      ,[IdFieldType]
      ,[IdObjectType]
      ,[DefaultValue])
	SELECT [Name], [IdLookup], [IdFieldType], 7, [DefaultValue] FROM OrderOptionTypes WHERE [IdObjectType] = 1
END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[OrderOptionTypes] Where Name='AutoShipId' AND IdObjectType=7)
BEGIN

	INSERT INTO [dbo].[OrderOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'AutoShipId', 3, NULL, 7, NULL)

END

IF NOT EXISTS(SELECT [Id] FROM [dbo].[OrderOptionTypes] Where Name='ShipDelayDate' AND IdObjectType=5)
BEGIN

	INSERT INTO [dbo].[OrderOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'ShipDelayDate', 6, NULL, 5, NULL)

END

GO