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

--TRUNCATE TABLE AZURE.[VitalChoice.Infrastructure].dbo.FaqsToContentCategories
--TRUNCATE TABLE AZURE.[VitalChoice.Infrastructure].dbo.FaqsToProducts
--TRUNCATE TABLE AZURE.[VitalChoice.Infrastructure].dbo.RecipeCrossSells
--TRUNCATE TABLE AZURE.[VitalChoice.Infrastructure].dbo.RelatedRecipes
--TRUNCATE TABLE AZURE.[VitalChoice.Infrastructure].dbo.FAQsToContentCategories

GO

DECLARE @contentType INT, @masterName NVARCHAR(50), @categoryMasterName NVARCHAR(50)
SET @contentType = 5
SET @masterName = N'FAQ Individual'
SET @categoryMasterName = N'FAQ Sub Category'

--BEGIN TRANSACTION
--BEGIN TRY
	--============================= Clean ===========================
	DECLARE @contentItemsToDelete TABLE(Id INT)

	INSERT INTO @contentItemsToDelete
	(Id)
	SELECT ContentItemId FROM AZURE.[VitalChoice.Infrastructure].dbo.Faqs

	DELETE FROM AZURE.[VitalChoice.Infrastructure].dbo.Faqs
	--OUTPUT deleted.ContentItemId INTO @contentItemsToDelete

	INSERT INTO @contentItemsToDelete
	(Id)
	SELECT ContentItemId FROM AZURE.[VitalChoice.Infrastructure].dbo.ContentCategories
	WHERE Type = @contentType AND Url <> 'root' AND (ParentId <> 4 OR ParentId IS NULL)

	DELETE FROM AZURE.[VitalChoice.Infrastructure].dbo.ContentCategories
	--OUTPUT deleted.ContentItemId INTO @contentItemsToDelete
	WHERE Type = @contentType AND Url <> 'root' AND (ParentId <> 4 OR ParentId IS NULL)

	DELETE FROM AZURE.[VitalChoice.Infrastructure].dbo.ContentItems
	WHERE Id IN (SELECT Id FROM @contentItemsToDelete)

	DECLARE @masterId INT
	SELECT @masterId = Id FROM AZURE.[VitalChoice.Infrastructure].dbo.MasterContentItems WHERE Name = @masterName

	--DECLARE @insertedArticles TABLE (Id INT NOT NULL PRIMARY KEY, TempId INT NOT NULL)

	--============================= Faqs ===========================

	INSERT AZURE.[VitalChoice.Infrastructure].dbo.ContentItems
	(Created, Description, MetaDescription, MetaKeywords, Title, Updated, TempId, Template)
	--OUTPUT inserted.Id, inserted.TempId INTO @insertedArticles
	SELECT GETDATE(), dbo.ReplaceUrl(content), N'', NULL, title, GETDATE(), idFAQ, N''
	FROM [vitalchoice2.0].[dbo].FAQ AS a
	ORDER BY a.idFAQ

	INSERT INTO AZURE.[VitalChoice.Infrastructure].dbo.Faqs
	(ContentItemId, MasterContentItemId, Name, StatusCode, Url, IdOld)
	SELECT i.Id, @masterId, a.title, 2/*Active*/, REPLACE(RTRIM(LTRIM(LOWER([vitalchoice2.0].[dbo].RegexReplace('[^a-zA-Z0-9]+', a.title, ' ')))) COLLATE SQL_Latin1_General_CP1_CI_AS,' ','-'), a.idFAQ 
	FROM [vitalchoice2.0].[dbo].FAQ AS a
	INNER JOIN AZURE.[VitalChoice.Infrastructure].dbo.ContentItems AS i ON i.TempId = a.idFAQ

	DECLARE @subMaster INT
	SELECT @subMaster = Id FROM AZURE.[VitalChoice.Infrastructure].dbo.MasterContentItems WHERE Name = @categoryMasterName

	--DECLARE @insertedArticleCategories TABLE (Id INT NOT NULL PRIMARY KEY, TempCategoryId INT NOT NULL)

	--============================= Recipe Categories ===========================

	INSERT AZURE.[VitalChoice.Infrastructure].dbo.ContentItems
	(Created, Description, MetaDescription, MetaKeywords, Title, Updated, TempCategoryId, Template)
	--OUTPUT inserted.Id, inserted.TempCategoryId INTO @insertedArticleCategories
	SELECT GETDATE(), N'', N'', N'', ca.title, GETDATE(), ca.idSLC, N''
	FROM [vitalchoice2.0].dbo.FAQSLC AS ca
	WHERE ca.idSLC <> 1
	ORDER BY ca.idTLC

	--UPDATE AZURE.[VitalChoice.Infrastructure].dbo.ContentCategories
	--SET IdOld = (SELECT TOP 1 c.idSLC FROM [vitalchoice2.0].dbo.FAQSLC AS c)
	--WHERE Type = @contentType AND Url = 'root'

	INSERT AZURE.[VitalChoice.Infrastructure].dbo.ContentCategories
	(ContentItemId, MasterContentItemId, IdOld, Name, StatusCode, Type, Url, [Order], ParentId)
	SELECT c.Id, @subMaster, ca.idSLC, ca.title,  
		2/*Active*/, @contentType, REPLACE(RTRIM(LTRIM(LOWER([vitalchoice2.0].[dbo].RegexReplace('[^a-zA-Z0-9]+', ca.title, ' ')))) COLLATE SQL_Latin1_General_CP1_CI_AS,' ','-'),
		ROW_NUMBER() OVER (ORDER BY c.Id), 
		(
		SELECT TOP 1 cc.Id 
		FROM AZURE.[VitalChoice.Infrastructure].dbo.ContentCategories AS cc
		INNER JOIN [vitalchoice2.0].dbo.FAQTLC AS t ON t.idTLC = ca.idTLC 
		WHERE cc.Name COLLATE SQL_Latin1_General_CP1_CI_AS = t.Title1 AND cc.ParentId = 4
		)
	FROM [vitalchoice2.0].dbo.FAQSLC AS ca
	INNER JOIN AZURE.[VitalChoice.Infrastructure].dbo.ContentItems AS c ON c.TempCategoryId = ca.idSLC
	WHERE ca.idSLC <> 1
	ORDER BY ca.idTLC

	INSERT AZURE.[VitalChoice.Infrastructure].dbo.FaqsToContentCategories
	(FAQId, ContentCategoryId)
	SELECT a.Id, c.Id FROM AZURE.[VitalChoice.Infrastructure].dbo.Faqs AS a
	INNER JOIN [vitalchoice2.0].dbo.FAQtoSLC AS ca ON ca.idFAQ = a.IdOld
	INNER JOIN AZURE.[VitalChoice.Infrastructure].dbo.ContentCategories AS c ON c.IdOld = ca.idSLC

--	COMMIT
--END TRY
--BEGIN CATCH
--	ROLLBACK
--	SELECT 
--        ERROR_NUMBER() AS ErrorNumber,
--        ERROR_MESSAGE() AS ErrorMessage;
--END CATCH

GO

--SELECT ROW_NUMBER() OVER (PARTITION BY r.Url ORDER BY r.Id), r.Id
--FROM AZURE.[VitalChoice.Infrastructure].dbo.Faqs AS r
--WHERE r.Url IN 
--(
--SELECT Url FROM AZURE.[VitalChoice.Infrastructure].dbo.Faqs
--	GROUP BY Url
--	HAVING COUNT(Url) > 1
--)

UPDATE AZURE.[VitalChoice.Infrastructure].dbo.Faqs
SET Url = r.Url + N'-' + CAST(j.Number AS NVARCHAR(10))
FROM AZURE.[VitalChoice.Infrastructure].dbo.Faqs AS r
INNER JOIN 
(
	SELECT ROW_NUMBER() OVER (PARTITION BY r.Url ORDER BY r.Id) AS Number, r.Id
	FROM AZURE.[VitalChoice.Infrastructure].dbo.Faqs AS r
	WHERE r.Url IN 
	(
		SELECT Url FROM AZURE.[VitalChoice.Infrastructure].dbo.Faqs
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
		WHERE Type = 5
		GROUP BY Url
		HAVING COUNT(Url) > 1
	) AND Type = 5
) AS j ON j.Id = r.Id
WHERE j.Number > 1 AND r.Type = 5

GO

SELECT Url FROM AZURE.[VitalChoice.Infrastructure].dbo.Faqs
GROUP BY Url
HAVING COUNT(Url) > 1

SELECT Url FROM AZURE.[VitalChoice.Infrastructure].dbo.ContentCategories
WHERE Type = 5
GROUP BY Url
HAVING COUNT(Url) > 1

GO
IF OBJECT_ID('dbo.ReplaceUrl') IS NOT NULL
BEGIN
DROP FUNCTION dbo.ReplaceUrl
END

GO
--ALTER TABLE AZURE.[VitalChoice.Infrastructure].dbo.ContentItems
--DROP COLUMN TempId, COLUMN TempCategoryId