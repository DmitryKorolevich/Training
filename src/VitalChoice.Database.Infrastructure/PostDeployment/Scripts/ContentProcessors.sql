IF((SELECT count(*) FROM ContentProcessors
WHERE Id IN (1,2,3))!=3)
BEGIN

INSERT INTO ContentProcessors
(Id, Type, Name, Description)
SELECT 1, 'RecipeRootCategoryProcessor', 'Recipe root category processor', NULL

INSERT INTO ContentProcessors
(Id, Type, Name, Description)
SELECT 2, 'RecipeSubCategoriesProcessor', 'Recipe sub-categories processor', NULL

INSERT INTO ContentProcessors
(Id, Type, Name, Description)
SELECT 3, 'RecipesProcessor', 'Recipes processor', NULL

END

IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentProcessors] WHERE [Type] = N'ProductCategoryProcessor')
BEGIN
	INSERT INTO [dbo].[ContentProcessors]
	(Id, [Type], Name, Description)
	VALUES
	(4, N'ProductCategoryProcessor', N'Product category processor', N'Processor to manage product categories')

	INSERT INTO [dbo].[MasterContentItemsToContentProcessors]
	([MasterContentItemId],[ContentProcessorId])
	SELECT [Id], 4 FROM [dbo].[MasterContentItems] WHERE [Name] = N'Product sub categories'
END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentProcessors] WHERE [Type] = N'ArticleCategoriesProcessor')
BEGIN
	INSERT INTO [dbo].[ContentProcessors]
	(Id, [Type], Name, Description)
	VALUES
	(7, N'ArticleCategoriesProcessor', N'Article categories processor', N'Tree view of article categories')
	INSERT INTO [dbo].[ContentProcessors]
	(Id, [Type], Name, Description)
	VALUES
	(8, N'ArticlesProcessor', N'Articles processor', N'Articles with paging by category id')

	INSERT INTO [dbo].[MasterContentItemsToContentProcessors]
	([MasterContentItemId],[ContentProcessorId])
	SELECT [Id], 7 FROM [dbo].[MasterContentItems] WHERE [Name] = N'Article sub categories'
	INSERT INTO [dbo].[MasterContentItemsToContentProcessors]
	([MasterContentItemId],[ContentProcessorId])
	SELECT [Id], 8 FROM [dbo].[MasterContentItems] WHERE [Name] = N'Article sub categories'
END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentProcessors] WHERE [Type] = N'RecentArticlesProcessor')
BEGIN
	INSERT INTO [dbo].[ContentProcessors]
	(Id, [Type], Name, Description)
	VALUES
	(9, N'RecentArticlesProcessor', N'Recent articles processor', N'5 recent articles by date')
	INSERT INTO [dbo].[ContentProcessors]
	(Id, [Type], Name, Description)
	VALUES
	(10, N'RecentRecipesProcessor', N'Recent recipes processor', N'1 recent recips by date')
	INSERT INTO [dbo].[ContentProcessors]
	(Id, [Type], Name, Description)
	VALUES
	(11, N'ArticleCategoriesForArticleProcessor', N'Article categories processor', N'Tree view of article categories')

	INSERT INTO [dbo].[MasterContentItemsToContentProcessors]
	([MasterContentItemId],[ContentProcessorId])
	SELECT [Id], 9 FROM [dbo].[MasterContentItems] WHERE [Name] = N'Article master template'
	INSERT INTO [dbo].[MasterContentItemsToContentProcessors]
	([MasterContentItemId],[ContentProcessorId])
	SELECT [Id], 10 FROM [dbo].[MasterContentItems] WHERE [Name] = N'Article master template'
	INSERT INTO [dbo].[MasterContentItemsToContentProcessors]
	([MasterContentItemId],[ContentProcessorId])
	SELECT [Id], 11 FROM [dbo].[MasterContentItems] WHERE [Name] = N'Article master template'
END

GO



