IF NOT EXISTS(SELECT * FROM ProductOptionTypes)
BEGIN
--non-perishable product type

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdObjectType, Name)
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
	(DefaultValue, IdFieldType, IdObjectType, Name, IdLookup)
	SELECT '1', 3, 1, 'Seller', @IdLookup

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdObjectType, Name, IdLookup)
	SELECT '1', 3, 2, 'Seller', @IdLookup

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdObjectType, Name, IdLookup)
	SELECT '1', 3, 3, 'Seller', @IdLookup

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdObjectType, Name, IdLookup)
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
	(DefaultValue, IdFieldType, IdObjectType, Name, IdLookup)
	SELECT NULL, 3, 1, 'SpecialIcon', @IdLookup

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdObjectType, Name, IdLookup)
	SELECT NULL, 3, 2, 'SpecialIcon', @IdLookup

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdObjectType, Name, IdLookup)
	SELECT NULL, 3, 3, 'SpecialIcon', @IdLookup

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdObjectType, Name, IdLookup)
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
	(DefaultValue, IdFieldType, IdObjectType, Name, IdLookup)
	VALUES
	(NULL, 3, 1, 'GoogleCategory', @IdLookup),
	(NULL, 3, 2, 'GoogleCategory', @IdLookup),
	(NULL, 3, 3, 'GoogleCategory', @IdLookup),
	(NULL, 3, 4, 'GoogleCategory', @IdLookup)

END

IF NOT EXISTS(SELECT * FROM ProductOptionTypes WHERE IdObjectType=2 AND Name='Description')
BEGIN

--perishable product type

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdObjectType, Name)
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
	SELECT N'0', 5, 2, N'AutoShipFrequency1'
	UNION
	SELECT N'0', 5, 2, N'AutoShipFrequency2'
	UNION
	SELECT N'0', 5, 2, N'AutoShipFrequency3'
	UNION
	SELECT N'0', 5, 2, N'AutoShipFrequency6'

	--EGC product type

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdObjectType, Name)
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
	(DefaultValue, IdFieldType, IdObjectType, Name)
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

IF NOT EXISTS (SELECT * FROM ProductOptionTypes WHERE Name='CrossSellImage1')
BEGIN

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdObjectType, Name)
	SELECT '/some1.png', 4, 1, N'CrossSellImage1'
	UNION
	SELECT '/some2.png', 4, 1, N'CrossSellImage2'
	UNION
	SELECT '/some3.png', 4, 1, N'CrossSellImage3'
	UNION
	SELECT '/some4.png', 4, 1, N'CrossSellImage4'
	UNION
	SELECT 'http://someurl.com/1', 4, 1, N'CrossSellUrl1'
	UNION
	SELECT 'http://someurl.com/1', 4, 1, N'CrossSellUrl2'
	UNION
	SELECT 'http://someurl.com/1', 4, 1, N'CrossSellUrl3'
	UNION
	SELECT 'http://someurl.com/1', 4, 1, N'CrossSellUrl4'
	UNION
	SELECT '/some1.png', 4, 1, N'YouTubeImage1'
	UNION	
	SELECT '/some2.png', 4, 1, N'YouTubeImage2'
	UNION
	SELECT '/some3.png', 4, 1, N'YouTubeImage3'
	UNION
	SELECT '/some1.png', 4, 1, N'YouTubeText1'
	UNION	
	SELECT '/some2.png', 4, 1, N'YouTubeText2'
	UNION
	SELECT '/some3.png', 4, 1, N'YouTubeText3'
	UNION
	SELECT 'jGwOsFo8TTg', 4, 1, N'YouTubeVideo1'
	UNION	
	SELECT 'btlfoO75kfI', 4, 1, N'YouTubeVideo2'
	UNION
	SELECT 'vCsRTamxWuw', 4, 1, N'YouTubeVideo3'
	UNION
	SELECT '/some1.png', 4, 2, N'CrossSellImage1'
	UNION	
	SELECT '/some2.png', 4, 2, N'CrossSellImage2'
	UNION
	SELECT '/some3.png', 4, 2, N'CrossSellImage3'
	UNION
	SELECT '/some4.png', 4, 2, N'CrossSellImage4'
	UNION
	SELECT 'http://someurl.com/1', 4, 2, N'CrossSellUrl1'
	UNION
	SELECT 'http://someurl.com/1', 4, 2, N'CrossSellUrl2'
	UNION
	SELECT 'http://someurl.com/1', 4, 2, N'CrossSellUrl3'
	UNION
	SELECT 'http://someurl.com/1', 4, 2, N'CrossSellUrl4'
	UNION
	SELECT '/some1.png', 4, 2, N'YouTubeImage1'
	UNION	
	SELECT '/some2.png', 4, 2, N'YouTubeImage2'
	UNION
	SELECT '/some3.png', 4, 2, N'YouTubeImage3'
	UNION
	SELECT '/some1.png', 4, 2, N'YouTubeText1'
	UNION	
	SELECT '/some2.png', 4, 2, N'YouTubeText2'
	UNION
	SELECT '/some3.png', 4, 2, N'YouTubeText3'
	UNION
	SELECT 'jGwOsFo8TTg', 4, 2, N'YouTubeVideo1'
	UNION	
	SELECT 'btlfoO75kfI', 4, 2, N'YouTubeVideo2'
	UNION
	SELECT 'vCsRTamxWuw', 4, 2, N'YouTubeVideo3'
	UNION
	SELECT '/some1.png', 4, 3, N'CrossSellImage1'
	UNION	
	SELECT '/some2.png', 4, 3, N'CrossSellImage2'
	UNION
	SELECT '/some3.png', 4, 3, N'CrossSellImage3'
	UNION
	SELECT '/some4.png', 4, 3, N'CrossSellImage4'
	UNION
	SELECT 'http://someurl.com/1', 4, 3, N'CrossSellUrl1'
	UNION
	SELECT 'http://someurl.com/1', 4, 3, N'CrossSellUrl2'
	UNION
	SELECT 'http://someurl.com/1', 4, 3, N'CrossSellUrl3'
	UNION
	SELECT 'http://someurl.com/1', 4, 3, N'CrossSellUrl4'
	UNION
	SELECT '/some1.png', 4, 3, N'YouTubeImage1'
	UNION	
	SELECT '/some2.png', 4, 3, N'YouTubeImage2'
	UNION
	SELECT '/some3.png', 4, 3, N'YouTubeImage3'
	UNION
	SELECT '/some1.png', 4, 3, N'YouTubeText1'
	UNION	
	SELECT '/some2.png', 4, 3, N'YouTubeText2'
	UNION
	SELECT '/some3.png', 4, 3, N'YouTubeText3'
	UNION
	SELECT 'jGwOsFo8TTg', 4, 3, N'YouTubeVideo1'
	UNION	
	SELECT 'btlfoO75kfI', 4, 3, N'YouTubeVideo2'
	UNION
	SELECT 'vCsRTamxWuw', 4, 3, N'YouTubeVideo3'
	UNION
	SELECT '/some1.png', 4, 4, N'CrossSellImage1'
	UNION	
	SELECT '/some2.png', 4, 4, N'CrossSellImage2'
	UNION
	SELECT '/some3.png', 4, 4, N'CrossSellImage3'
	UNION
	SELECT '/some4.png', 4, 4, N'CrossSellImage4'
	UNION
	SELECT 'http://someurl.com/1', 4, 4, N'CrossSellUrl1'
	UNION
	SELECT 'http://someurl.com/1', 4, 4, N'CrossSellUrl2'
	UNION
	SELECT 'http://someurl.com/1', 4, 4, N'CrossSellUrl3'
	UNION
	SELECT 'http://someurl.com/1', 4, 4, N'CrossSellUrl4'
	UNION
	SELECT '/some1.png', 4, 4, N'YouTubeImage1'
	UNION	
	SELECT '/some2.png', 4, 4, N'YouTubeImage2'
	UNION
	SELECT '/some3.png', 4, 4, N'YouTubeImage3'
	UNION
	SELECT '/some1.png', 4, 4, N'YouTubeText1'
	UNION	
	SELECT '/some2.png', 4, 4, N'YouTubeText2'
	UNION
	SELECT '/some3.png', 4, 4, N'YouTubeText3'
	UNION
	SELECT 'jGwOsFo8TTg', 4, 4, N'YouTubeVideo1'
	UNION	
	SELECT 'btlfoO75kfI', 4, 4, N'YouTubeVideo2'
	UNION
	SELECT 'vCsRTamxWuw', 4, 4, N'YouTubeVideo3'


END


IF EXISTS(SELECT * FROM ProductOptionTypes WHERE IdLookup IS NOT NULL AND IdFieldType=2)
BEGIN

  UPDATE ProductOptionTypes
  SET IdFieldType=3
  WHERE IdLookup IS NOT NULL AND IdFieldType=2

END

IF NOT EXISTS(SELECT * FROM ProductOptionTypes WHERE Name='SubProductGroupName')
BEGIN

INSERT INTO ProductOptionTypes
(DefaultValue, IdFieldType, IdObjectType, Name)
SELECT NULL, 4, 1, N'SubProductGroupName'
UNION
SELECT NULL, 4, 2, N'SubProductGroupName'
UNION
SELECT NULL, 4, 3, N'SubProductGroupName'
UNION
SELECT NULL, 4, 4, N'SubProductGroupName'

UPDATE ProductOptionTypes
SET Name='Recipes'
WHERE Name='Recepies'

END

GO

IF EXISTS(SELECT * FROM ProductOptionTypes WHERE IdObjectType=3 AND Name='Stock')
BEGIN

DELETE ProductOptionTypes
WHERE (IdObjectType=3 OR IdObjectType=4) AND
(Name = 'Stock' OR Name='DisregardStock' OR Name='DisallowSingle' OR Name='OrphanType' OR Name='AutoShipProduct' OR
Name='AutoShipFrequency1' OR Name='AutoShipFrequency2' OR Name='AutoShipFrequency3' OR Name='AutoShipFrequency6' OR Name='OffPercent')

END

IF EXISTS(SELECT * FROM ProductOptionTypes WHERE DefaultValue='2' AND Name='DisregardStock')
BEGIN

UPDATE ProductOptionTypes
SET DefaultValue='1'
WHERE Name='DisregardStock'

END

IF EXISTS(SELECT * FROM ProductOptionTypes WHERE DefaultValue='1' AND Name='DisregardStock')
BEGIN

UPDATE ProductOptionTypes
SET DefaultValue='True'
WHERE IdFieldType=5 AND DefaultValue='1'

UPDATE ProductOptionTypes
SET DefaultValue='False'
WHERE IdFieldType=5 AND DefaultValue='0'

END

GO

IF NOT EXISTS(SELECT * FROM ProductOptionTypes WHERE Name='GoogleFeedTitle')
BEGIN

INSERT INTO ProductOptionTypes
(DefaultValue, IdFieldType, IdObjectType, Name)
SELECT NULL, 4, 1, N'GoogleFeedTitle'
UNION
SELECT NULL, 4, 2, N'GoogleFeedTitle'
UNION
SELECT NULL, 4, 3, N'GoogleFeedTitle'
UNION
SELECT NULL, 4, 4, N'GoogleFeedTitle'

INSERT INTO ProductOptionTypes
(DefaultValue, IdFieldType, IdObjectType, Name)
SELECT NULL, 4, 1, N'GoogleFeedDescription'
UNION
SELECT NULL, 4, 2, N'GoogleFeedDescription'
UNION
SELECT NULL, 4, 3, N'GoogleFeedDescription'
UNION
SELECT NULL, 4, 4, N'GoogleFeedDescription'

END
GO
IF (SELECT TOP 1 IdFieldType FROM ProductOptionTypes WHERE Name = N'AdditionalNotes') <> 8
BEGIN

	UPDATE ProductOptionTypes
	SET IdFieldType = 8
	WHERE Name = N'Description'

	UPDATE ProductOptionTypes
	SET IdFieldType = 8
	WHERE Name = N'ShortDescription'

	UPDATE ProductOptionTypes
	SET IdFieldType = 8
	WHERE Name = N'ProductNotes'

	UPDATE ProductOptionTypes
	SET IdFieldType = 8
	WHERE Name = N'Serving'

	UPDATE ProductOptionTypes
	SET IdFieldType = 8
	WHERE Name = N'Recipes'

	UPDATE ProductOptionTypes
	SET IdFieldType = 8
	WHERE Name = N'Ingredients'

	UPDATE ProductOptionTypes
	SET IdFieldType = 8
	WHERE Name = N'AdditionalNotes'
END

IF NOT EXISTS(SELECT * FROM ProductOptionTypes WHERE Name='InventoryCategoryId')
BEGIN

INSERT INTO ProductOptionTypes
(DefaultValue, IdFieldType, IdObjectType, Name)
SELECT NULL, 3, 1, N'InventoryCategoryId'
UNION
SELECT NULL, 3, 2, N'InventoryCategoryId'
UNION
SELECT NULL, 3, 3, N'InventoryCategoryId'
UNION
SELECT NULL, 3, 4, N'InventoryCategoryId'

END

GO

IF NOT EXISTS(SELECT * FROM ProductOptionTypes WHERE Name='QTY')
BEGIN

INSERT INTO ProductOptionTypes
(DefaultValue, IdFieldType, IdObjectType, Name)
SELECT NULL, 4, 1, N'QTY'
UNION
SELECT NULL, 4, 2, N'QTY'
UNION
SELECT NULL, 4, 3, N'QTY'
UNION
SELECT NULL, 4, 4, N'QTY'

END

GO

IF NOT EXISTS (SELECT * FROM ProductOptionTypes WHERE Name='QTYThreshold')
BEGIN

INSERT INTO ProductOptionTypes
(DefaultValue, IdFieldType, IdObjectType, Name)
SELECT 4, 3, 1, N'QTYThreshold'

END

GO

IF NOT EXISTS(SELECT * FROM ProductOptionTypes WHERE Name='SalesText')
BEGIN

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdObjectType, Name)
	SELECT NULL, 4, NULL, N'SalesText'
	UNION
	SELECT NULL, 4, 1, N'DescriptionTitleOverride'
	UNION
	SELECT 'False', 5, 1, N'DescriptionHide'
	UNION
	SELECT NULL, 4, 2, N'DescriptionTitleOverride'
	UNION
	SELECT 'False', 5, 2, N'DescriptionHide'
	UNION
	SELECT NULL, 4, 3, N'DescriptionTitleOverride'
	UNION
	SELECT 'False', 5, 3, N'DescriptionHide'
	UNION
	SELECT NULL, 4, 4, N'DescriptionTitleOverride'
	UNION
	SELECT 'False', 5, 4, N'DescriptionHide'
	UNION
	SELECT NULL, 8, 1, N'Shipping'
	UNION
	SELECT NULL, 4, 1, N'ShippingTitleOverride'
	UNION
	SELECT 'False', 5, 1, N'ShippingHide'
	UNION
	SELECT NULL, 8, 2, N'Shipping'
	UNION
	SELECT NULL, 4, 2, N'ShippingTitleOverride'
	UNION
	SELECT 'False', 5, 2, N'ShippingHide'
	UNION
	SELECT NULL, 4, 1, N'ServingTitleOverride'
	UNION
	SELECT 'False', 5, 1, N'ServingHide'
	UNION
	SELECT NULL, 4, 2, N'ServingTitleOverride'
	UNION
	SELECT 'False', 5, 2, N'ServingHide'
	UNION
	SELECT NULL, 4, 1, N'RecipesTitleOverride'
	UNION
	SELECT 'False', 5, 1, N'RecipesHide'
	UNION
	SELECT NULL, 4, 2, N'RecipesTitleOverride'
	UNION
	SELECT 'False', 5, 2, N'RecipesHide'
	UNION
	SELECT NULL, 4, 1, N'IngredientsTitleOverride'
	UNION
	SELECT 'False', 5, 1, N'IngredientsHide'
	UNION
	SELECT NULL, 4, 2, N'IngredientsTitleOverride'
	UNION
	SELECT 'False', 5, 2, N'IngredientsHide'

END

GO

UPDATE [dbo].[ProductOptionTypes]
SET [IdFieldType] = 3
WHERE [Name] = 'QTY'

GO
