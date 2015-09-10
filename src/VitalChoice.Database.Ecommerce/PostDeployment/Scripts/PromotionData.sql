IF NOT EXISTS(SELECT * FROM PromotionOptionTypes)
BEGIN

	INSERT INTO PromotionOptionTypes
	(DefaultValue, IdFieldType, IdObjectType, Name)
	SELECT 'False', 5, NULL, N'AllowHealthwise'

END

GO
