IF OBJECT_ID(N'[dbo].[ProductOptionTypes]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[ProductOptionTypes]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[Name] NVARCHAR(50) NOT NULL, 
		[IdFieldType] INT NOT NULL, 
		[IdLookup] INT NULL, 
		[IdProductType] INT NULL, 
		[DefaultValue] NVARCHAR(250) NULL, 
		CONSTRAINT [FK_ProductOptionTypes_ToLookup] FOREIGN KEY ([IdLookup]) REFERENCES [Lookups]([Id]), 
		CONSTRAINT [FK_ProductOptionTypes_ToFieldType] FOREIGN KEY ([IdFieldType]) REFERENCES [FieldTypes]([Id]), 
		CONSTRAINT [FK_ProductOptionTypes_ToProductType] FOREIGN KEY ([IdProductType]) REFERENCES [ProductTypes]([Id])
	);

	CREATE INDEX [IX_ProductOptionTypes_Name] ON [dbo].[ProductOptionTypes] ([Name]) INCLUDE (Id, IdFieldType, IdProductType)
END

IF NOT EXISTS(SELECT * FROM ProductOptionTypes)
BEGIN
--non-perishable product type

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdProductType, Name)
	SELECT NULL, 4, 1, N'Description'
	UNION
	SELECT NULL, 4, 1, N'Serving'
	UNION
	SELECT NULL, 4, 1, N'Recipes'
	UNION
	SELECT NULL, 4, 1, N'Ingredients'
	UNION
	SELECT NULL, 4, 1, N'ShortDescription'
	UNION
	SELECT NULL, 4, 1, N'ProductNotes'
	UNION
	SELECT NULL, 4, 1, N'MetaTitle'
	UNION
	SELECT NULL, 4, 1, N'MetaDescription'
	UNION
	SELECT NULL, 4, 1, N'TaxCode'
	UNION
	SELECT NULL, 4, 1, N'Thumbnail'
	UNION
	SELECT NULL, 4, 1, N'NutritionalTitle'
	UNION
	SELECT NULL, 4, 1, N'MainProductImage'
	UNION
	SELECT NULL, 4, 1, N'NutritionalTitle'
	UNION
	SELECT N'0', 3, 1, N'Stock'
	UNION
	SELECT N'1', 5, 1, N'DisregardStock'
	UNION
	SELECT N'0', 5, 1, N'DisallowSingle'
	UNION
	SELECT N'0', 5, 1, N'NonDiscountable'
	UNION
	SELECT N'0', 5, 1, N'OrphanType'
	UNION
	SELECT N'0', 5, 1, N'AutoShipProduct'
	UNION
	SELECT N'0', 2, 1, N'OffPercent'
	UNION
	SELECT N'0', 5, 1, N'HideFromDataFeed'
	UNION
	SELECT N'0', 5, 1, N'AutoShipFrequency1'
	UNION
	SELECT N'0', 5, 1, N'AutoShipFrequency2'
	UNION
	SELECT N'0', 5, 1, N'AutoShipFrequency3'
	UNION
	SELECT N'0', 5, 1, N'AutoShipFrequency6'
	
	DECLARE @IdLookup INT
	
	INSERT INTO Lookups
	(LookupValueType)
	VALUES
	(N'string')

	SET @IdLookup = SCOPE_IDENTITY()

	INSERT INTO LookupVariants
	(Id, IdLookup, ValueVariant)
	SELECT 1, @IdLookup, 'ModSeller'
	UNION
	SELECT 2, @IdLookup, 'LowSeller'
	UNION
	SELECT 3, @IdLookup, 'BestSeller'

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdProductType, Name, IdLookup)
	SELECT '1', 3, 1, 'Seller', @IdLookup

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdProductType, Name, IdLookup)
	SELECT '1', 3, 2, 'Seller', @IdLookup

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdProductType, Name, IdLookup)
	SELECT '1', 3, 3, 'Seller', @IdLookup

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdProductType, Name, IdLookup)
	SELECT '1', 3, 4, 'Seller', @IdLookup


	INSERT INTO Lookups
	(LookupValueType)
	VALUES
	(N'string')

	SET @IdLookup = SCOPE_IDENTITY()

	INSERT INTO LookupVariants
	(Id, IdLookup, ValueVariant)
	SELECT 1, @IdLookup, 'MSC Icon'
	UNION
	SELECT 2, @IdLookup, 'USDA Icon'
	UNION
	SELECT 3, @IdLookup, 'ASMI Seal'
	UNION
	SELECT 4, @IdLookup, 'USDA + Fair Trade'
	UNION
	SELECT 5, @IdLookup, 'Certified Humane'
	UNION
	SELECT 6, @IdLookup, 'ASMI-W'

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdProductType, Name, IdLookup)
	SELECT NULL, 3, 1, 'SpecialIcon', @IdLookup

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdProductType, Name, IdLookup)
	SELECT NULL, 3, 2, 'SpecialIcon', @IdLookup

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdProductType, Name, IdLookup)
	SELECT NULL, 3, 3, 'SpecialIcon', @IdLookup

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdProductType, Name, IdLookup)
	SELECT NULL, 3, 4, 'SpecialIcon', @IdLookup


	INSERT INTO Lookups
	(LookupValueType)
	VALUES
	(N'string')

	SET @IdLookup = SCOPE_IDENTITY()

	INSERT INTO LookupVariants
	(Id, IdLookup, ValueVariant)
	SELECT 1, @IdLookup, 'Food, Beverages & Tobacco > Beverages > Tea & Infusions > Tea Bags & Loose Tea Leaves'
	UNION
	SELECT 2, @IdLookup, 'Food, Beverages & Tobacco > Food Items'
	UNION
	SELECT 3, @IdLookup, 'Food, Beverages & Tobacco > Food Items > Candy & Gum > Candy & Chocolate > Chocolate Bars'
	UNION
	SELECT 4, @IdLookup, 'Food, Beverages & Tobacco > Food Items > Condiments & Sauces > Pickles & Relishes'
	UNION
	SELECT 5, @IdLookup, 'Food, Beverages & Tobacco > Food Items > Cooking & Baking Ingredients'
	UNION
	SELECT 6, @IdLookup, 'Food, Beverages & Tobacco > Food Items > Cooking & Baking Ingredients > Cooking Oil'
	UNION
	SELECT 7, @IdLookup, 'Food, Beverages & Tobacco > Food Items > Cooking & Baking Ingredients > Cooking Oil > Olive Oil'
	UNION
	SELECT 8, @IdLookup, 'Food, Beverages & Tobacco > Food Items > Food Gift Baskets'
	UNION
	SELECT 9, @IdLookup, 'Food, Beverages & Tobacco > Food Items > Fruits & Vegetables > Dried Fruits'
	UNION
	SELECT 10, @IdLookup, 'Food, Beverages & Tobacco > Food Items > Fruits & Vegetables > Fresh & Frozen Fruits > Berries'
	UNION
	SELECT 11, @IdLookup, 'Food, Beverages & Tobacco > Food Items > Fruits & Vegetables > Fresh & Frozen Vegetables'
	UNION
	SELECT 12, @IdLookup, 'Food, Beverages & Tobacco > Food Items > Fruits & Vegetables > Fresh & Frozen Vegetables > Beets'
	UNION
	SELECT 13, @IdLookup, 'Food, Beverages & Tobacco > Food Items > Fruits & Vegetables > Fresh & Frozen Vegetables > Cabbage'
	UNION
	SELECT 14, @IdLookup, 'Food, Beverages & Tobacco > Food Items > Fruits & Vegetables > Fresh & Frozen Vegetables > Carrots'
	UNION
	SELECT 15, @IdLookup, 'Food, Beverages & Tobacco > Food Items > Fruits & Vegetables > Fresh & Frozen Vegetables > Radishes'
	UNION
	SELECT 16, @IdLookup, 'Food, Beverages & Tobacco > Food Items > Meat, Seafood & Eggs > Meat > Fresh & Frozen Meats'
	UNION
	SELECT 17, @IdLookup, 'Food, Beverages & Tobacco > Food Items > Meat, Seafood & Eggs > Meat > Meat Patties'
	UNION
	SELECT 18, @IdLookup, 'Food, Beverages & Tobacco > Food Items > Meat, Seafood & Eggs > Seafood'
	UNION
	SELECT 19, @IdLookup, 'Food, Beverages & Tobacco > Food Items > Meat, Seafood & Eggs > Seafood > Canned Seafood'
	UNION
	SELECT 20, @IdLookup, 'Food, Beverages & Tobacco > Food Items > Nuts & Seeds > Almonds'
	UNION
	SELECT 21, @IdLookup, 'Food, Beverages & Tobacco > Food Items > Nuts & Seeds > Cashews'
	UNION
	SELECT 22, @IdLookup, 'Food, Beverages & Tobacco > Food Items > Nuts & Seeds > Hazelnuts'
	UNION
	SELECT 23, @IdLookup, 'Food, Beverages & Tobacco > Food Items > Nuts & Seeds > Mixed Nuts'
	UNION
	SELECT 24, @IdLookup, 'Food, Beverages & Tobacco > Food Items > Nuts & Seeds > Walnuts'
	UNION
	SELECT 25, @IdLookup, 'Food, Beverages & Tobacco > Food Items > Prepared Foods > Sushi'
	UNION
	SELECT 26, @IdLookup, 'Food, Beverages & Tobacco > Food Items > Seasonings & Spices > Herbs & Spices'
	UNION
	SELECT 27, @IdLookup, 'Food, Beverages & Tobacco > Food Items > Snack Foods > Jerky'
	UNION
	SELECT 28, @IdLookup, 'Food, Beverages & Tobacco > Food Items > Soups & Broths'
	UNION
	SELECT 29, @IdLookup, 'Health & Beauty > Health Care'
	UNION
	SELECT 30, @IdLookup, 'Health & Beauty > Health Care > Fitness & Nutrition > Vitamins & Supplements'
	UNION
	SELECT 31, @IdLookup, 'Home & Garden > Kitchen & Dining > Food & Beverage Carriers > Water Bottles'
	UNION
	SELECT 32, @IdLookup, 'Home & Garden > Kitchen & Dining > Food Storage > Bowl Covers'
	UNION
	SELECT 33, @IdLookup, 'Home & Garden > Kitchen & Dining > Kitchen Tools & Utensils > Aprons'
	UNION
	SELECT 34, @IdLookup, 'Home & Garden > Kitchen & Dining > Kitchen Tools & Utensils > Can Openers'
	UNION
	SELECT 35, @IdLookup, 'Home & Garden > Kitchen & Dining > Kitchen Tools & Utensils > Tea Strainers'
	UNION
	SELECT 36, @IdLookup, 'Media > Books > Non-Fiction > Cooking Books'
	UNION
	SELECT 37, @IdLookup, 'Media > Books > Non-Fiction > Health & Fitness Books'
	UNION
	SELECT 38, @IdLookup, 'Animals & Pet Supplies > Pet Supplies > Dog Supplies > Dog Treats'

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdProductType, Name, IdLookup)
	SELECT NULL, 3, 1, 'GoogleCategory', @IdLookup

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdProductType, Name, IdLookup)
	SELECT NULL, 3, 2, 'GoogleCategory', @IdLookup

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdProductType, Name, IdLookup)
	SELECT NULL, 3, 3, 'GoogleCategory', @IdLookup

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdProductType, Name, IdLookup)
	SELECT NULL, 3, 4, 'GoogleCategory', @IdLookup

END

IF EXISTS(SELECT * FROM ProductOptionTypes WHERE IdLookup IS NOT NULL AND IdFieldType=2)
BEGIN

  UPDATE ProductOptionTypes
  SET IdFieldType=3
  WHERE IdLookup IS NOT NULL AND IdFieldType=2

END

IF NOT EXISTS(SELECT * FROM ProductOptionTypes WHERE IdProductType=2 AND Name='Description')
BEGIN

--perishable product type

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdProductType, Name)
	SELECT NULL, 4, 2, N'Description'
	UNION
	SELECT NULL, 4, 2, N'Serving'
	UNION
	SELECT NULL, 4, 2, N'Recipes'
	UNION
	SELECT NULL, 4, 2, N'Ingredients'
	UNION
	SELECT NULL, 4, 2, N'ShortDescription'
	UNION
	SELECT NULL, 4, 2, N'ProductNotes'
	UNION
	SELECT NULL, 4, 2, N'MetaTitle'
	UNION
	SELECT NULL, 4, 2, N'MetaDescription'
	UNION
	SELECT NULL, 4, 2, N'TaxCode'
	UNION
	SELECT NULL, 4, 2, N'Thumbnail'
	UNION
	SELECT NULL, 4, 2, N'MainProductImage'
	UNION
	SELECT NULL, 4, 2, N'NutritionalTitle'
	UNION
	SELECT N'0', 3, 2, N'Stock'
	UNION
	SELECT N'2', 5, 2, N'DisregardStock'
	UNION
	SELECT N'0', 5, 2, N'DisallowSingle'
	UNION
	SELECT N'0', 5, 2, N'NonDiscountable'
	UNION
	SELECT N'0', 5, 2, N'AutoShipProduct'
	UNION
	SELECT N'0', 2, 2, N'OffPercent'
	UNION
	SELECT N'0', 5, 2, N'HideFromDataFeed'
	UNION
	SELECT N'0', 5, 2, N'AutoShipFrequency2'
	UNION
	SELECT N'0', 5, 2, N'AutoShipFrequency2'
	UNION
	SELECT N'0', 5, 2, N'AutoShipFrequency3'
	UNION
	SELECT N'0', 5, 2, N'AutoShipFrequency6'

	--EGC product type

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdProductType, Name)
	SELECT NULL, 4, 3, N'Description'
	UNION
	SELECT NULL, 4, 3, N'ShortDescription'
	UNION
	SELECT NULL, 4, 3, N'ProductNotes'
	UNION
	SELECT NULL, 4, 3, N'MetaTitle'
	UNION
	SELECT NULL, 4, 3, N'MetaDescription'
	UNION
	SELECT NULL, 4, 3, N'TaxCode'
	UNION
	SELECT NULL, 4, 3, N'Thumbnail'
	UNION
	SELECT NULL, 4, 3, N'MainProductImage'
	UNION
	SELECT NULL, 4, 3, N'NutritionalTitle'
	UNION
	SELECT N'0', 3, 3, N'Stock'
	UNION
	SELECT N'3', 5, 3, N'DisregardStock'
	UNION
	SELECT N'0', 5, 3, N'DisallowSingle'
	UNION
	SELECT N'0', 5, 3, N'NonDiscountable'
	UNION
	SELECT N'0', 5, 3, N'OrphanType'
	UNION
	SELECT N'0', 5, 3, N'AutoShipProduct'
	UNION
	SELECT N'0', 2, 3, N'OffPercent'
	UNION
	SELECT N'0', 5, 3, N'HideFromDataFeed'
	UNION
	SELECT N'0', 5, 3, N'AutoShipFrequency1'
	UNION
	SELECT N'0', 5, 3, N'AutoShipFrequency2'
	UNION
	SELECT N'0', 5, 3, N'AutoShipFrequency3'
	UNION
	SELECT N'0', 5, 3, N'AutoShipFrequency6'

	--GC product type

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdProductType, Name)
	SELECT NULL, 4, 4, N'Description'
	UNION
	SELECT NULL, 4, 4, N'ShortDescription'
	UNION
	SELECT NULL, 4, 4, N'ProductNotes'
	UNION
	SELECT NULL, 4, 4, N'MetaTitle'
	UNION
	SELECT NULL, 4, 4, N'MetaDescription'
	UNION
	SELECT NULL, 4, 4, N'TaxCode'
	UNION
	SELECT NULL, 4, 4, N'Thumbnail'
	UNION
	SELECT NULL, 4, 4, N'MainProductImage'
	UNION
	SELECT NULL, 4, 4, N'NutritionalTitle'
	UNION
	SELECT N'0', 3, 4, N'Stock'
	UNION
	SELECT N'5', 5, 4, N'DisregardStock'
	UNION
	SELECT N'0', 5, 4, N'DisallowSingle'
	UNION
	SELECT N'0', 5, 4, N'NonDiscountable'
	UNION
	SELECT N'0', 5, 4, N'OrphanType'
	UNION
	SELECT N'0', 5, 4, N'AutoShipProduct'
	UNION
	SELECT N'0', 2, 4, N'OffPercent'
	UNION
	SELECT N'0', 5, 4, N'HideFromDataFeed'
	UNION
	SELECT N'0', 5, 4, N'AutoShipFrequency1'
	UNION
	SELECT N'0', 5, 4, N'AutoShipFrequency2'
	UNION
	SELECT N'0', 5, 4, N'AutoShipFrequency3'
	UNION
	SELECT N'0', 5, 4, N'AutoShipFrequency6'

END