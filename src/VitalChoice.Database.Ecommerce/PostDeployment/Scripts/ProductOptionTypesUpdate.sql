GO

IF NOT EXISTS(SELECT * FROM ProductOptionTypes WHERE Name='SubProductGroupName')
BEGIN

INSERT INTO ProductOptionTypes
(DefaultValue, IdFieldType, IdProductType, Name)
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

IF EXISTS(SELECT * FROM ProductOptionTypes WHERE IdProductType=3 AND Name='Stock')
BEGIN

DELETE ProductOptionTypes
WHERE (IdProductType=3 OR IdProductType=4) AND
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