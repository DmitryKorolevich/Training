DECLARE @contentCategoryId int, @contentItemId int, @recipeId int

INSERT INTO ContentTypes
(Id,Name,DefaultMasterContentItemId)
VALUES
(1,'Category',NULL)

INSERT INTO ContentTypes
(Id,Name,DefaultMasterContentItemId)
VALUES
(2,'Recipe',NULL)

INSERT INTO MasterContentItems
(Id,Name,Template,TypeId)
VALUES
(1,'Recipe root categories','Template',1)

INSERT INTO MasterContentItems
(Id,Name,Template,TypeId)
VALUES
(2,'Recipe sub categories','Template',1)

INSERT INTO MasterContentItems
(Id,Name,Template,TypeId)
VALUES
(3,'Recipe master template','Template',2)

UPDATE ContentTypes
SET DefaultMasterContentItemId=1
WHERE Id=1

UPDATE ContentTypes
SET DefaultMasterContentItemId=3
WHERE Id=2

INSERT INTO MasterContentItemsToContentProcessors
(MasterContentItemId,ContentProcessorId)
VALUES
(1,1)

INSERT INTO MasterContentItemsToContentProcessors
(MasterContentItemId,ContentProcessorId)
VALUES
(2,2)

INSERT INTO MasterContentItemsToContentProcessors
(MasterContentItemId,ContentProcessorId)
VALUES
(2,3)

INSERT INTO ContentItems
(MetaDescription,MetaKeywords,Template,Title)
VALUES
(NULL,NULL,'Recipe categories root template', 'Recipe categories')

SELECT @contentItemId=@@IDENTITY

INSERT INTO ContentCategories
(ContentItemId,MasterContentItemId,Name,ParentId,StatusCode,Url, Type)
VALUES
(@contentItemId,1,'Recipes',NULL,2,'root',2)

SELECT @contentCategoryId=@@IDENTITY

INSERT INTO ContentItems
(MetaDescription,MetaKeywords,Template,Title)
VALUES
(NULL,NULL,'Recipe sub categories template','Recipe categories')

SELECT @contentItemId=@@IDENTITY

INSERT INTO ContentCategories
(ContentItemId,MasterContentItemId,Name,ParentId,StatusCode,Url, Type)
VALUES
(@contentItemId,2,'Wild Salmon',@contentCategoryId,2,'wild-salmon',2)

SELECT @contentCategoryId=@@IDENTITY

INSERT INTO ContentCategories
(ContentItemId,MasterContentItemId,Name,ParentId,StatusCode,Url, Type)
VALUES
(NULL,2,'Salmon Portions & Sides',@contentCategoryId,2,'salmon-portions-and-sides',2)

SELECT @contentCategoryId=@@IDENTITY

INSERT INTO ContentItems
(MetaDescription,MetaKeywords,Template,Title)
VALUES
(NULL,NULL,'Data template - Grilled Salmon with Blueberry-Horseradish Glaze', 'Grilled Salmon with Blueberry-Horseradish Glaze')

SELECT @contentItemId=@@IDENTITY

INSERT Recipes
(ContentItemId,MasterContentItemId,Name,StatusCode,Url)
VALUES
(@contentItemId,3,'Grilled Salmon with Blueberry-Horseradish Glaze',2,'grilled-salmon-with-blueberry-horseradish-glaze')

SELECT @recipeId=@@IDENTITY

INSERT INTO RecipesToContentCategories
(ContentCategoryId,RecipeId)
VALUES
(@contentCategoryId,@recipeId)

INSERT INTO ContentItems
(MetaDescription,MetaKeywords,Template,Title)
VALUES
(NULL,NULL,'Data template - Cedar Plank Salmon; John’s Fish Marinade', 'Cedar Plank Salmon; John’s Fish Marinade')

SELECT @contentItemId=@@IDENTITY

INSERT Recipes
(ContentItemId,MasterContentItemId,Name,StatusCode,Url)
VALUES
(@contentItemId,3,'Cedar Plank Salmon; John’s Fish Marinade',2,'cedar-plank-salmon')

SELECT @recipeId=@@IDENTITY

INSERT INTO RecipesToContentCategories
(ContentCategoryId,RecipeId)
VALUES
(@contentCategoryId,@recipeId)