IF NOT EXISTS(SELECT * FROM Lookups WHERE Name ='InventorySkuChannels')
BEGIN

	DECLARE @IdLookup INT
	
	INSERT INTO [dbo].[Lookups]
	([LookupValueType], [Name], [Description])
	VALUES
	(N'string', N'InventorySkuChannels', N'Inventory Sku Channels')

	SET @IdLookup = SCOPE_IDENTITY()

	INSERT INTO [dbo].[LookupVariants]
	([Id], [IdLookup], [ValueVariant], [Order])
	VALUES
	(1, @IdLookup,	N'Drop Ship (Retail)' ,1),
	(2, @IdLookup,	N'Drop Ship (Vendor)', 2),
	(3, @IdLookup,	N'Private Label',3 ),
	(4, @IdLookup,	N'Retail', 4),
	(5, @IdLookup,	N'Wholesale',5 )

	INSERT INTO [dbo].[Lookups]
	([LookupValueType], [Name], [Description])
	VALUES
	(N'string', N'InventorySkuProductSources', N'Inventory Sku Product Sources')

	SET @IdLookup = SCOPE_IDENTITY()

	INSERT INTO [dbo].[LookupVariants]
	([Id], [IdLookup], [ValueVariant], [Order])
	VALUES
	(1, @IdLookup,	N'Custom Formulation' ,1),
	(2, @IdLookup,	N'Custom Packaged Single Ingredient', 2),
	(3, @IdLookup,	N'Non-Branded',3 ),
	(4, @IdLookup,	N'Private Label', 4),
	(5, @IdLookup,	N'Reformulation',5 )

	INSERT INTO [dbo].[Lookups]
	([LookupValueType], [Name], [Description])
	VALUES
	(N'string', N'InventorySkuUnitOfMeasures', N'Inventory Unit of Measures')

	SET @IdLookup = SCOPE_IDENTITY()

	INSERT INTO [dbo].[LookupVariants]
	([Id], [IdLookup], [ValueVariant], [Order])
	VALUES
	(1, @IdLookup,	N'Bag' ,1),
	(2, @IdLookup,	N'Bar', 2),
	(3, @IdLookup,	N'Bottle',3 ),
	(4, @IdLookup,	N'Case', 4),
	(5, @IdLookup,	N'Dozen',5 ),
	(6, @IdLookup,	N'Pack' ,6),
	(7, @IdLookup,	N'Packet', 7),
	(8, @IdLookup,	N'Pouch',8 ),
	(9, @IdLookup,	N'Pounds',9),
	(10, @IdLookup,	N'Tin',10),
	(11, @IdLookup,	N'Tray' ,11),
	(12, @IdLookup,	N'Tub', 12),
	(13, @IdLookup,	N'Unit',13 )

	INSERT INTO [dbo].[Lookups]
	([LookupValueType], [Name], [Description])
	VALUES
	(N'string', N'InventorySkuPurchaseUnitOfMeasures', N'Purchase Unit of Measures')

	SET @IdLookup = SCOPE_IDENTITY()

	INSERT INTO [dbo].[LookupVariants]
	([Id], [IdLookup], [ValueVariant], [Order])
	VALUES
	(1, @IdLookup,	N'Capsule' ,1),
	(2, @IdLookup,	N'Grams', 2),
	(3, @IdLookup,	N'Ounces',3 ),
	(4, @IdLookup,	N'Unit', 4),
	(5, @IdLookup,	N'Carton',5 )

	INSERT INTO [dbo].[InventorySkuOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'ProductSource', 3, (SELECT TOP 1 Id FROM [Lookups] WHERE Name='InventorySkuProductSources'), NULL, NULL),
	(N'Quantity', 3, NULL, NULL, NULL),
	(N'UnitOfMeasure', 3, (SELECT TOP 1 Id FROM [Lookups] WHERE Name='InventorySkuUnitOfMeasures'), NULL, NULL),
	(N'UnitOfMeasureAmount', 3, NULL, NULL, NULL),
	(N'PurchaseUnitOfMeasure', 3, (SELECT TOP 1 Id FROM [Lookups] WHERE Name='InventorySkuPurchaseUnitOfMeasures'), NULL, NULL),
	(N'PurchaseUnitOfMeasureAmount', 3, NULL, NULL, NULL)

	INSERT INTO ProductOptionTypes
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'InventorySkuChannel', 3, (SELECT TOP 1 Id FROM [Lookups] WHERE Name='InventorySkuChannels'), NULL, NULL),
	(N'Assemble', 5, NULL, NULL, 'False')

END
GO