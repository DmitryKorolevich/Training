IF NOT EXISTS(SELECT * FROM DiscountOptionTypes)
BEGIN

	INSERT INTO DiscountOptionTypes
	(DefaultValue, IdFieldType, IdObjectType, Name)
	SELECT 'False', 5, 1, N'AllowHealthwise'
	UNION
	SELECT 'False', 5, 1, N'RequireMinimumPerishable'
	UNION
	SELECT NULL, 1, 1, N'RequireMinimumPerishableAmount'
	UNION
	SELECT 'False', 5, 1, N'FreeShipping'
	UNION
	SELECT 'False', 5, 2, N'AllowHealthwise'
	UNION
	SELECT 'False', 5, 2, N'RequireMinimumPerishable'
	UNION
	SELECT NULL, 1, 2, N'RequireMinimumPerishableAmount'
	UNION
	SELECT 'False', 5, 2, N'FreeShipping'
	UNION
	SELECT 'False', 5, 4, N'AllowHealthwise'
	UNION
	SELECT 'False', 5, 4, N'RequireMinimumPerishable'
	UNION
	SELECT NULL, 1, 4, N'RequireMinimumPerishableAmount'
	UNION
	SELECT 'False', 5, 4, N'FreeShipping'
	UNION
	SELECT 'False', 5, 5, N'AllowHealthwise'
	UNION
	SELECT 'False', 5, 5, N'RequireMinimumPerishable'
	UNION
	SELECT NULL, 1, 5, N'RequireMinimumPerishableAmount'
	UNION
	SELECT 'False', 5, 5, N'FreeShipping'
	UNION

	SELECT NULL, 1, 1, N'Amount'
	UNION
	SELECT NULL, 1, 2, N'Percent'
	UNION
	SELECT NULL, 1, 4, N'Amount'
	UNION
	SELECT NULL, 4, 4, N'ProductSKU'

END

GO

IF NOT EXISTS(SELECT * FROM DiscountOptionTypes WHERE IdObjectType=4 AND Name='Threshold')
BEGIN

UPDATE DiscountOptionTypes
SET Name='Threshold'
WHERE IdObjectType=4 AND Name='Amount'

END

GO

IF NOT EXISTS (SELECT * FROM DiscountOptionTypes WHERE Name='MaxTimesUse')
BEGIN

INSERT INTO DiscountOptionTypes
(DefaultValue, IdFieldType, IdObjectType, Name)
SELECT NULL, 3, NULL, N'MaxTimesUse'

END

GO

IF EXISTS (SELECT * FROM DiscountOptionTypes WHERE Name='OneTimeOnly')
BEGIN

DELETE DiscountOptionValues
WHERE IdOptionType IN
(SELECT Id FROM DiscountOptionTypes WHERE Name='OneTimeOnly')

DELETE DiscountOptionTypes WHERE Name='OneTimeOnly'

END

GO