IF((SELECT count(*) FROM ContentTypes
WHERE Id IN (7,8))!=2)
BEGIN

DECLARE @contentCategoryId int, @contentItemId int, @articleId int, @masterRootId int,@masterSubId int,@masterLeafId int

INSERT INTO ContentTypes
(Id,Name,DefaultMasterContentItemId)
VALUES
(7,'Content Page Category',NULL)

INSERT INTO ContentTypes
(Id,Name,DefaultMasterContentItemId)
VALUES
(8,'Content Page',NULL)

INSERT INTO MasterContentItems
(Name,Template,TypeId,StatusCode)
VALUES
('Content page root categories','',7,2)

SELECT @masterRootId=@@IDENTITY

INSERT INTO MasterContentItems
(Name,Template,TypeId,StatusCode)
VALUES
('Content page sub categories','',7,2)

SELECT @masterSubId=@@IDENTITY

INSERT INTO MasterContentItems
(Name,Template,TypeId,StatusCode)
VALUES
('Content page master template','',8,2)

SELECT @masterLeafId=@@IDENTITY

UPDATE ContentTypes
SET DefaultMasterContentItemId=@masterSubId
WHERE Id=7

UPDATE ContentTypes
SET DefaultMasterContentItemId=@masterLeafId
WHERE Id=8


INSERT INTO ContentItems
(MetaDescription,MetaKeywords,Template,Title,[Description])
VALUES
(NULL,NULL,'default()', 'Content Pages','')

SELECT @contentItemId=@@IDENTITY

INSERT INTO ContentCategories
(ContentItemId,MasterContentItemId,Name,ParentId,StatusCode,Url, Type, [Order])
VALUES
(@contentItemId,@masterRootId,'Content Pages',NULL,2,'root',7,0)

END

GO