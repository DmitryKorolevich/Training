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