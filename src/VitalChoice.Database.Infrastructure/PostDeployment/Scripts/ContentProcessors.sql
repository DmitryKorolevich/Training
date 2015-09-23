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