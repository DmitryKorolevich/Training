USE [vitalchoice2.0]
GO

IF OBJECT_ID('dbo.ReplaceUrl') IS NOT NULL
BEGIN
DROP FUNCTION dbo.ReplaceUrl
END
GO
CREATE FUNCTION dbo.ReplaceUrl
(@text NVARCHAR(MAX))
RETURNS NVARCHAR(MAX)
AS
BEGIN
	RETURN REPLACE(REPLACE(REPLACE(ISNULL(@text, N''), 'http://www.vitalchoice.com/shop/pc/catalog/', '/files/catalog/'), '/shop/pc/catalog/', '/files/catalog/'), '../pc/catalog/', '/files/catalog/')
END
GO
--ALTER TABLE AZURE.[VitalChoice.Infrastructure].dbo.ContentItems
--ADD TempId INT NULL,
--	TempCategoryId INT NULL

--TRUNCATE TABLE AZURE.[VitalChoice.Infrastructure].dbo.RecipesToContentCategories
--TRUNCATE TABLE AZURE.[VitalChoice.Infrastructure].dbo.RecipesToProducts
--TRUNCATE TABLE AZURE.[VitalChoice.Infrastructure].dbo.RecipeCrossSells
--TRUNCATE TABLE AZURE.[VitalChoice.Infrastructure].dbo.RelatedRecipes

GO

DECLARE @contentType INT, @oldContentType INT, @masterName NVARCHAR(50), @categoryMasterName NVARCHAR(50)
SET @contentType = 1
SET @oldContentType = 1
SET @masterName = N'Recipe Individual'
SET @categoryMasterName = N'Recipe Sub Category'

--BEGIN TRANSACTION
--BEGIN TRY
	--============================= Clean ===========================
	DECLARE @contentItemsToDelete TABLE(Id INT)

	INSERT INTO @contentItemsToDelete
	(Id)
	SELECT ContentItemId FROM AZURE.[VitalChoice.Infrastructure].dbo.Recipes

	DELETE FROM AZURE.[VitalChoice.Infrastructure].dbo.Recipes
	--OUTPUT deleted.ContentItemId INTO @contentItemsToDelete

	INSERT INTO @contentItemsToDelete
	(Id)
	SELECT ContentItemId FROM AZURE.[VitalChoice.Infrastructure].dbo.ContentCategories
	WHERE Type = @contentType AND Url <> 'root'

	DELETE FROM AZURE.[VitalChoice.Infrastructure].dbo.ContentCategories
	--OUTPUT deleted.ContentItemId INTO @contentItemsToDelete
	WHERE Type = @contentType AND Url <> 'root'

	DELETE FROM AZURE.[VitalChoice.Infrastructure].dbo.ContentItems
	WHERE Id IN (SELECT Id FROM @contentItemsToDelete)

	DECLARE @masterId INT
	SELECT @masterId = Id FROM AZURE.[VitalChoice.Infrastructure].dbo.MasterContentItems WHERE Name = @masterName

	--DECLARE @insertedArticles TABLE (Id INT NOT NULL PRIMARY KEY, TempId INT NOT NULL)

	--============================= Recipes ===========================

	INSERT AZURE.[VitalChoice.Infrastructure].dbo.ContentItems
	(Created, Description, MetaDescription, MetaKeywords, Title, Updated, TempId, Template)
	--OUTPUT inserted.Id, inserted.TempId INTO @insertedArticles
	SELECT GETDATE(), dbo.ReplaceUrl(RecipeDescription), LEFT(MetaDescription, 250), NULL, MetaTitle, GETDATE(), RecipeId, N''
	FROM [vitalchoice2.0].[dbo].Recipes AS a
	ORDER BY a.RecipeId

	INSERT INTO AZURE.[VitalChoice.Infrastructure].dbo.Recipes
	(AboutChef, ContentItemId, MasterContentItemId, Name, Directions, Ingredients, YoutubeVideo, YoutubeImage, StatusCode, SubTitle, Url, FileUrl, IdOld)
	SELECT a.About, i.Id, @masterId, a.RecipeTitle, a.Directions, a.Ingredients, ISNULL(a.VideoUrl, N''), dbo.ReplaceUrl(a.VideoImage), 2/*Active*/, ISNULL(a.SubTitle, N''), REPLACE(RTRIM(LTRIM(LOWER([vitalchoice2.0].[dbo].RegexReplace('[^a-zA-Z0-9]+', a.RecipeTitle, ' ')))) COLLATE SQL_Latin1_General_CP1_CI_AS,' ','-'), N'/files/catalog/recipe-images/' + a.RecipeImage, a.RecipeId 
	FROM [vitalchoice2.0].[dbo].Recipes AS a
	INNER JOIN AZURE.[VitalChoice.Infrastructure].dbo.ContentItems AS i ON i.TempId = a.RecipeId

	DECLARE @subMaster INT
	SELECT @subMaster = Id FROM AZURE.[VitalChoice.Infrastructure].dbo.MasterContentItems WHERE Name = @categoryMasterName

	--DECLARE @insertedArticleCategories TABLE (Id INT NOT NULL PRIMARY KEY, TempCategoryId INT NOT NULL)

	--============================= Recipe Categories ===========================

	INSERT AZURE.[VitalChoice.Infrastructure].dbo.ContentItems
	(Created, Description, MetaDescription, MetaKeywords, Title, Updated, TempCategoryId, Template)
	--OUTPUT inserted.Id, inserted.TempCategoryId INTO @insertedArticleCategories
	SELECT ISNULL(ca.pcCats_EditedDate, GETDATE()), N'', ca.pcCats_MetaDesc, ca.pcCats_MetaKeywords, ca.pcCats_MetaTitle, GETDATE(), ca.idCategory, N''
	FROM [vitalchoice2.0].dbo.categories AS ca
	WHERE ca.type=@oldContentType AND ca.idParentCategory <> 1
	ORDER BY ca.idParentCategory

	UPDATE AZURE.[VitalChoice.Infrastructure].dbo.ContentCategories
	SET IdOld = (SELECT TOP 1 c.idCategory FROM [vitalchoice2.0].dbo.categories AS c WHERE c.type = @oldContentType AND c.idParentCategory = 1)
	WHERE Type = @contentType AND Url = 'root'

	INSERT AZURE.[VitalChoice.Infrastructure].dbo.ContentCategories
	(ContentItemId, MasterContentItemId, IdOld, Name, StatusCode, Type, Url, [Order])
	SELECT c.Id, @subMaster, ca.idCategory, ca.categoryDesc,  
		2/*Active*/, @contentType, REPLACE(RTRIM(LTRIM(LOWER([vitalchoice2.0].[dbo].RegexReplace('[^a-zA-Z0-9]+', ca.categoryDesc, ' ')))) COLLATE SQL_Latin1_General_CP1_CI_AS,' ','-'),
		ROW_NUMBER() OVER (ORDER BY c.Id)
	FROM [vitalchoice2.0].dbo.categories AS ca
	INNER JOIN AZURE.[VitalChoice.Infrastructure].dbo.ContentItems AS c ON c.TempCategoryId = ca.idCategory
	WHERE ca.type=@oldContentType AND ca.idParentCategory <> 1
	ORDER BY ca.idParentCategory

	UPDATE AZURE.[VitalChoice.Infrastructure].dbo.ContentCategories
	SET ParentId = (SELECT TOP 1 cc.Id FROM AZURE.[VitalChoice.Infrastructure].dbo.ContentCategories AS cc WHERE cc.IdOld = ca.idParentCategory)
	FROM AZURE.[VitalChoice.Infrastructure].dbo.ContentCategories AS cc
	INNER JOIN [vitalchoice2.0].dbo.categories AS ca ON ca.idCategory = cc.IdOld
	WHERE ca.type=@oldContentType AND ca.idParentCategory <> 1

	INSERT AZURE.[VitalChoice.Infrastructure].dbo.RecipesToContentCategories
	(RecipeId, ContentCategoryId)
	SELECT a.Id, c.Id FROM AZURE.[VitalChoice.Infrastructure].dbo.Recipes AS a
	INNER JOIN [vitalchoice2.0].dbo.categories_recipes AS ca ON ca.idRecipe = a.IdOld
	INNER JOIN AZURE.[VitalChoice.Infrastructure].dbo.ContentCategories AS c ON c.IdOld = ca.idCategory

	--============================= Recipe Cross Sells ===========================

	INSERT AZURE.[VitalChoice.Infrastructure].dbo.RecipeCrossSells
	(IdRecipe, Image, Title, Subtitle, Url, Number)
	SELECT Id, dbo.ReplaceUrl(rold.crosssell1Image), rold.crosssell1Title, rold.crosssell1SubTitle, rold.crosssell1Url, 1 
	FROM AZURE.[VitalChoice.Infrastructure].dbo.Recipes AS r
	INNER JOIN [vitalchoice2.0].dbo.Recipes AS rold ON rold.RecipeId = r.IdOld
	WHERE rold.crosssell1Image IS NOT NULL AND rold.crosssell1Image <> ''

	INSERT AZURE.[VitalChoice.Infrastructure].dbo.RecipeCrossSells
	(IdRecipe, Image, Title, Subtitle, Url, Number)
	SELECT Id, dbo.ReplaceUrl(rold.crosssell2Image), rold.crosssell2Title, rold.crosssell2SubTitle, rold.crosssell2Url, 2
	FROM AZURE.[VitalChoice.Infrastructure].dbo.Recipes AS r
	INNER JOIN [vitalchoice2.0].dbo.Recipes AS rold ON rold.RecipeId = r.IdOld
	WHERE rold.crosssell2Image IS NOT NULL AND rold.crosssell2Image <> ''

	INSERT AZURE.[VitalChoice.Infrastructure].dbo.RecipeCrossSells
	(IdRecipe, Image, Title, Subtitle, Url, Number)
	SELECT Id, dbo.ReplaceUrl(rold.crosssell3Image), rold.crosssell3Title, rold.crosssell3SubTitle, rold.crosssell3Url, 3
	FROM AZURE.[VitalChoice.Infrastructure].dbo.Recipes AS r
	INNER JOIN [vitalchoice2.0].dbo.Recipes AS rold ON rold.RecipeId = r.IdOld
	WHERE rold.crosssell3Image IS NOT NULL AND rold.crosssell3Image <> ''

	--============================= Related Recipes ===========================

	INSERT AZURE.[VitalChoice.Infrastructure].dbo.RelatedRecipes
	(IdRecipe, Image, Title, Url, Number)
	SELECT Id, dbo.ReplaceUrl(rold.relatedrecipe1Image), rold.relatedrecipe1Title, rold.relatedrecipe1Url, 1 
	FROM AZURE.[VitalChoice.Infrastructure].dbo.Recipes AS r
	INNER JOIN [vitalchoice2.0].dbo.Recipes AS rold ON rold.RecipeId = r.IdOld
	WHERE rold.relatedrecipe1Image IS NOT NULL AND rold.relatedrecipe1Image <> ''

	INSERT AZURE.[VitalChoice.Infrastructure].dbo.RelatedRecipes
	(IdRecipe, Image, Title, Url, Number)
	SELECT Id, dbo.ReplaceUrl(rold.relatedrecipe2Image), rold.relatedrecipe2Title, rold.relatedrecipe2Url, 2
	FROM AZURE.[VitalChoice.Infrastructure].dbo.Recipes AS r
	INNER JOIN [vitalchoice2.0].dbo.Recipes AS rold ON rold.RecipeId = r.IdOld
	WHERE rold.relatedrecipe2Image IS NOT NULL AND rold.relatedrecipe2Image <> ''

	INSERT AZURE.[VitalChoice.Infrastructure].dbo.RelatedRecipes
	(IdRecipe, Image, Title, Url, Number)
	SELECT Id, dbo.ReplaceUrl(rold.relatedrecipe3Image), rold.relatedrecipe3Title, rold.relatedrecipe3Url, 3
	FROM AZURE.[VitalChoice.Infrastructure].dbo.Recipes AS r
	INNER JOIN [vitalchoice2.0].dbo.Recipes AS rold ON rold.RecipeId = r.IdOld
	WHERE rold.relatedrecipe3Image IS NOT NULL AND rold.relatedrecipe3Image <> ''

	INSERT AZURE.[VitalChoice.Infrastructure].dbo.RelatedRecipes
	(IdRecipe, Image, Title, Url, Number)
	SELECT Id, dbo.ReplaceUrl(rold.relatedrecipe4Image), rold.relatedrecipe4Title, rold.relatedrecipe4Url, 4
	FROM AZURE.[VitalChoice.Infrastructure].dbo.Recipes AS r
	INNER JOIN [vitalchoice2.0].dbo.Recipes AS rold ON rold.RecipeId = r.IdOld
	WHERE rold.relatedrecipe4Image IS NOT NULL AND rold.relatedrecipe4Image <> ''

--	COMMIT
--END TRY
--BEGIN CATCH
--	ROLLBACK
--	SELECT 
--        ERROR_NUMBER() AS ErrorNumber,
--        ERROR_MESSAGE() AS ErrorMessage;
--END CATCH

GO
--ALTER TABLE AZURE.[VitalChoice.Infrastructure].dbo.ContentItems
--DROP COLUMN TempId, COLUMN TempCategoryId

GO

--SELECT ROW_NUMBER() OVER (PARTITION BY r.Url ORDER BY r.Id), r.Id
--FROM AZURE.[VitalChoice.Infrastructure].dbo.Recipes AS r
--WHERE r.Url IN 
--(
--SELECT Url FROM AZURE.[VitalChoice.Infrastructure].dbo.Recipes
--	GROUP BY Url
--	HAVING COUNT(Url) > 1
--)

UPDATE AZURE.[VitalChoice.Infrastructure].dbo.Recipes
SET Url = r.Url + N'-' + CAST(j.Number AS NVARCHAR(10))
FROM AZURE.[VitalChoice.Infrastructure].dbo.Recipes AS r
INNER JOIN 
(
	SELECT ROW_NUMBER() OVER (PARTITION BY r.Url ORDER BY r.Id) AS Number, r.Id
	FROM AZURE.[VitalChoice.Infrastructure].dbo.Recipes AS r
	WHERE r.Url IN 
	(
		SELECT Url FROM AZURE.[VitalChoice.Infrastructure].dbo.Recipes
		GROUP BY Url
		HAVING COUNT(Url) > 1
	)
) AS j ON j.Id = r.Id
WHERE j.Number > 1

UPDATE AZURE.[VitalChoice.Infrastructure].dbo.ContentCategories
SET Url = r.Url + N'-' + CAST(j.Number AS NVARCHAR(10))
FROM AZURE.[VitalChoice.Infrastructure].dbo.ContentCategories AS r
INNER JOIN 
(
	SELECT ROW_NUMBER() OVER (PARTITION BY r.Url ORDER BY r.Id) AS Number, r.Id
	FROM AZURE.[VitalChoice.Infrastructure].dbo.ContentCategories AS r
	WHERE r.Url IN 
	(
		SELECT Url FROM AZURE.[VitalChoice.Infrastructure].dbo.ContentCategories
		WHERE Type = 1
		GROUP BY Url
		HAVING COUNT(Url) > 1
	) AND Type = 1
) AS j ON j.Id = r.Id
WHERE j.Number > 1 AND r.Type = 1

GO

SELECT Url FROM AZURE.[VitalChoice.Infrastructure].dbo.Recipes
GROUP BY Url
HAVING COUNT(Url) > 1

SELECT Url FROM AZURE.[VitalChoice.Infrastructure].dbo.ContentCategories
WHERE Type = 1
GROUP BY Url
HAVING COUNT(Url) > 1

GO
IF OBJECT_ID('dbo.ReplaceUrl') IS NOT NULL
BEGIN
DROP FUNCTION dbo.ReplaceUrl
END