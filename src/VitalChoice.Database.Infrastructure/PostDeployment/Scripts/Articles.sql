IF((SELECT count(*) FROM ContentTypes
WHERE Id IN (3,4))!=2)
BEGIN

DECLARE @contentCategoryId int, @contentItemId int, @articleId int, @masterRootId int,@masterSubId int,@masterLeafId int

INSERT INTO ContentTypes
(Id,Name,DefaultMasterContentItemId)
VALUES
(3,'Article Category',NULL)

INSERT INTO ContentTypes
(Id,Name,DefaultMasterContentItemId)
VALUES
(4,'Article',NULL)

INSERT INTO MasterContentItems
(Name,Template,TypeId,StatusCode)
VALUES
('Article root categories','',3,2)

SELECT @masterRootId=@@IDENTITY

INSERT INTO MasterContentItems
(Name,Template,TypeId,StatusCode)
VALUES
('Article sub categories','',3,2)

SELECT @masterSubId=@@IDENTITY

INSERT INTO MasterContentItems
(Name,Template,TypeId,StatusCode)
VALUES
('Article master template','',4,2)

SELECT @masterLeafId=@@IDENTITY

UPDATE ContentTypes
SET DefaultMasterContentItemId=@masterSubId
WHERE Id=3

UPDATE ContentTypes
SET DefaultMasterContentItemId=@masterLeafId
WHERE Id=4


INSERT INTO ContentItems
(MetaDescription,MetaKeywords,Template,Title,[Description])
VALUES
(NULL,NULL,'default()', 'Articles','')

SELECT @contentItemId=@@IDENTITY

INSERT INTO ContentCategories
(ContentItemId,MasterContentItemId,Name,ParentId,StatusCode,Url, Type, [Order])
VALUES
(@contentItemId,@masterRootId,'Articles',NULL,2,'root',3,0)

END

GO