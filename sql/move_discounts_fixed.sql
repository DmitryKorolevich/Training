USE [vitalchoice2.0]
GO

IF EXISTS(SELECT * FROM sys.procedures WHERE name = N'MoveDiscountField')
	DROP PROCEDURE dbo.MoveDiscountField
GO

CREATE PROCEDURE dbo.MoveDiscountField
(@destFieldName NVARCHAR(250), @sourceFieldName NVARCHAR(250), @fieldOperation NVARCHAR(MAX) = NULL, @sourceConditions NVARCHAR(MAX) = NULL)
AS
BEGIN
BEGIN TRY
	DECLARE @sql NVARCHAR(MAX)
	
	IF @fieldOperation IS NULL

		SET @sql = N'INSERT INTO [VitalChoice.Ecommerce].dbo.DiscountOptionValues
		(IdOptionType, IdDiscount, Value)
		SELECT t.Id, p.Id, a.'+@sourceFieldName+' FROM [VitalChoice.Ecommerce].dbo.Discounts AS p
		INNER JOIN [vitalchoice2.0].[dbo].discounts AS a ON a.idDiscount = p.Id
		INNER JOIN [VitalChoice.Ecommerce].dbo.DiscountOptionTypes AS t ON (t.IdObjectType = p.IdObjectType OR t.IdObjectType IS NULL) AND t.Name = N'''+@destFieldName+'''
		WHERE a.'+@sourceFieldName+' IS NOT NULL AND ('+ISNULL(@sourceConditions, '1=1')+')';

	ELSE
		
		SET @sql = N'INSERT INTO [VitalChoice.Ecommerce].dbo.DiscountOptionValues
		(IdOptionType, IdDiscount, Value)
		SELECT t.Id, p.Id, '+@fieldOperation+' FROM [VitalChoice.Ecommerce].dbo.Discounts AS p
		INNER JOIN [vitalchoice2.0].[dbo].discounts AS a ON a.idDiscount = p.Id
		INNER JOIN [VitalChoice.Ecommerce].dbo.DiscountOptionTypes AS t ON (t.IdObjectType = p.IdObjectType OR t.IdObjectType IS NULL) AND t.Name = N'''+@destFieldName+'''
		WHERE a.'+@sourceFieldName+' IS NOT NULL AND ('+ISNULL(@sourceConditions, '1=1')+')';

	EXEC(@sql)
END TRY
BEGIN CATCH
	SELECT 
		ERROR_MESSAGE() AS [Message],
		@destFieldName AS dest,
		@sourceFieldName AS source
END CATCH
END

GO

USE [VitalChoice.Ecommerce]
GO


--=========== Wipe everything ==============

DELETE FROM DiscountOptionValues
DELETE FROM DiscountsToCategories
DELETE FROM DiscountsToSelectedSkus
DELETE FROM DiscountsToSkus
DELETE FROM DiscountTiers
DELETE FROM DiscountToSelectedCategories
DELETE FROM OneTimeDiscountToCustomerUsages
UPDATE Orders
SET IdDiscount = NULL
UPDATE Carts
SET DiscountCode = NULL
DELETE FROM Discounts

DECLARE @toDelete TABLE(Id BIGINT NULL)

DELETE FROM [VitalChoice.Ecommerce].[dbo].[ObjectHistoryLogItems]
OUTPUT deleted.IdObjectHistoryLogDataItem INTO @toDelete
WHERE IdObjectType = 2 /*Order*/ AND (IdObject NOT IN (SELECT Id FROM [VitalChoice.Ecommerce].dbo.Orders) OR DateCreated < '2016-05-13')

DELETE FROM [VitalChoice.Ecommerce].[dbo].[ObjectHistoryLogItems]
OUTPUT deleted.IdObjectHistoryLogDataItem INTO @toDelete
WHERE IdObjectType = 3 /*Product*/ AND (IdObject NOT IN (SELECT Id FROM [VitalChoice.Ecommerce].dbo.Products) OR DateCreated < '2016-05-13')

DELETE FROM [VitalChoice.Ecommerce].[dbo].[ObjectHistoryLogItems]
OUTPUT deleted.IdObjectHistoryLogDataItem INTO @toDelete
WHERE IdObjectType = 4 /*Discount*/

DELETE FROM [VitalChoice.Ecommerce].[dbo].[ObjectHistoryLogItems]
OUTPUT deleted.IdObjectHistoryLogDataItem INTO @toDelete
WHERE IdObjectType = 8 /*Article*/ AND IdObject NOT IN (SELECT Id FROM [VitalChoice.Infrastructure].dbo.Articles)

DELETE FROM [VitalChoice.Ecommerce].[dbo].[ObjectHistoryLogItems]
OUTPUT deleted.IdObjectHistoryLogDataItem INTO @toDelete
WHERE IdObjectType = 10 /*Recipe*/ AND IdObject NOT IN (SELECT Id FROM [VitalChoice.Infrastructure].dbo.Recipes)

DELETE FROM [VitalChoice.Ecommerce].[dbo].[ObjectHistoryLogItems]
OUTPUT deleted.IdObjectHistoryLogDataItem INTO @toDelete
WHERE IdObjectType = 11 /*Content Page*/ AND IdObject NOT IN (SELECT Id FROM [VitalChoice.Infrastructure].dbo.ContentPages)

DELETE FROM [VitalChoice.Ecommerce].[dbo].[ObjectHistoryLogItems]
OUTPUT deleted.IdObjectHistoryLogDataItem INTO @toDelete
WHERE IdObjectType = 14 /*Product Category*/ AND IdObject NOT IN (SELECT Id FROM [VitalChoice.Infrastructure].dbo.ProductCategories)

DELETE FROM [VitalChoice.Ecommerce].[dbo].[ObjectHistoryLogItems]
OUTPUT deleted.IdObjectHistoryLogDataItem INTO @toDelete
WHERE IdObjectType = 12 /*Content Category*/ AND IdObject NOT IN (SELECT Id FROM [VitalChoice.Infrastructure].dbo.ContentCategories)

DELETE FROM [VitalChoice.Ecommerce].dbo.ObjectHistoryLogDataItems
WHERE IdObjectHistoryLogDataItem IN (SELECT Id FROM @toDelete)

GO
--=========== Import ==============

USE [vitalchoice2.0]

SET IDENTITY_INSERT [VitalChoice.Ecommerce].dbo.Discounts ON;
	
--================== Price =====================

INSERT INTO [VitalChoice.Ecommerce].dbo.Discounts
(Id, DateCreated, DateEdited, IdObjectType, StatusCode, Code, Description, ExcludeCategories, ExcludeSkus, ExpirationDate, StartDate, Assigned)
SELECT 
a.iddiscount, GETDATE(), GETDATE(), 1, CASE WHEN ISNULL(a.active, 0) <> 0 THEN 2/*Active*/ ELSE 1 /*Not Active*/ END, discountcode, a.discountdesc, a.pcDisc_IncExcCat, a.pcDisc_IncExcPrd, ISNULL(a.expDate, DATEADD(year, 1, GETDATE())), ISNULL(a.pcDisc_StartDate, '2010-01-01'), CASE WHEN ISNULL(a.pcWholesaleFlag, 0) <> 0 THEN 2 ELSE CASE WHEN ISNULL(a.pcRetailFlag, 0) <> 0 THEN 1 ELSE NULL END END
FROM [vitalchoice2.0].[dbo].discounts AS a
WHERE ISNULL(a.pricetodiscount, 0) > 0

--================== Percent =========================

INSERT INTO [VitalChoice.Ecommerce].dbo.Discounts
(Id, DateCreated, DateEdited, IdObjectType, StatusCode, Code, Description, ExcludeCategories, ExcludeSkus, ExpirationDate, StartDate, Assigned)
SELECT 
a.iddiscount, GETDATE(), GETDATE(), 2, CASE WHEN ISNULL(a.active, 0) <> 0 THEN 2/*Active*/ ELSE 1 /*Not Active*/ END, discountcode, a.discountdesc, a.pcDisc_IncExcCat, a.pcDisc_IncExcPrd, ISNULL(a.expDate, DATEADD(year, 1, GETDATE())), ISNULL(a.pcDisc_StartDate, '2010-01-01'), CASE WHEN ISNULL(a.pcWholesaleFlag, 0) <> 0 THEN 2 ELSE CASE WHEN ISNULL(a.pcRetailFlag, 0) <> 0 THEN 1 ELSE NULL END END
FROM [vitalchoice2.0].[dbo].discounts AS a
WHERE ISNULL(a.percentagetodiscount, 0) > 0

INSERT INTO [VitalChoice.Ecommerce].dbo.Discounts
(Id, DateCreated, DateEdited, IdObjectType, StatusCode, Code, Description, ExcludeCategories, ExcludeSkus, ExpirationDate, StartDate, Assigned)
SELECT 
a.iddiscount, GETDATE(), GETDATE(), 2, CASE WHEN ISNULL(a.active, 0) <> 0 THEN 2/*Active*/ ELSE 1 /*Not Active*/ END, discountcode, a.discountdesc, a.pcDisc_IncExcCat, a.pcDisc_IncExcPrd, ISNULL(a.expDate, DATEADD(year, 1, GETDATE())), ISNULL(a.pcDisc_StartDate, '2010-01-01'), CASE WHEN ISNULL(a.pcWholesaleFlag, 0) <> 0 THEN 2 ELSE CASE WHEN ISNULL(a.pcRetailFlag, 0) <> 0 THEN 1 ELSE NULL END END
FROM [vitalchoice2.0].[dbo].discounts AS a
WHERE ISNULL(a.singleProductPercent, 0) > 0

--=================== Free Shipping ====================

INSERT INTO [VitalChoice.Ecommerce].dbo.Discounts
(Id, DateCreated, DateEdited, IdObjectType, StatusCode, Code, Description, ExcludeCategories, ExcludeSkus, ExpirationDate, StartDate, Assigned)
SELECT 
a.iddiscount, GETDATE(), GETDATE(), 3, CASE WHEN ISNULL(a.active, 0) <> 0 THEN 2/*Active*/ ELSE 1 /*Not Active*/ END, discountcode, a.discountdesc, a.pcDisc_IncExcCat, a.pcDisc_IncExcPrd, ISNULL(a.expDate, DATEADD(year, 1, GETDATE())), ISNULL(a.pcDisc_StartDate, '2010-01-01'), CASE WHEN ISNULL(a.pcWholesaleFlag, 0) <> 0 THEN 2 ELSE CASE WHEN ISNULL(a.pcRetailFlag, 0) <> 0 THEN 1 ELSE NULL END END
FROM [vitalchoice2.0].[dbo].discounts AS a
INNER JOIN [vitalchoice2.0].[dbo].[pcDFShip] AS s ON s.pcFShip_IDDiscount = a.iddiscount

--================== Threshold =========================

INSERT INTO [VitalChoice.Ecommerce].dbo.Discounts
(Id, DateCreated, DateEdited, IdObjectType, StatusCode, Code, Description, ExcludeCategories, ExcludeSkus, ExpirationDate, StartDate, Assigned)
SELECT 
a.iddiscount, GETDATE(), GETDATE(), 4, CASE WHEN ISNULL(a.active, 0) <> 0 THEN 2/*Active*/ ELSE 1 /*Not Active*/ END, discountcode, a.discountdesc, a.pcDisc_IncExcCat, a.pcDisc_IncExcPrd, ISNULL(a.expDate, DATEADD(year, 1, GETDATE())), ISNULL(a.pcDisc_StartDate, '2010-01-01'), CASE WHEN ISNULL(a.pcWholesaleFlag, 0) <> 0 THEN 2 ELSE CASE WHEN ISNULL(a.pcRetailFlag, 0) <> 0 THEN 1 ELSE NULL END END
FROM [vitalchoice2.0].[dbo].discounts AS a
WHERE ISNULL(a.specialEnabled, 0) <> 0

--================== Tiered =========================

INSERT INTO [VitalChoice.Ecommerce].dbo.Discounts
(Id, DateCreated, DateEdited, IdObjectType, StatusCode, Code, Description, ExcludeCategories, ExcludeSkus, ExpirationDate, StartDate, Assigned)
SELECT 
a.iddiscount, GETDATE(), GETDATE(), 5, CASE WHEN ISNULL(a.active, 0) <> 0 THEN 2/*Active*/ ELSE 1 /*Not Active*/ END, discountcode, a.discountdesc, a.pcDisc_IncExcCat, a.pcDisc_IncExcPrd, ISNULL(a.expDate, DATEADD(year, 1, GETDATE())), ISNULL(a.pcDisc_StartDate, '2010-01-01'), CASE WHEN ISNULL(a.pcWholesaleFlag, 0) <> 0 THEN 2 ELSE CASE WHEN ISNULL(a.pcRetailFlag, 0) <> 0 THEN 1 ELSE NULL END END
FROM [vitalchoice2.0].[dbo].discounts AS a
WHERE a.tiered <> 0

SET IDENTITY_INSERT [VitalChoice.Ecommerce].dbo.Discounts OFF;

--======================= Google Category =============================

EXEC dbo.MoveDiscountField @destFieldName = N'MaxTimesUse', @sourceFieldName = N'onetime', @fieldOperation = N'CASE WHEN a.onetime <> 0 THEN N''1'' ELSE NULL END'
EXEC dbo.MoveDiscountField @destFieldName = N'RequireMinimumPerishable', @sourceFieldName = N'minPerishEnabled', @fieldOperation = N'CASE WHEN a.minPerishEnabled <> 0 THEN N''True'' ELSE N''False'' END'
EXEC dbo.MoveDiscountField @destFieldName = N'RequireMinimumPerishableAmount', @sourceFieldName = N'minPerish', @fieldOperation = N'CASE WHEN a.minPerishEnabled <> 0 THEN CAST(a.minPerish AS NVARCHAR(250)) ELSE NULL END'
EXEC dbo.MoveDiscountField @destFieldName = N'AllowHealthwise', @sourceFieldName = N'allowHealthwise', @fieldOperation = N'CASE WHEN ISNULL(a.allowHealthwise, 0) <> 0 THEN N''True'' ELSE N''False'' END'
EXEC dbo.MoveDiscountField @destFieldName = N'ProductSKU', @sourceFieldName = N'specialSku'
EXEC dbo.MoveDiscountField @destFieldName = N'Threshold', @sourceFieldName = N'specialThreshold', @fieldOperation = N'CAST(a.specialThreshold AS NVARCHAR(250))'
EXEC dbo.MoveDiscountField @destFieldName = N'Amount', @sourceFieldName = N'pricetodiscount', @fieldOperation = N'CAST(a.pricetodiscount AS NVARCHAR(250))', @sourceConditions = 'a.pricetodiscount > 0'
EXEC dbo.MoveDiscountField @destFieldName = N'Percent', @sourceFieldName = N'percentagetodiscount', @fieldOperation = N'CAST(a.percentagetodiscount AS NVARCHAR(250))', @sourceConditions = 'a.percentagetodiscount > 0'
EXEC dbo.MoveDiscountField @destFieldName = N'Percent', @sourceFieldName = N'singleProductPercent', @fieldOperation = N'CAST(a.singleProductPercent AS NVARCHAR(250))', @sourceConditions = 'a.singleProductPercent > 0'
	

INSERT INTO [VitalChoice.Ecommerce].dbo.DiscountsToCategories
(IdCategory, IdDiscount)
SELECT c.pcFCat_IDCategory, c.pcFCat_IDDiscount FROM [vitalchoice2.0].[dbo].[pcDFCats] AS c

INSERT INTO [VitalChoice.Ecommerce].dbo.DiscountsToSkus
(IdSku, IdDiscount)
SELECT s.Id, c.pcFPro_IDDiscount FROM [vitalchoice2.0].[dbo].[pcDFProds] AS c
INNER JOIN [VitalChoice.Ecommerce].dbo.Products AS p ON p.Id = c.pcFPro_IDProduct
INNER JOIN [VitalChoice.Ecommerce].dbo.Skus AS s ON s.IdProduct = p.Id

INSERT INTO [VitalChoice.Ecommerce].dbo.DiscountsToSelectedSkus
(IdDiscount, IdSku)
SELECT d.iddiscount, p.idProduct FROM [vitalchoice2.0].[dbo].[discounts] AS d
INNER JOIN [vitalchoice2.0].dbo.productDiscounts AS p ON p.idDiscount = d.iddiscount

INSERT INTO [VitalChoice.Ecommerce].dbo.DiscountTiers
(Amount, [Percent], [From], [To], IdDiscount, IdDiscountType, [Order])
SELECT d.tier1value, d.tier1value, d.tier1thresholdStart, d.tier1thresholdEnd, d.iddiscount, CASE WHEN d.tier1percent IS NOT NULL AND d.tier1percent > 0 THEN 2 ELSE 1 END, 0
FROM [vitalchoice2.0].dbo.discounts AS d
WHERE d.tiered <> 0 AND d.tier1thresholdStart > 0

INSERT INTO [VitalChoice.Ecommerce].dbo.DiscountTiers
(Amount, [Percent], [From], [To], IdDiscount, IdDiscountType, [Order])
SELECT d.tier2value, d.tier2value, d.tier1thresholdEnd + 0.01, d.tier2thresholdEnd, d.iddiscount, CASE WHEN d.tier2percent IS NOT NULL AND d.tier2percent > 0 THEN 2 ELSE 1 END, 1
FROM [vitalchoice2.0].dbo.discounts AS d
WHERE d.tiered <> 0 AND d.tier1thresholdEnd > 0 AND d.tier1thresholdStart > 0

INSERT INTO [VitalChoice.Ecommerce].dbo.DiscountTiers
(Amount, [Percent], [From], [To], IdDiscount, IdDiscountType, [Order])
SELECT d.tier3value, d.tier3value, d.tier2thresholdEnd + 0.01, d.tier3thresholdEnd, d.iddiscount, CASE WHEN d.tier3percent IS NOT NULL AND d.tier3percent > 0 THEN 2 ELSE 1 END, 2
FROM [vitalchoice2.0].dbo.discounts AS d
WHERE d.tiered <> 0 AND d.tier2thresholdEnd > 0 AND d.tier1thresholdEnd > 0 AND d.tier1thresholdStart > 0

GO

