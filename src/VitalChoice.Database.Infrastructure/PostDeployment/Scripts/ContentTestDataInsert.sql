DECLARE @contentCategoryId int, @contentItemId int, @recipeId int

INSERT INTO MasterContentItems
(Id,Name,Template,Type)
VALUES
(1,'Recipe categories','Template',1)

INSERT INTO MasterContentItems
(Id,Name,Template,Type)
VALUES
(2,'Recipe master template','Template',1)

INSERT INTO ContentItems
(MetaDescription,MetaKeywords,Name,Template,Title)
VALUES
(NULL,NULL,'Recipe categories root template','Template', 'Recipe categories')

SELECT @contentItemId=@@IDENTITY

INSERT INTO ContentItemsToContentProcessors
(ContentItemId,ContentItemProcessorId)
VALUES
(@contentItemId,1)

INSERT INTO ContentCategories
(ContentItemId,MasterContentItemId,Name,ParentId,StatusCode,Url)
VALUES
(@contentItemId,1,'Recipes',NULL,2,'root')

SELECT @contentCategoryId=@@IDENTITY

INSERT INTO ContentItems
(MetaDescription,MetaKeywords,Name,Template,Title)
VALUES
(NULL,NULL,'Recipe sub categories template','Template', 'Recipe categories')

SELECT @contentItemId=@@IDENTITY

INSERT INTO ContentItemsToContentProcessors
(ContentItemId,ContentItemProcessorId)
VALUES
(@contentItemId,2)

INSERT INTO ContentItemsToContentProcessors
(ContentItemId,ContentItemProcessorId)
VALUES
(@contentItemId,3)

INSERT INTO ContentCategories
(ContentItemId,MasterContentItemId,Name,ParentId,StatusCode,Url)
VALUES
(@contentItemId,1,'Wild Salmon',@contentCategoryId,2,'wild-salmon')

SELECT @contentCategoryId=@@IDENTITY

INSERT INTO ContentCategories
(ContentItemId,MasterContentItemId,Name,ParentId,StatusCode,Url)
VALUES
(NULL,1,'Salmon Portions & Sides',@contentCategoryId,2,'salmon-portions-and-sides')

SELECT @contentCategoryId=@@IDENTITY

INSERT INTO ContentItems
(MetaDescription,MetaKeywords,Name,Template,Title)
VALUES
(NULL,NULL,'Grilled Salmon with Blueberry-Horseradish Glaze','Data template - Grilled Salmon with Blueberry-Horseradish Glaze', 'Grilled Salmon with Blueberry-Horseradish Glaze')

SELECT @contentItemId=@@IDENTITY

INSERT Recipes
(ContentItemId,MasterContentItemId,Name,StatusCode,Url)
VALUES
(@contentItemId,2,'Grilled Salmon with Blueberry-Horseradish Glaze',2,'grilled-salmon-with-blueberry-horseradish-glaze')

SELECT @recipeId=@@IDENTITY

INSERT INTO RecipesToContentCategories
(ContentCategoryId,RecipesId)
VALUES
(@contentCategoryId,@recipeId)

INSERT INTO ContentItems
(MetaDescription,MetaKeywords,Name,Template,Title)
VALUES
(NULL,NULL,'Cedar Plank Salmon; John’s Fish Marinade','Data template - Cedar Plank Salmon; John’s Fish Marinade', 'Cedar Plank Salmon; John’s Fish Marinade')

SELECT @contentItemId=@@IDENTITY

INSERT Recipes
(ContentItemId,MasterContentItemId,Name,StatusCode,Url)
VALUES
(@contentItemId,2,'Cedar Plank Salmon; John’s Fish Marinade',2,'cedar-plank-salmon')

SELECT @recipeId=@@IDENTITY

INSERT INTO RecipesToContentCategories
(ContentCategoryId,RecipesId)
VALUES
(@contentCategoryId,@recipeId)