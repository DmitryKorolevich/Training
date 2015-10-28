IF NOT EXISTS(SELECT * FROM PromotionOptionTypes)
BEGIN

	INSERT INTO PromotionOptionTypes
	(DefaultValue, IdFieldType, IdObjectType, Name)
	SELECT 'False', 5, NULL, N'AllowHealthwise'

END

GO

IF NOT EXISTS(SELECT * FROM [PromotionTypes] WHERE Id=2)
BEGIN
	INSERT INTO PromotionTypes
	(Id, Name)
	SELECT 2, 'Category Discount'

	INSERT INTO PromotionOptionTypes
	(DefaultValue, IdFieldType, IdObjectType, Name)
	SELECT NULL, 3, 1, N'MaxTimesUse'

	INSERT INTO PromotionOptionTypes
	(DefaultValue, IdFieldType, IdObjectType, Name)
	SELECT NULL, 1, 2, N'Percent'
END

GO

IF NOT EXISTS(SELECT * FROM [Lookups] WHERE Name='PromotionBuyTypes')
BEGIN

	DECLARE @IdLookupPromotionBuyType INT

	INSERT INTO [dbo].[Lookups]
	([LookupValueType], Name)
	VALUES
	(N'string', N'PromotionBuyTypes')

	SET @IdLookupPromotionBuyType = SCOPE_IDENTITY()

	INSERT INTO [dbo].[LookupVariants]
	([Id], [IdLookup], [ValueVariant])
	VALUES
	(1, @IdLookupPromotionBuyType, 'Any'),
	(2, @IdLookupPromotionBuyType, 'All')

	INSERT INTO PromotionOptionTypes
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	SELECT 'IdPromotionBuyType', 3, @IdLookupPromotionBuyType, 1, N'1'
END

IF EXISTS(SELECT * FROM PromotionOptionTypes WHERE Name='IdPromotionBuyType')
BEGIN
	UPDATE PromotionOptionTypes
	SET Name = 'PromotionBuyType'
	WHERE Name = 'IdPromotionBuyType'
END

GO
