USE [vitalchoice2.0]
GO

IF EXISTS(SELECT * FROM sys.procedures WHERE name = N'MoveProductTextField')
	DROP PROCEDURE dbo.MoveProductTextField

GO

CREATE PROCEDURE dbo.MoveProductTextField
(@sourceColumnName NVARCHAR(250), @destFieldName NVARCHAR(250), @sourceCondition NVARCHAR(MAX) = NULL)
AS
BEGIN
	DECLARE @sql NVARCHAR(MAX)

	IF @sourceCondition IS NOT NULL

	SET @sql = N'DECLARE @fieldType INT
	DECLARE @textData NVARCHAR(MAX), @IdProduct INT, @idObjectType INT
	DECLARE @bigId TABLE(Id BIGINT NOT NULL)

	DECLARE src CURSOR FOR
	SELECT a.'+@sourceColumnName+N', idProduct, p.IdObjectType FROM products AS a
	INNER JOIN [VitalChoice.Ecommerce].dbo.Products AS p ON p.Id = a.idProduct
	WHERE ('+@sourceCondition+N') AND a.'+@sourceColumnName+N' IS NOT NULL AND a.'+@sourceColumnName+N' <> N''''

	OPEN src

	FETCH NEXT FROM src
	INTO @textData, @IdProduct, @idObjectType

	WHILE @@FETCH_STATUS = 0
	BEGIN

		IF EXISTS(SELECT * FROM [VitalChoice.Ecommerce].dbo.ProductOptionTypes WHERE Name = N'''+@destFieldName+N''' AND (IdObjectType = @idObjectType OR IdObjectType IS NULL))
		BEGIN
			SELECT TOP 1 @fieldType = Id FROM [VitalChoice.Ecommerce].dbo.ProductOptionTypes WHERE Name = N'''+@destFieldName+N''' AND (IdObjectType = @idObjectType OR IdObjectType IS NULL)

			INSERT INTO [VitalChoice.Ecommerce].dbo.BigStringValues
			(Value)
			OUTPUT inserted.IdBigString INTO @bigId
			VALUES
			(@textData)

			INSERT INTO [VitalChoice.Ecommerce].dbo.ProductOptionValues
			(IdProduct, IdOptionType, IdBigString)
			VALUES
			(@IdProduct, @fieldType, (SELECT TOP 1 Id FROM @bigId))

			DELETE FROM @bigId
		END
		ELSE
			SELECT '''+@destFieldName+''', @idObjectType

		FETCH NEXT FROM src
		INTO @textData, @IdProduct, @idObjectType
	END

	CLOSE src;
	DEALLOCATE src;';

	ELSE

	SET @sql = N'DECLARE @fieldType INT
	DECLARE @textData NVARCHAR(MAX), @IdProduct INT, @idObjectType INT
	DECLARE @bigId TABLE(Id BIGINT NOT NULL)

	DECLARE src CURSOR FOR
	SELECT a.'+@sourceColumnName+N', idProduct, p.IdObjectType FROM products AS a
	INNER JOIN [VitalChoice.Ecommerce].dbo.Products AS p ON p.Id = a.idProduct
	WHERE a.'+@sourceColumnName+N' IS NOT NULL AND a.'+@sourceColumnName+N' <> N''''

	OPEN src

	FETCH NEXT FROM src
	INTO @textData, @IdProduct, @idObjectType

	WHILE @@FETCH_STATUS = 0
	BEGIN

		IF EXISTS(SELECT * FROM [VitalChoice.Ecommerce].dbo.ProductOptionTypes WHERE Name = N'''+@destFieldName+N''' AND (IdObjectType = @idObjectType OR IdObjectType IS NULL))
		BEGIN
			SELECT TOP 1 @fieldType = Id FROM [VitalChoice.Ecommerce].dbo.ProductOptionTypes WHERE Name = N'''+@destFieldName+N''' AND (IdObjectType = @idObjectType OR IdObjectType IS NULL)

			INSERT INTO [VitalChoice.Ecommerce].dbo.BigStringValues
			(Value)
			OUTPUT inserted.IdBigString INTO @bigId
			VALUES
			(@textData)

			INSERT INTO [VitalChoice.Ecommerce].dbo.ProductOptionValues
			(IdProduct, IdOptionType, IdBigString)
			VALUES
			(@IdProduct, @fieldType, (SELECT TOP 1 Id FROM @bigId))

			DELETE FROM @bigId
		END
		ELSE
			SELECT '''+@destFieldName+''', @idObjectType

		FETCH NEXT FROM src
		INTO @textData, @IdProduct, @idObjectType
	END

	CLOSE src;
	DEALLOCATE src;';

	EXEC (@sql)
END
GO

IF EXISTS(SELECT * FROM sys.procedures WHERE name = N'MoveProductSmallField')
	DROP PROCEDURE dbo.MoveProductSmallField
GO

CREATE PROCEDURE dbo.MoveProductSmallField
(@destFieldName NVARCHAR(250), @sourceFieldName NVARCHAR(250), @fieldOperation NVARCHAR(MAX) = NULL, @sourceConditions NVARCHAR(MAX) = NULL)
AS
BEGIN
BEGIN TRY
	DECLARE @sql NVARCHAR(MAX)
	
	IF @fieldOperation IS NULL

		SET @sql = N'INSERT INTO [VitalChoice.Ecommerce].dbo.ProductOptionValues
		(IdOptionType, IdProduct, Value)
		SELECT t.Id, p.Id, a.'+@sourceFieldName+' FROM [VitalChoice.Ecommerce].dbo.Products AS p
		INNER JOIN [vitalchoice2.0].dbo.products AS a ON a.idProduct = p.Id
		INNER JOIN [VitalChoice.Ecommerce].dbo.ProductOptionTypes AS t ON (t.IdObjectType = p.IdObjectType OR t.IdObjectType IS NULL) AND t.Name = N'''+@destFieldName+'''
		WHERE a.'+@sourceFieldName+' IS NOT NULL AND ('+ISNULL(@sourceConditions, '1=1')+')';

	ELSE
		
		SET @sql = N'INSERT INTO [VitalChoice.Ecommerce].dbo.ProductOptionValues
		(IdOptionType, IdProduct, Value)
		SELECT t.Id, p.Id, '+@fieldOperation+' FROM [VitalChoice.Ecommerce].dbo.Products AS p
		INNER JOIN [vitalchoice2.0].dbo.products AS a ON a.idProduct = p.Id
		INNER JOIN [VitalChoice.Ecommerce].dbo.ProductOptionTypes AS t ON (t.IdObjectType = p.IdObjectType OR t.IdObjectType IS NULL) AND t.Name = N'''+@destFieldName+'''
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

IF EXISTS(SELECT * FROM sys.procedures WHERE name = N'MoveSkuField')
	DROP PROCEDURE dbo.MoveSkuField
GO

CREATE PROCEDURE dbo.MoveSkuField
(@destFieldName NVARCHAR(250), @sourceFieldName NVARCHAR(250), @conversion NVARCHAR(MAX) = NULL, @sourceConditions NVARCHAR(MAX) = NULL)
AS
BEGIN
BEGIN TRY
	DECLARE @sql NVARCHAR(MAX)
	
	IF @conversion IS NULL

		SET @sql = N'INSERT INTO [VitalChoice.Ecommerce].dbo.SkuOptionValues
		(IdOptionType, IdSku, Value)
		SELECT t.Id, s.Id, a.'+@sourceFieldName+' FROM [VitalChoice.Ecommerce].dbo.Skus AS s
		INNER JOIN [VitalChoice.Ecommerce].dbo.Products AS p ON p.Id = s.IdProduct
		INNER JOIN [vitalchoice2.0].dbo.products AS a ON a.idProduct = s.Id
		INNER JOIN [VitalChoice.Ecommerce].dbo.ProductOptionTypes AS t ON (t.IdObjectType = p.IdObjectType OR t.IdObjectType IS NULL) AND t.Name = N'''+@destFieldName+'''
		WHERE a.'+@sourceFieldName+' IS NOT NULL AND ('+ISNULL(@sourceConditions, '1=1')+')';

	ELSE
		
		SET @sql = N'INSERT INTO [VitalChoice.Ecommerce].dbo.SkuOptionValues
		(IdOptionType, IdSku, Value)
		SELECT t.Id, s.Id, '+@conversion+' FROM [VitalChoice.Ecommerce].dbo.Skus AS s
		INNER JOIN [VitalChoice.Ecommerce].dbo.Products AS p ON p.Id = s.IdProduct
		INNER JOIN [vitalchoice2.0].dbo.products AS a ON a.idProduct = s.Id
		INNER JOIN [VitalChoice.Ecommerce].dbo.ProductOptionTypes AS t ON (t.IdObjectType = p.IdObjectType OR t.IdObjectType IS NULL) AND t.Name = N'''+@destFieldName+'''
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

ALTER TABLE [VitalChoice.Infrastructure].dbo.ContentItems
	ADD TempId INT NULL,
		TempCategoryId INT NULL

GO

USE [VitalChoice.Ecommerce]
GO

--BEGIN TRANSACTION
--BEGIN TRY

--=========== Wipe everything ==============

	DELETE FROM BigStringValues
	WHERE IdBigString IN (SELECT v.IdBigString FROM ProductOptionValues AS v WHERE v.IdBigString IS NOT NULL)

	DELETE FROM BigStringValues
	WHERE IdBigString NOT IN (SELECT v.IdBigString FROM CustomerOptionValues AS v WHERE v.IdBigString IS NOT NULL UNION SELECT v.IdBigString FROM AffiliateOptionValues AS v WHERE v.IdBigString IS NOT NULL)

	DELETE FROM SkusToInventorySkus
	DELETE FROM SkuOptionValues
	DELETE FROM ReshipProblemSkus
	DELETE FROM RefundSkus
	DELETE FROM PromotionsToGetSkus
	DELETE FROM PromotionsToBuySkus
	DELETE FROM OrderToSkusToInventorySkus
	DELETE FROM OrderToSkus
	DELETE FROM OrderToPromosToInventorySkus
	DELETE FROM OrderToPromos
	DELETE FROM RefundOrderToGiftCertificates
	DELETE FROM OrderToGiftCertificates
	DELETE FROM CartToGiftCertificates
	DELETE FROM GiftCertificates
	DELETE FROM DiscountsToSkus
	DELETE FROM DiscountsToSelectedSkus
	DELETE FROM CartToSkus
	DELETE FROM Skus
	DELETE FROM ProductsToCategories
	DELETE FROM ProductReviews
	DELETE FROM ProductOutOfStockRequests
	DELETE FROM ProductOptionValues
	DELETE FROM Products
	DELETE FROM Carts
	DELETE FROM AffiliateOrderPayments
	DELETE FROM HealthwiseOrders
	DELETE FROM HelpTicketComments
	DELETE FROM HelpTickets
	DELETE FROM OrderOptionValues
	DELETE FROM Orders
	DELETE FROM PromotionsToSelectedCategories
	DELETE FROM DiscountToSelectedCategories
	DELETE FROM DiscountsToCategories
	DELETE FROM ProductCategories
	WHERE Id <> 1

	USE [VitalChoice.Infrastructure]

	DECLARE @contentRemoved TABLE(Id INT)

	INSERT INTO @contentRemoved
	SELECT ContentItemId FROM Products

	INSERT INTO @contentRemoved
	SELECT ContentItemId FROM ProductCategories
	WHERE Id <> 1

	DELETE FROM Products
	DELETE FROM ProductCategories
	WHERE Id <> 1

	DELETE FROM ContentItems
	WHERE Id IN (SELECT Id FROM @contentRemoved)


--=========== Import ==============

	USE [vitalchoice2.0]


	DECLARE @contentType INT, @oldContentType INT, @masterName NVARCHAR(50), @categoryMasterName NVARCHAR(50)
	SET @contentType = 9
	SET @oldContentType = 0
	SET @masterName = N'Product page'
	SET @categoryMasterName = N'Product sub categories'


	DECLARE @contentItemsToDelete TABLE(Id INT)

	INSERT INTO @contentItemsToDelete
	(Id)
	SELECT ContentItemId FROM [VitalChoice.Infrastructure].dbo.Products
	DELETE FROM [VitalChoice.Infrastructure].dbo.Products

	DELETE FROM [VitalChoice.Infrastructure].dbo.ContentItems
	WHERE Id IN (SELECT Id FROM @contentItemsToDelete)

	DECLARE @articleMasterId INT
	SELECT @articleMasterId = Id FROM [VitalChoice.Infrastructure].dbo.MasterContentItems WHERE Name = @masterName

	INSERT [VitalChoice.Infrastructure].dbo.ContentItems
	(Created, Description, MetaDescription, MetaKeywords, Title, Updated, TempId, Template)
	--OUTPUT inserted.Id, inserted.TempId INTO @insertedArticles
	SELECT ISNULL(a.pcProd_EditedDate, GETDATE()), ISNULL(a.details, N''), LEFT(a.pcProd_MetaDesc, 250), a.pcProd_MetaKeywords, a.pcProd_MetaTitle, ISNULL(a.pcProd_EditedDate, GETDATE()), a.idProduct, N''
	FROM [vitalchoice2.0].[dbo].Products AS a
	WHERE a.pcprod_ParentPrd = 0
	ORDER BY a.idProduct

	INSERT INTO [VitalChoice.Infrastructure].dbo.Products
	(Id, ContentItemId, MasterContentItemId, StatusCode, Url, IdOld)
	SELECT a.idProduct, i.Id, @articleMasterId, CASE WHEN ISNULL(a.removed, 0) <> 0 THEN 3 ELSE CASE WHEN ISNULL(a.active, 0) <> 0 THEN 2/*Active*/ ELSE 1 /*Not Active*/ END END, REPLACE(RTRIM(LTRIM(LOWER([vitalchoice2.0].[dbo].RegexReplace('[^a-zA-Z0-9]+', a.description, ' ')))) COLLATE SQL_Latin1_General_CP1_CI_AS,' ','-'), a.idProduct
	FROM [vitalchoice2.0].[dbo].Products AS a
	INNER JOIN [VitalChoice.Infrastructure].dbo.ContentItems AS i ON i.TempId = a.idProduct
	WHERE a.pcprod_ParentPrd = 0

	DECLARE @articleSubMaster INT
	SELECT @articleSubMaster = Id FROM [VitalChoice.Infrastructure].dbo.MasterContentItems WHERE Name = @categoryMasterName

	--DECLARE @insertedArticleCategories TABLE (Id INT NOT NULL PRIMARY KEY, TempCategoryId INT NOT NULL)
	
	INSERT [VitalChoice.Infrastructure].dbo.ContentItems
	(Created, Description, MetaDescription, MetaKeywords, Title, Updated, TempCategoryId, Template)
	--OUTPUT inserted.Id, inserted.TempCategoryId INTO @insertedArticleCategories
	SELECT ISNULL(ca.pcCats_EditedDate, GETDATE()), N'', LEFT(CAST(ca.pcCats_MetaDesc AS NVARCHAR(MAX)), 250), ca.pcCats_MetaKeywords, ca.pcCats_MetaTitle, GETDATE(), ca.idCategory, N''
	FROM [vitalchoice2.0].dbo.categories AS ca 
	WHERE ca.type=@oldContentType AND ca.idCategory <> 1
	ORDER BY ca.idParentCategory

	INSERT [VitalChoice.Infrastructure].dbo.ProductCategories
	(Id, ContentItemId, MasterContentItemId, IdOld, NavLabel, StatusCode, Url, NavIdVisible, FileImageLargeUrl, FileImageSmallUrl, HideLongDescription, HideLongDescriptionBottom, LongDescription, LongDescriptionBottom)
	SELECT ca.idCategory, c.Id, @articleSubMaster, ca.idCategory, REPLACE(ca.categoryDesc, '&amp;', '&'),  
		2/*Active*/, REPLACE(RTRIM(LTRIM(LOWER([vitalchoice2.0].[dbo].RegexReplace('[^a-zA-Z0-9]+', ca.categoryDesc, ' ')))) COLLATE SQL_Latin1_General_CP1_CI_AS,' ','-'),
		CASE WHEN ISNULL(ca.iBTOhide, 0) <> 0 THEN NULL ELSE CASE WHEN ISNULL(ca.pcCats_RetailHide, 0) <> 0 THEN 2 ELSE 1 END END,
		REPLACE(N'/files/catalog/' + ca.largeimage, '//', '/'), REPLACE(N'/files/catalog/' + ca.image, '//', '/'), CASE WHEN ISNULL(ca.HideDesc, 0) <> 0 THEN 1 ELSE 0 END, CASE WHEN ISNULL(ca.HideDesc, 0) <> 0 THEN 1 ELSE 0 END, ca.LDesc, ca.LDesc2
	FROM [vitalchoice2.0].dbo.categories AS ca
	INNER JOIN [VitalChoice.Infrastructure].dbo.ContentItems AS c ON c.TempCategoryId = ca.idCategory
	WHERE ca.type=@oldContentType AND ca.idCategory <> 1
	ORDER BY ca.idParentCategory

	SET IDENTITY_INSERT [VitalChoice.Ecommerce].dbo.Products ON;
	
	--================== Non-Perishable =====================

	INSERT INTO [VitalChoice.Ecommerce].dbo.Products
	(Id, DateCreated, DateEdited, Hidden, IdObjectType, Name, PublicId, StatusCode)
	SELECT 
	a.idProduct, ISNULL(a.pcProd_EditedDate, GETDATE()), ISNULL(a.pcProd_EditedDate, GETDATE()), a.hidden, 1, a.description, NEWID(), CASE WHEN ISNULL(a.removed, 0) <> 0 THEN 3 ELSE CASE WHEN ISNULL(a.active, 0) <> 0 THEN 2/*Active*/ ELSE 1 /*Not Active*/ END END
	FROM [vitalchoice2.0].[dbo].Products AS a
	WHERE a.pcprod_ParentPrd = 0 AND ISNULL(a.pcprod_GC, 0) = 0 AND ISNULL(a.perishable, 0) = 0

	--================== Perishable =========================

	INSERT INTO [VitalChoice.Ecommerce].dbo.Products
	(Id, DateCreated, DateEdited, Hidden, IdObjectType, Name, PublicId, StatusCode)
	SELECT 
	a.idProduct, ISNULL(a.pcProd_EditedDate, GETDATE()), ISNULL(a.pcProd_EditedDate, GETDATE()), a.hidden, 2, a.description, NEWID(), CASE WHEN ISNULL(a.removed, 0) <> 0 THEN 3 ELSE CASE WHEN ISNULL(a.active, 0) <> 0 THEN 2/*Active*/ ELSE 1 /*Not Active*/ END END
	FROM [vitalchoice2.0].[dbo].Products AS a
	WHERE a.pcprod_ParentPrd = 0 AND ISNULL(a.pcprod_GC, 0) = 0 AND ISNULL(a.perishable, 0) = -1

	--================== Gift Certificates =========================

	INSERT INTO [VitalChoice.Ecommerce].dbo.Products
	(Id, DateCreated, DateEdited, Hidden, IdObjectType, Name, PublicId, StatusCode)
	SELECT 
	a.idProduct, ISNULL(a.pcProd_EditedDate, GETDATE()), ISNULL(a.pcProd_EditedDate, GETDATE()), a.hidden, 3, a.description, NEWID(), CASE WHEN ISNULL(a.removed, 0) <> 0 THEN 3 ELSE CASE WHEN ISNULL(a.active, 0) <> 0 THEN 2/*Active*/ ELSE 1 /*Not Active*/ END END
	FROM [vitalchoice2.0].[dbo].Products AS a
	WHERE a.pcprod_ParentPrd = 0 AND ISNULL(a.pcprod_GC, 0) = 1 AND a.sku LIKE 'EGIFT%'

	INSERT INTO [VitalChoice.Ecommerce].dbo.Products
	(Id, DateCreated, DateEdited, Hidden, IdObjectType, Name, PublicId, StatusCode)
	SELECT 
	a.idProduct, ISNULL(a.pcProd_EditedDate, GETDATE()), ISNULL(a.pcProd_EditedDate, GETDATE()), a.hidden, 4, a.description, NEWID(), CASE WHEN ISNULL(a.removed, 0) <> 0 THEN 3 ELSE CASE WHEN ISNULL(a.active, 0) <> 0 THEN 2/*Active*/ ELSE 1 /*Not Active*/ END END
	FROM [vitalchoice2.0].[dbo].Products AS a
	WHERE a.pcprod_ParentPrd = 0 AND ISNULL(a.pcprod_GC, 0) = 1 AND a.sku NOT LIKE 'EGIFT%'

	SET IDENTITY_INSERT [VitalChoice.Ecommerce].dbo.Products OFF;

	SET IDENTITY_INSERT [VitalChoice.Ecommerce].dbo.ProductCategories ON;

	INSERT [VitalChoice.Ecommerce].dbo.ProductCategories
	(Id, Name, StatusCode, [Order])
	SELECT ca.idCategory, REPLACE(ca.categoryDesc, '&amp;', '&'), 2/*Active*/, ISNULL(ca.[priority], 0)
	FROM [vitalchoice2.0].dbo.categories AS ca
	WHERE ca.type=@oldContentType AND ca.idCategory <> 1
	ORDER BY ca.idParentCategory

	SET IDENTITY_INSERT [VitalChoice.Ecommerce].dbo.ProductCategories OFF;

	UPDATE [VitalChoice.Ecommerce].dbo.ProductCategories
	SET ParentId = ca.idParentCategory
	FROM [VitalChoice.Ecommerce].dbo.ProductCategories AS c
	INNER JOIN [vitalchoice2.0].dbo.categories AS ca ON ca.idCategory = c.Id AND ca.idCategory <> 1
	WHERE ca.type=@oldContentType AND ca.idParentCategory IN (SELECT Id FROM [VitalChoice.Ecommerce].dbo.ProductCategories)

	UPDATE [VitalChoice.Infrastructure].dbo.ProductCategories
	SET NavIdVisible = NULL
	WHERE Id IN (SELECT c.Id FROM [VitalChoice.Ecommerce].dbo.ProductCategories AS c WHERE c.ParentId IS NULL AND c.Id <> 1)

	UPDATE [VitalChoice.Ecommerce].dbo.ProductCategories
	SET ParentId = 1
	WHERE ParentId IS NULL AND Id <> 1

-- Move fields

	EXEC dbo.MoveProductTextField @sourceColumnName = N'details', @destFieldName = N'Description'
	EXEC dbo.MoveProductTextField @sourceColumnName = N'ingredients', @destFieldName = N'Ingredients'
	EXEC dbo.MoveProductTextField @sourceColumnName = N'pcProd_PrdNotes', @destFieldName = N'ProductNotes'
	EXEC dbo.MoveProductTextField @sourceColumnName = N'recipes', @destFieldName = N'Recipes'
	EXEC dbo.MoveProductTextField @sourceColumnName = N'serving', @destFieldName = N'Serving'
	EXEC dbo.MoveProductTextField @sourceColumnName = N'sDesc', @destFieldName = N'ShortDescription'

	INSERT INTO [VitalChoice.Ecommerce].dbo.ProductOptionValues
	(IdOptionType, IdProduct, Value)
	SELECT t.Id, p.Id, data.Item FROM [VitalChoice.Ecommerce].dbo.Products AS p
	INNER JOIN (
		SELECT 
			CASE d.ItemNumber 
				WHEN 1 THEN N'NutritionalTitle'
				WHEN 2 THEN N'ServingSize'
				WHEN 3 THEN N'Servings'
				WHEN 4 THEN N'Calories'
				WHEN 5 THEN N'CaloriesFromFat'
				WHEN 6 THEN N'TotalFat'
				WHEN 7 THEN N'TotalFatPercent'
				WHEN 8 THEN N'SaturatedFat'
				WHEN 9 THEN N'SaturatedFatPercent'
				WHEN 10 THEN N'TransFat'
				WHEN 11 THEN N'TransFatPercent'
				WHEN 12 THEN N'Cholesterol'
				WHEN 13 THEN N'CholesterolPercent'
				WHEN 14 THEN N'Sodium'
				WHEN 15 THEN N'SodiumPercent'
				WHEN 16 THEN N'TotalCarbohydrate'
				WHEN 17 THEN N'TotalCarbohydratePercent'
				WHEN 18 THEN N'DietaryFiber'
				WHEN 19 THEN N'DietaryFiberPercent'
				WHEN 20 THEN N'Sugars'
				WHEN 21 THEN N'SugarsPercent'
				WHEN 22 THEN N'Protein'
				WHEN 23 THEN N'ProteinPercent'
				WHEN 24 THEN N'AdditionalNotes'
			ELSE
				'INVALID'
			END AS FieldName, 
			d.Item, 
			p.idProduct 
		FROM products AS p
		CROSS APPLY [dbo].[DelimitedSplit8K](p.nutritionInfo, '|~|', p.idProduct) AS d
		WHERE d.JoinKey = p.idProduct AND p.removed = 0 AND p.pcprod_ParentPrd = 0 AND p.nutritionInfo IS NOT NULL AND p.nutritionInfo <> ''
	) AS data ON data.idProduct = p.Id
	INNER JOIN [VitalChoice.Ecommerce].dbo.ProductOptionTypes AS t ON t.Name = data.FieldName AND (t.IdObjectType = p.IdObjectType OR t.IdObjectType IS NULL)
	WHERE LEN(data.Item) <= 250

--======================== Additional Long Notes (Nutrition) =================================

	DECLARE @fieldType INT
	DECLARE @textData NVARCHAR(MAX), @IdProduct INT, @idObjectType INT

	DECLARE src CURSOR FOR
	SELECT data.Item, p.Id, p.IdObjectType FROM [VitalChoice.Ecommerce].dbo.Products AS p
	INNER JOIN (
		SELECT 
			CASE d.ItemNumber 
				WHEN 24 THEN N'AdditionalNotes'
			ELSE
				'INVALID'
			END AS FieldName, 
			d.Item, 
			p.idProduct 
		FROM products AS p
		CROSS APPLY [dbo].[DelimitedSplit8K](p.nutritionInfo, '|~|', p.idProduct) AS d
		WHERE d.JoinKey = p.idProduct AND p.removed = 0 AND p.pcprod_ParentPrd = 0 AND p.nutritionInfo IS NOT NULL AND p.nutritionInfo <> ''
	) AS data ON data.idProduct = p.Id
	INNER JOIN [VitalChoice.Ecommerce].dbo.ProductOptionTypes AS t ON t.Name = data.FieldName AND (t.IdObjectType = p.IdObjectType OR t.IdObjectType IS NULL)
	WHERE LEN(data.Item) > 250 AND data.FieldName = N'AdditionalNotes'

	OPEN src

	FETCH NEXT FROM src
	INTO @textData, @IdProduct, @idObjectType

	WHILE @@FETCH_STATUS = 0
	BEGIN

		IF EXISTS(SELECT * FROM [VitalChoice.Ecommerce].dbo.ProductOptionTypes WHERE Name = N'AdditionalNotes' AND IdObjectType = @idObjectType)
		BEGIN
			SELECT TOP 1 @fieldType = Id FROM [VitalChoice.Ecommerce].dbo.ProductOptionTypes WHERE Name = N'AdditionalNotes' AND (IdObjectType = @idObjectType OR IdObjectType IS NULL)

			INSERT INTO [VitalChoice.Ecommerce].dbo.BigStringValues
			(Value)
			VALUES
			(@textData)

			INSERT INTO [VitalChoice.Ecommerce].dbo.ProductOptionValues
			(IdProduct, IdOptionType, IdBigString)
			VALUES
			(@IdProduct, @fieldType, SCOPE_IDENTITY())
		END

		FETCH NEXT FROM src
		INTO @textData, @IdProduct, @idObjectType
	END

	CLOSE src;
	DEALLOCATE src;

--======================= Google Category =============================
	DECLARE @googleCategoriesLookup INT

	SELECT TOP 1 @googleCategoriesLookup = Id FROM [VitalChoice.Ecommerce].dbo.Lookups AS l WHERE l.Name = 'GoogleCategories'

	DELETE FROM [VitalChoice.Ecommerce].dbo.LookupVariants WHERE IdLookup = @googleCategoriesLookup

	INSERT INTO [VitalChoice.Ecommerce].dbo.LookupVariants
	(Id, IdLookup, [Order], ValueVariant)
	SELECT c.id, @googleCategoriesLookup, ROW_NUMBER() OVER (ORDER BY c.id), c.google_category_name FROM [vitalchoice2.0].dbo.google_categories AS c

	INSERT INTO [VitalChoice.Ecommerce].dbo.ProductOptionValues
	(IdOptionType, IdProduct, Value)
	SELECT t.Id, p.Id, CAST(l.Id AS NVARCHAR(20)) FROM [VitalChoice.Ecommerce].dbo.Products AS p
	INNER JOIN [vitalchoice2.0].dbo.products AS a ON a.idProduct = p.Id
	INNER JOIN [VitalChoice.Ecommerce].dbo.ProductOptionTypes AS t ON (t.IdObjectType = p.IdObjectType OR t.IdObjectType IS NULL) AND t.Name = N'GoogleCategory'
	INNER JOIN [VitalChoice.Ecommerce].dbo.LookupVariants AS l ON l.IdLookup = t.IdLookup AND l.ValueVariant COLLATE SQL_Latin1_General_CP1_CI_AS = a.google_category

	INSERT [VitalChoice.Ecommerce].dbo.ProductsToCategories
	(IdProduct, IdCategory)
	SELECT a.Id, c.Id FROM [VitalChoice.Infrastructure].dbo.Products AS a
	INNER JOIN [vitalchoice2.0].dbo.categories_products AS ca ON ca.idProduct = a.Id AND ca.idCategory <> 1
	INNER JOIN [VitalChoice.Infrastructure].dbo.ProductCategories AS c ON c.IdOld = ca.idCategory
	
	UPDATE [VitalChoice.Infrastructure].dbo.Products
	SET Url = r.Url + N'-' + CAST(j.Number AS NVARCHAR(10))
	FROM [VitalChoice.Infrastructure].dbo.Products AS r
	INNER JOIN 
	(
		SELECT ROW_NUMBER() OVER (PARTITION BY r.Url ORDER BY r.Id) AS Number, r.Id
		FROM [VitalChoice.Infrastructure].dbo.Products AS r
		WHERE r.Url IN 
		(
			SELECT Url FROM [VitalChoice.Infrastructure].dbo.Products
			GROUP BY Url
			HAVING COUNT(Url) > 1
		)
	) AS j ON j.Id = r.Id
	WHERE j.Number > 1

	UPDATE [VitalChoice.Infrastructure].dbo.ProductCategories
	SET Url = r.Url + N'-' + CAST(j.Number AS NVARCHAR(10))
	FROM [VitalChoice.Infrastructure].dbo.ProductCategories AS r
	INNER JOIN 
	(
		SELECT ROW_NUMBER() OVER (PARTITION BY r.Url ORDER BY r.Id) AS Number, r.Id
		FROM [VitalChoice.Infrastructure].dbo.ProductCategories AS r
		WHERE r.Url IN 
		(
			SELECT Url FROM [VitalChoice.Infrastructure].dbo.ProductCategories
			GROUP BY Url
			HAVING COUNT(Url) > 1
		)
	) AS j ON j.Id = r.Id
	WHERE j.Number > 1

	EXEC dbo.MoveProductSmallField @destFieldName = N'SubProductGroupName', @sourceFieldName = N'idProduct', @fieldOperation = N'(SELECT ogi.OptionGroupDesc FROM [vitalchoice2.0].[dbo].[pcProductsOptions] AS po INNER JOIN [vitalchoice2.0].[dbo].optionsGroups AS ogi ON ogi.idOptionGroup = po.idOptionGroup WHERE po.idProduct = a.idProduct)'
	EXEC dbo.MoveProductSmallField @destFieldName = N'SpecialIcon', @sourceFieldName = N'mscicon', @fieldOperation = N'CAST(a.mscicon AS NVARCHAR(250))'
	EXEC dbo.MoveProductSmallField @destFieldName = N'TaxCode', @sourceFieldName = N'TaxCode'
	EXEC dbo.MoveProductSmallField @destFieldName = N'Thumbnail', @sourceFieldName = N'smallImageUrl', @fieldOperation = N'REPLACE(''/files/catalog/'' + a.smallImageUrl, ''//'', ''/'')'
	EXEC dbo.MoveProductSmallField @destFieldName = N'MainProductImage', @sourceFieldName = N'imageUrl', @fieldOperation = N'REPLACE(''/files/catalog/'' + a.imageUrl, ''//'', ''/'')'
	
	EXEC dbo.MoveProductSmallField @destFieldName = N'CrossSellImage1', @sourceFieldName = N'crossSellImg1', @fieldOperation = N'REPLACE(a.crossSellImg1, ''/shop/pc/catalog/'',''/files/catalog/'')'
	EXEC dbo.MoveProductSmallField @destFieldName = N'CrossSellImage2', @sourceFieldName = N'crossSellImg2', @fieldOperation = N'REPLACE(a.crossSellImg2, ''/shop/pc/catalog/'',''/files/catalog/'')'
	EXEC dbo.MoveProductSmallField @destFieldName = N'CrossSellImage3', @sourceFieldName = N'crossSellImg3', @fieldOperation = N'REPLACE(a.crossSellImg3, ''/shop/pc/catalog/'',''/files/catalog/'')'
	EXEC dbo.MoveProductSmallField @destFieldName = N'CrossSellImage4', @sourceFieldName = N'crossSellImg4', @fieldOperation = N'REPLACE(a.crossSellImg4, ''/shop/pc/catalog/'',''/files/catalog/'')'
	
	EXEC dbo.MoveProductSmallField @destFieldName = N'CrossSellUrl1', @sourceFieldName = N'crossSellUrl1', @fieldOperation = N'''/product/'' + (SELECT Url FROM [VitalChoice.Infrastructure].dbo.Products AS p WHERE p.Id = CAST([vitalchoice2.0].dbo.RegexReplace(''.*shop/pc/viewPrd.asp\?idproduct=([0-9]+).*'', a.crossSellUrl1, ''$1'') AS INT))', @sourceConditions = 'a.crossSellUrl1 LIKE N''%viewPrd.asp%'''
	EXEC dbo.MoveProductSmallField @destFieldName = N'CrossSellUrl2', @sourceFieldName = N'crossSellUrl2', @fieldOperation = N'''/product/'' + (SELECT Url FROM [VitalChoice.Infrastructure].dbo.Products AS p WHERE p.Id = CAST([vitalchoice2.0].dbo.RegexReplace(''.*shop/pc/viewPrd.asp\?idproduct=([0-9]+).*'', a.crossSellUrl2, ''$1'') AS INT))', @sourceConditions = 'a.crossSellUrl2 LIKE N''%viewPrd.asp%'''
	EXEC dbo.MoveProductSmallField @destFieldName = N'CrossSellUrl3', @sourceFieldName = N'crossSellUrl3', @fieldOperation = N'''/product/'' + (SELECT Url FROM [VitalChoice.Infrastructure].dbo.Products AS p WHERE p.Id = CAST([vitalchoice2.0].dbo.RegexReplace(''.*shop/pc/viewPrd.asp\?idproduct=([0-9]+).*'', a.crossSellUrl3, ''$1'') AS INT))', @sourceConditions = 'a.crossSellUrl3 LIKE N''%viewPrd.asp%'''
	EXEC dbo.MoveProductSmallField @destFieldName = N'CrossSellUrl4', @sourceFieldName = N'crossSellUrl4', @fieldOperation = N'''/product/'' + (SELECT Url FROM [VitalChoice.Infrastructure].dbo.Products AS p WHERE p.Id = CAST([vitalchoice2.0].dbo.RegexReplace(''.*shop/pc/viewPrd.asp\?idproduct=([0-9]+).*'', a.crossSellUrl4, ''$1'') AS INT))', @sourceConditions = 'a.crossSellUrl4 LIKE N''%viewPrd.asp%'''
	
	EXEC dbo.MoveProductSmallField @destFieldName = N'CrossSellUrl1', @sourceFieldName = N'crossSellUrl1', @fieldOperation = N'''/category/'' + (SELECT Url FROM [VitalChoice.Infrastructure].dbo.ProductCategories AS p WHERE p.Id = CAST([vitalchoice2.0].dbo.RegexReplace(''.*shop/pc/viewcategories.asp\?idcategory=([0-9]+).*'', a.crossSellUrl1, ''$1'') AS INT))', @sourceConditions = 'a.crossSellUrl1 LIKE N''%viewcategories.asp%'''
	EXEC dbo.MoveProductSmallField @destFieldName = N'CrossSellUrl2', @sourceFieldName = N'crossSellUrl2', @fieldOperation = N'''/category/'' + (SELECT Url FROM [VitalChoice.Infrastructure].dbo.ProductCategories AS p WHERE p.Id = CAST([vitalchoice2.0].dbo.RegexReplace(''.*shop/pc/viewcategories.asp\?idcategory=([0-9]+).*'', a.crossSellUrl2, ''$1'') AS INT))', @sourceConditions = 'a.crossSellUrl2 LIKE N''%viewcategories.asp%'''
	EXEC dbo.MoveProductSmallField @destFieldName = N'CrossSellUrl3', @sourceFieldName = N'crossSellUrl3', @fieldOperation = N'''/category/'' + (SELECT Url FROM [VitalChoice.Infrastructure].dbo.ProductCategories AS p WHERE p.Id = CAST([vitalchoice2.0].dbo.RegexReplace(''.*shop/pc/viewcategories.asp\?idcategory=([0-9]+).*'', a.crossSellUrl3, ''$1'') AS INT))', @sourceConditions = 'a.crossSellUrl3 LIKE N''%viewcategories.asp%'''
	EXEC dbo.MoveProductSmallField @destFieldName = N'CrossSellUrl4', @sourceFieldName = N'crossSellUrl4', @fieldOperation = N'''/category/'' + (SELECT Url FROM [VitalChoice.Infrastructure].dbo.ProductCategories AS p WHERE p.Id = CAST([vitalchoice2.0].dbo.RegexReplace(''.*shop/pc/viewcategories.asp\?idcategory=([0-9]+).*'', a.crossSellUrl4, ''$1'') AS INT))', @sourceConditions = 'a.crossSellUrl4 LIKE N''%viewcategories.asp%'''

	EXEC dbo.MoveProductSmallField @destFieldName = N'YouTubeImage1', @sourceFieldName = N'videoImage1', @fieldOperation = N'REPLACE(a.videoImage1, ''/shop/pc/catalog/'',''/files/catalog/'')'
	EXEC dbo.MoveProductSmallField @destFieldName = N'YouTubeImage2', @sourceFieldName = N'videoImage2', @fieldOperation = N'REPLACE(a.videoImage2, ''/shop/pc/catalog/'',''/files/catalog/'')'
	EXEC dbo.MoveProductSmallField @destFieldName = N'YouTubeImage3', @sourceFieldName = N'videoImage3', @fieldOperation = N'REPLACE(a.videoImage3, ''/shop/pc/catalog/'',''/files/catalog/'')'
	EXEC dbo.MoveProductSmallField @destFieldName = N'YouTubeText1', @sourceFieldName = N'text1'
	EXEC dbo.MoveProductSmallField @destFieldName = N'YouTubeText2', @sourceFieldName = N'text2'
	EXEC dbo.MoveProductSmallField @destFieldName = N'YouTubeText3', @sourceFieldName = N'text3'
	EXEC dbo.MoveProductSmallField @destFieldName = N'YouTubeVideo1', @sourceFieldName = N'video1'
	EXEC dbo.MoveProductSmallField @destFieldName = N'YouTubeVideo2', @sourceFieldName = N'video2'
	EXEC dbo.MoveProductSmallField @destFieldName = N'YouTubeVideo3', @sourceFieldName = N'video3'

	DELETE FROM [VitalChoice.Ecommerce].dbo.ProductOptionValues
	WHERE (Value IS NULL OR Value = N'') AND IdBigString IS NULL

	SET IDENTITY_INSERT [VitalChoice.Ecommerce].dbo.Skus ON;

	INSERT INTO [VitalChoice.Ecommerce].dbo.Skus
	(Id, Code, DateCreated, DateEdited, Hidden, IdProduct, [Order], Price, WholesalePrice, StatusCode)
	SELECT 
		a.idProduct, 
		a.sku, 
		ISNULL(a.pcProd_EditedDate, GETDATE()), 
		ISNULL(a.pcProd_EditedDate, GETDATE()), 
		ISNULL(a.hidden, 0), 
		p.Id, 
		ROW_NUMBER() OVER (ORDER BY a.idProduct), 
		a.price, 
		a.bToBPrice,
		CASE WHEN ISNULL(a.removed, 0) <> 0 THEN 3 ELSE CASE WHEN ISNULL(a.pcProd_SPInActive, 0) = 0 THEN 2 ELSE 1 END END
	FROM [vitalchoice2.0].dbo.products AS a
	INNER JOIN [VitalChoice.Ecommerce].dbo.Products AS p ON p.Id = a.pcprod_ParentPrd

	INSERT INTO [VitalChoice.Ecommerce].dbo.Skus
	(Id, Code, DateCreated, DateEdited, Hidden, IdProduct, [Order], Price, WholesalePrice, StatusCode)
	SELECT 
		a.idProduct, 
		LEFT(a.sku, 20), 
		ISNULL(a.pcProd_EditedDate, GETDATE()), 
		ISNULL(a.pcProd_EditedDate, GETDATE()), 
		ISNULL(a.hidden, 0), 
		p.Id, 
		ROW_NUMBER() OVER (ORDER BY a.idProduct), 
		a.price, 
		a.bToBPrice,
		p.StatusCode
	FROM [vitalchoice2.0].dbo.products AS a
	INNER JOIN [VitalChoice.Ecommerce].dbo.Products AS p ON p.Id = a.idProduct AND ISNULL(a.pcprod_Apparel, 0) = 0

	SET IDENTITY_INSERT [VitalChoice.Ecommerce].dbo.Skus OFF;

	INSERT INTO [VitalChoice.Ecommerce].dbo.SkuOptionValues
	(IdOptionType, IdSku, Value)
	SELECT t.Id, pp.idProduct,
	CAST(CASE
		WHEN dbo.IsMatch('^([0-9]+)\s*([dD]ozen|[dD]z)$', o.optionDescrip) = 1 
		THEN CAST(dbo.RegexReplace('^([0-9]+)\s*([dD]ozen|[dD]z)$', o.optionDescrip, '$1') AS INT) * 12
		WHEN dbo.IsMatch('^([0-9]+)-[0-9]+$', o.optionDescrip) = 1
		THEN CAST(dbo.RegexReplace('^([0-9]+)-[0-9]+$', o.optionDescrip, '$1') AS INT)
		ELSE
		CAST(CAST(dbo.RegexReplace('[^0-9\.]', o.optionDescrip, '') AS FLOAT) AS INT) END AS NVARCHAR(250))
	FROM products AS p
	INNER JOIN [vitalchoice2.0].[dbo].[pcProductsOptions] AS po ON po.idProduct = p.idProduct
	INNER JOIN options_optionsGroups AS og ON og.idOptionGroup = po.idOptionGroup AND og.idProduct = p.idProduct
	INNER JOIN options AS o ON o.idOption = og.idOption
	INNER JOIN products AS pp ON (pp.pcProd_Relationship like '%[_]'+ CAST(og.idoptoptgrp AS NVARCHAR(10)) OR (pp.pcProd_Relationship like '%[_]'+CAST(og.idoptoptgrp AS NVARCHAR(10))+'[_]%'))
	INNER JOIN [VitalChoice.Ecommerce].dbo.Products AS ep ON ep.Id = p.idProduct
	INNER JOIN [VitalChoice.Ecommerce].dbo.ProductOptionTypes AS t ON (t.IdObjectType = ep.IdObjectType OR t.IdObjectType IS NULL) AND t.Name = N'QTY'

	EXEC dbo.MoveSkuField @destFieldName = N'DisregardStock', @sourceFieldName = N'nostock', @conversion = N'CASE WHEN ISNULL(a.nostock, 0) <> 0 THEN ''True'' ELSE ''False'' END'
	EXEC dbo.MoveSkuField @destFieldName = N'Stock', @sourceFieldName = N'stock', @conversion = N'CAST(ISNULL(a.stock, 0) AS NVARCHAR(250))'
	EXEC dbo.MoveSkuField @destFieldName = N'DisallowSingle', @sourceFieldName = N'disallowSingle', @conversion = N'CASE WHEN ISNULL(a.disallowSingle, 0) <> 0 THEN ''True'' ELSE ''False'' END'
	EXEC dbo.MoveSkuField @destFieldName = N'NonDiscountable', @sourceFieldName = N'NonDiscountable', @conversion = N'CASE WHEN ISNULL(a.NonDiscountable, 0) <> 0 THEN ''True'' ELSE ''False'' END'
	EXEC dbo.MoveSkuField @destFieldName = N'OrphanType', @sourceFieldName = N'OrphanType', @conversion = N'CASE WHEN ISNULL(a.OrphanType, 0) <> 0 THEN ''True'' ELSE ''False'' END'
	EXEC dbo.MoveSkuField @destFieldName = N'QTYThreshold', @sourceFieldName = N'QTYThreshold', @conversion = N'CASE WHEN ISNULL(a.OrphanType, 0) <> 0 THEN a.QTYThreshold ELSE NULL END'
	EXEC dbo.MoveSkuField @destFieldName = N'AutoShipProduct', @sourceFieldName = N'autoShip', @conversion = N'CASE WHEN ISNULL(a.autoShip, 0) <> 0 THEN ''True'' ELSE ''False'' END'
	EXEC dbo.MoveSkuField @destFieldName = N'AutoShipFrequency1', @sourceFieldName = N'Schedules', @conversion = N'CASE WHEN ISNULL(a.autoShip, 0) <> 0 THEN CASE WHEN a.Schedules LIKE ''%1%'' THEN ''True'' ELSE ''False'' END ELSE NULL END'
	EXEC dbo.MoveSkuField @destFieldName = N'AutoShipFrequency2', @sourceFieldName = N'Schedules', @conversion = N'CASE WHEN ISNULL(a.autoShip, 0) <> 0 THEN CASE WHEN a.Schedules LIKE ''%2%'' THEN ''True'' ELSE ''False'' END ELSE NULL END'
	EXEC dbo.MoveSkuField @destFieldName = N'AutoShipFrequency3', @sourceFieldName = N'Schedules', @conversion = N'CASE WHEN ISNULL(a.autoShip, 0) <> 0 THEN CASE WHEN a.Schedules LIKE ''%3%'' THEN ''True'' ELSE ''False'' END ELSE NULL END'
	EXEC dbo.MoveSkuField @destFieldName = N'AutoShipFrequency6', @sourceFieldName = N'Schedules', @conversion = N'CASE WHEN ISNULL(a.autoShip, 0) <> 0 THEN CASE WHEN a.Schedules LIKE ''%6%'' THEN ''True'' ELSE ''False'' END ELSE NULL END'
	EXEC dbo.MoveSkuField @destFieldName = N'OffPercent', @sourceFieldName = N'autoShipDiscount', @conversion = N'CASE WHEN ISNULL(a.autoShip, 0) <> 0 THEN CAST(a.autoShipDiscount AS NVARCHAR(250)) ELSE NULL END'
	EXEC dbo.MoveSkuField @destFieldName = N'Seller', @sourceFieldName = N'sellergoogle', @conversion = N'(SELECT v.Id FROM [VitalChoice.Ecommerce].[dbo].[Lookups] AS l INNER JOIN [VitalChoice.Ecommerce].dbo.LookupVariants AS v ON v.IdLookup = l.Id WHERE l.Name = N''ProductSellers'' AND v.ValueVariant = a.sellergoogle COLLATE Cyrillic_General_CI_AS)'
	EXEC dbo.MoveSkuField @destFieldName = N'HideFromDataFeed', @sourceFieldName = N'excludegoogle', @conversion = N'CASE WHEN ISNULL(a.excludegoogle, 0) <> 0 THEN ''True'' ELSE ''False'' END'

	DELETE FROM [VitalChoice.Ecommerce].dbo.SkuOptionValues
	WHERE Value IS NULL OR Value = N''

	INSERT [VitalChoice.Infrastructure].dbo.RecipesToProducts
	(IdProduct, IdRecipe)
	SELECT rp.idProduct, r.Id FROM [vitalchoice2.0].dbo.recipes_products AS rp
	INNER JOIN [VitalChoice.Infrastructure].dbo.Recipes AS r ON r.IdOld = rp.idRecipe
	INNER JOIN [VitalChoice.Infrastructure].dbo.Products AS p ON p.Id = rp.idProduct
--	COMMIT
--END TRY
--BEGIN CATCH
--	ROLLBACK
--	SELECT 
--        ERROR_NUMBER() AS ErrorNumber,
--        ERROR_MESSAGE() AS ErrorMessage,
--		ERROR_LINE() AS Line
--END CATCH

ALTER TABLE [VitalChoice.Infrastructure].dbo.ContentItems
	DROP COLUMN TempId, COLUMN TempCategoryId

ALTER TABLE [VitalChoice.Ecommerce].dbo.BigStringValues
ADD IdProduct INT

GO

DECLARE @fieldType INT, @textId BIGINT
DECLARE @textData NVARCHAR(MAX), @IdProduct INT, @idObjectType INT
DECLARE @bigId TABLE(Id BIGINT NOT NULL, IdProduct INT NOT NULL)

SET @textData = N'Frozen foods are packed with dry ice<br />
and ship Monday through Wednesday (except holidays).<br />
 <br />
Frozen foods ship via 2nd Day Air service or 1-3 Day Express Ground service, depending on shipping date and destination.<br />
<a href="/shop/pc/viewContent.asp?idpage=16"><img src="/files/catalog/image001.jpg" border="0" style="margin-top:3px;"/></a>
<br />
<u>Standard Shipping Fees</u>
<ul>
<li>Up to $49.99 = $4.95 shipping</li>
<li>$50 - $98.99 = $9.95 shipping</li>
<li>$99 and over = FREE shipping</li>
</ul>
Overnight shipping and other premium services cost extra; for more information, see our <a href="/shop/pc/viewContent.asp?idpage=16">Shipping page</a>. (Live shellfish ship overnight at no extra cost.)
<ul> 
<li>We cannot ship perishable goods to PO boxes.</li>
<li>Free standard shipping applies to all orders of $99 or more sent to any U.S. street address.</li>
<li>Perishable shipments to Alaska and Hawaii are subject to surcharges; please see our <a href="/shop/pc/viewContent.asp?idpage=16">Shipping page</a>.</li>
</ul> 
<u>When will my frozen foods arrive?</u>
<ul>
<li>Orders placed Saturday, Sunday, Monday, or before 10 AM Tuesday will ship on Tuesday* to arrive 1-3 days later.</li>
<li>Orders placed between 10 AM Tuesday and 10 AM Wednesday will ship on Wednesday* to arrive 1-2 days later.</li>
<li>Orders placed after 10 AM on Wednesday through 12:00 midnight Friday will ship on Monday* to arrive 1-3 days later.</li>
</ul> 
*Except when a holiday falls on the designated shipping day.<br />
 <br />
For more information, see our <a href="/shop/pc/viewContent.asp?idpage=16">Shipping page</a>.'

SELECT TOP 1 @fieldType = Id FROM [VitalChoice.Ecommerce].dbo.ProductOptionTypes WHERE Name = N'Shipping' AND IdObjectType = 2

DELETE FROM [VitalChoice.Ecommerce].dbo.BigStringValues
WHERE IdBigString IN (SELECT v.IdBigString FROM [VitalChoice.Ecommerce].dbo.ProductOptionValues AS v WHERE IdOptionType = @fieldType)

DELETE FROM [VitalChoice.Ecommerce].dbo.ProductOptionValues
WHERE IdOptionType = @fieldType

INSERT INTO [VitalChoice.Ecommerce].dbo.BigStringValues
(Value, IdProduct)
OUTPUT inserted.IdBigString, inserted.IdProduct INTO @bigId
SELECT @textData, Id FROM [VitalChoice.Ecommerce].dbo.Products
WHERE IdObjectType = 2

INSERT INTO [VitalChoice.Ecommerce].dbo.ProductOptionValues
(IdProduct, IdOptionType, IdBigString)
SELECT IdProduct, @fieldType, Id FROM @bigId

DELETE FROM @bigId

SET @textData = N'Non-perishable goods ship via Ground service Monday - Friday (except
holidays) and normally arrive in 3-7 days, depending upon destination.<br />
 <br />
Non-perishable shipments to Washington State, Oregon, Idaho, and<br />
Northern California may arrive faster, in 1 or 2 days.<br />
<a href="/shop/pc/viewContent.asp?idpage=16"><img src="/files/catalog/image001.jpg" border="0" /></a>
<br />
Standard Shipping Fees
<ul>
<li>Up to $49.99 = $4.95 shipping</li>
<li>$50 - $98.99 = $9.95 shipping</li>
<li>$99 and over = FREE shipping</li>
</ul>
Free standard shipping applies to orders of $99 or more per U.S.<br />
address, including non-perishables sent to Alaska and Hawaii.<br />
 <br />
Overnight shipping and other premium services cost more; for<br />
more information on this and all shipping topics, see our <a href="/shop/pc/viewContent.asp?idpage=16">Shipping page</a>.'

SELECT TOP 1 @fieldType = Id FROM [VitalChoice.Ecommerce].dbo.ProductOptionTypes WHERE Name = N'Shipping' AND IdObjectType = 1

DELETE FROM [VitalChoice.Ecommerce].dbo.BigStringValues
WHERE IdBigString IN (SELECT v.IdBigString FROM [VitalChoice.Ecommerce].dbo.ProductOptionValues AS v WHERE IdOptionType = @fieldType)

DELETE FROM [VitalChoice.Ecommerce].dbo.ProductOptionValues
WHERE IdOptionType = @fieldType

INSERT INTO [VitalChoice.Ecommerce].dbo.BigStringValues
(Value, IdProduct)
OUTPUT inserted.IdBigString, inserted.IdProduct INTO @bigId
SELECT @textData, Id FROM [VitalChoice.Ecommerce].dbo.Products
WHERE IdObjectType = 1

INSERT INTO [VitalChoice.Ecommerce].dbo.ProductOptionValues
(IdProduct, IdOptionType, IdBigString)
SELECT IdProduct, @fieldType, Id FROM @bigId

GO

ALTER TABLE [VitalChoice.Ecommerce].dbo.BigStringValues
DROP COLUMN IdProduct

GO

INSERT INTO [VitalChoice.Ecommerce].dbo.ProductOptionValues
(IdOptionType, IdProduct, Value)
SELECT t.Id, p.Id, pp.GoogleFeedTitle FROM [VitalChoice.Ecommerce].dbo.Products AS p
INNER JOIN [vitalchoice2.0].dbo.products AS pp ON pp.idProduct = p.Id
INNER JOIN [VitalChoice.Ecommerce].dbo.ProductOptionTypes AS t ON t.IdObjectType = p.IdObjectType AND t.Name = 'GoogleFeedTitle' 

INSERT INTO [VitalChoice.Ecommerce].dbo.ProductOptionValues
(IdOptionType, IdProduct, Value)
SELECT t.Id, p.Id, LEFT(pp.GoogleFeedDescription, 250) FROM [VitalChoice.Ecommerce].dbo.Products AS p
INNER JOIN [vitalchoice2.0].dbo.products AS pp ON pp.idProduct = p.Id
INNER JOIN [VitalChoice.Ecommerce].dbo.ProductOptionTypes AS t ON t.IdObjectType = p.IdObjectType AND t.Name = 'GoogleFeedDescription' 