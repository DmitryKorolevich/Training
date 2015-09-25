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
