USE [vitalchoice2.0]
GO

ALTER TABLE [VitalChoice.Infrastructure].dbo.ContentItems
ADD TempId INT NULL,
	TempCategoryId INT NULL

TRUNCATE TABLE [VitalChoice.Infrastructure].dbo.ArticlesToContentCategories

GO

--BEGIN TRANSACTION
--BEGIN TRY

	DECLARE @contentItemsToDelete TABLE(Id INT)

	INSERT INTO @contentItemsToDelete
	(Id)
	SELECT ContentItemId FROM [VitalChoice.Infrastructure].dbo.Articles
	DELETE FROM [VitalChoice.Infrastructure].dbo.Articles
	--OUTPUT deleted.ContentItemId INTO @contentItemsToDelete

	INSERT INTO @contentItemsToDelete
	(Id)
	SELECT ContentItemId FROM [VitalChoice.Infrastructure].dbo.ContentCategories
	WHERE Type = 3 AND Url <> 'root'

	DELETE FROM [VitalChoice.Infrastructure].dbo.ContentCategories
	--OUTPUT deleted.ContentItemId INTO @contentItemsToDelete
	WHERE Type = 3 AND Url <> 'root'

	DELETE FROM [VitalChoice.Infrastructure].dbo.ContentItems
	WHERE Id IN (SELECT Id FROM @contentItemsToDelete)

	DECLARE @articleMasterId INT
	SELECT @articleMasterId = Id FROM [VitalChoice.Infrastructure].dbo.MasterContentItems WHERE Name = 'Article Individual'

	--DECLARE @insertedArticles TABLE (Id INT NOT NULL PRIMARY KEY, TempId INT NOT NULL)

	INSERT [VitalChoice.Infrastructure].dbo.ContentItems
	(Created, Description, MetaDescription, MetaKeywords, Title, Updated, TempId, Template)
	--OUTPUT inserted.Id, inserted.TempId INTO @insertedArticles
	SELECT CAST(ArticlesDate AS DATE), ISNULL(ArticlesDescription, N''), MetaDescription, NULL, MetaTitle, GETDATE(), Id, N''
	FROM [vitalchoice2.0].[dbo].Articles AS a
	ORDER BY a.ID

	INSERT INTO [VitalChoice.Infrastructure].dbo.Articles
	(Author, ContentItemId, MasterContentItemId, Name, PublishedDate, StatusCode, SubTitle, Url, FileUrl, IdOld)
	SELECT a.ArticlesAuthor, i.Id, @articleMasterId, a.ArticlesTitle, CAST(a.ArticlesDate AS DATE), 2/*Active*/, a.ArticlesSubTitle, REPLACE(RTRIM(LTRIM(LOWER([vitalchoice2.0].[dbo].RegexReplace('[^a-zA-Z0-9]+', a.ArticlesTitle, ' ')))) COLLATE SQL_Latin1_General_CP1_CI_AS,' ','-'), a.articlesImage, a.ID 
	FROM [vitalchoice2.0].[dbo].Articles AS a
	INNER JOIN [VitalChoice.Infrastructure].dbo.ContentItems AS i ON i.TempId = a.ID

	DECLARE @articleSubMaster INT
	SELECT @articleSubMaster = Id FROM [VitalChoice.Infrastructure].dbo.MasterContentItems WHERE Name = 'Article Sub Category'

	--DECLARE @insertedArticleCategories TABLE (Id INT NOT NULL PRIMARY KEY, TempCategoryId INT NOT NULL)

	UPDATE [VitalChoice.Infrastructure].dbo.ContentItems
	SET MetaDescription = ca.pcCats_MetaDesc,
		MetaKeywords = ca.pcCats_MetaKeywords
	FROM [VitalChoice.Infrastructure].dbo.ContentItems AS i
	INNER JOIN [VitalChoice.Infrastructure].dbo.ContentCategories AS c ON c.ContentItemId = i.Id AND Url = 'root' AND c.Type=3
	INNER JOIN [vitalchoice2.0].dbo.categories AS ca ON ca.type = 2 AND ca.idParentCategory = 1

	INSERT [VitalChoice.Infrastructure].dbo.ContentItems
	(Created, Description, MetaDescription, MetaKeywords, Title, Updated, TempCategoryId, Template)
	--OUTPUT inserted.Id, inserted.TempCategoryId INTO @insertedArticleCategories
	SELECT ISNULL(ca.pcCats_EditedDate, GETDATE()), N'', ca.pcCats_MetaDesc, ca.pcCats_MetaKeywords, ca.pcCats_MetaTitle, GETDATE(), ca.idCategory, N''
	FROM [vitalchoice2.0].dbo.categories AS ca
	WHERE ca.type=2 AND ca.idParentCategory <> 1
	ORDER BY ca.idParentCategory

	UPDATE [VitalChoice.Infrastructure].dbo.ContentCategories
	SET IdOld = (SELECT TOP 1 c.idCategory FROM [vitalchoice2.0].dbo.categories AS c WHERE c.type = 2 AND c.idParentCategory = 1)
	WHERE Type = 3 AND Url = 'root'

	INSERT [VitalChoice.Infrastructure].dbo.ContentCategories
	(ContentItemId, MasterContentItemId, IdOld, Name, StatusCode, Type, Url, [Order])
	SELECT c.Id, @articleSubMaster, ca.idCategory, ca.categoryDesc,  
		2/*Active*/, 3, REPLACE(RTRIM(LTRIM(LOWER([vitalchoice2.0].[dbo].RegexReplace('[^a-zA-Z0-9]+', ca.categoryDesc, ' ')))) COLLATE SQL_Latin1_General_CP1_CI_AS,' ','-'),
		ROW_NUMBER() OVER (ORDER BY c.Id)
	FROM [vitalchoice2.0].dbo.categories AS ca
	INNER JOIN [VitalChoice.Infrastructure].dbo.ContentItems AS c ON c.TempCategoryId = ca.idCategory
	WHERE ca.type=2 AND ca.idParentCategory <> 1
	ORDER BY ca.idParentCategory

	UPDATE [VitalChoice.Infrastructure].dbo.ContentCategories
	SET ParentId = (SELECT TOP 1 cc.Id FROM [VitalChoice.Infrastructure].dbo.ContentCategories AS cc WHERE cc.IdOld = ca.idParentCategory)
	FROM [VitalChoice.Infrastructure].dbo.ContentCategories AS cc
	INNER JOIN [vitalchoice2.0].dbo.categories AS ca ON ca.idCategory = cc.IdOld
	WHERE ca.type=2 AND ca.idParentCategory <> 1

	INSERT [VitalChoice.Infrastructure].dbo.ArticlesToContentCategories
	(ArticleId, ContentCategoryId)
	SELECT a.Id, c.Id FROM [VitalChoice.Infrastructure].dbo.Articles AS a
	INNER JOIN [vitalchoice2.0].dbo.categories_articles AS ca ON ca.idArticle = a.IdOld
	INNER JOIN [VitalChoice.Infrastructure].dbo.ContentCategories AS c ON c.IdOld = ca.idCategory

--	COMMIT
--END TRY
--BEGIN CATCH
--	ROLLBACK
--	SELECT 
--        ERROR_NUMBER() AS ErrorNumber,
--        ERROR_MESSAGE() AS ErrorMessage;
--END CATCH

GO
ALTER TABLE [VitalChoice.Infrastructure].dbo.ContentItems
DROP COLUMN TempId, COLUMN TempCategoryId

GO

SELECT Url FROM [VitalChoice.Infrastructure].dbo.Articles
	GROUP BY Url
	HAVING COUNT(Url) > 1