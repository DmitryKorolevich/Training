IF((SELECT count(*) FROM ContentTypes
WHERE Id IN (5,6,7))!=3)
BEGIN

DECLARE @contentCategoryId int, @contentItemId int, @articleId int, @masterRootId int,@masterSubId int,@masterLeafId int,@masterContentId int

INSERT INTO ContentTypes
(Id,Name,DefaultMasterContentItemId)
VALUES
(5,'FAQ Category',NULL)

INSERT INTO ContentTypes
(Id,Name,DefaultMasterContentItemId)
VALUES
(6,'FAQ',NULL)

INSERT INTO ContentTypes
(Id,Name,DefaultMasterContentItemId)
VALUES
(7,'Content',NULL)

INSERT INTO MasterContentItems
(Name,Template,TypeId,StatusCode)
VALUES
('FAQ root categories','default()',5,2)

SELECT @masterRootId=@@IDENTITY

INSERT INTO MasterContentItems
(Name,Template,TypeId,StatusCode)
VALUES
('FAQ sub categories','default()',5,2)

SELECT @masterSubId=@@IDENTITY

INSERT INTO MasterContentItems
(Name,Template,TypeId,StatusCode)
VALUES
('FAQ master template','default()',6,2)

SELECT @masterLeafId=@@IDENTITY

INSERT INTO MasterContentItems
(Name,Template,TypeId,StatusCode)
VALUES
('Content master template','default()',7,2)

SELECT @masterContentId=@@IDENTITY

UPDATE ContentTypes
SET DefaultMasterContentItemId=@masterSubId
WHERE Id=5

UPDATE ContentTypes
SET DefaultMasterContentItemId=@masterLeafId
WHERE Id=6

UPDATE ContentTypes
SET DefaultMasterContentItemId=@masterContentId
WHERE Id=7


INSERT INTO ContentItems
(MetaDescription,MetaKeywords,Template,Title,[Description])
VALUES
(NULL,NULL,'default()', 'FAQs','')

SELECT @contentItemId=@@IDENTITY

INSERT INTO ContentCategories
(ContentItemId,MasterContentItemId,Name,ParentId,StatusCode,Url, Type, [Order])
VALUES
(@contentItemId,@masterRootId,'FAQs',NULL,2,'root',5,0)

END

GO