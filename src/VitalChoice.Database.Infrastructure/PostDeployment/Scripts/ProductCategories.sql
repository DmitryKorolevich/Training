IF OBJECT_ID(N'dbo.ProductCategories', N'U') IS NULL
BEGIN

CREATE TABLE [dbo].[ProductCategories](
	[Id] [int] NOT NULL,
	[ContentItemId] [int] NOT NULL,
	[MasterContentItemId] [int] NOT NULL,
	[FileImageSmallUrl] [nvarchar](250) NULL,
	[FileImageLargeUrl] [nvarchar](250) NULL,
	[LongDescription] [nvarchar](max) NULL,
	[LongDescriptionBottom] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[ProductCategories]  WITH CHECK ADD  CONSTRAINT [FK_ProductCategories_ToContentItem] FOREIGN KEY([ContentItemId])
REFERENCES [dbo].[ContentItems] ([Id])

ALTER TABLE [dbo].[ProductCategories] CHECK CONSTRAINT [FK_ProductCategories_ToContentItem]

ALTER TABLE [dbo].[ProductCategories]  WITH CHECK ADD  CONSTRAINT [FK_ProductCategories_ToMasterContentItem] FOREIGN KEY([MasterContentItemId])
REFERENCES [dbo].[MasterContentItems] ([Id])

ALTER TABLE [dbo].[ProductCategories] CHECK CONSTRAINT [FK_ProductCategories_ToMasterContentItem]

CREATE NONCLUSTERED INDEX [IX_ProductCategories_MasterContentItemId] ON [dbo].[ProductCategories]
(
	[MasterContentItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80)

DECLARE @productCategoryId int, @contentItemId int, @masterRootId int,@masterSubId int

INSERT INTO ContentTypes
(Id,Name,DefaultMasterContentItemId)
VALUES
(9,'Product Category',NULL)

INSERT INTO MasterContentItems
(Name,Template,TypeId,StatusCode)
VALUES
('Product root categories','',9,2)

SELECT @masterRootId=@@IDENTITY

INSERT INTO MasterContentItems
(Name,Template,TypeId,StatusCode)
VALUES
('Product sub categories','',9,2)

SELECT @masterSubId=@@IDENTITY

UPDATE ContentTypes
SET DefaultMasterContentItemId=@masterSubId
WHERE Id=9

INSERT INTO ContentItems
(MetaDescription,MetaKeywords,Template,Title,[Description])
VALUES
(NULL,NULL,'', 'Products','')

SELECT @contentItemId=@@IDENTITY

INSERT INTO ProductCategories
(Id,ContentItemId,MasterContentItemId)
VALUES
(1,@contentItemId,@masterRootId)

END

GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'ProductCategories') AND Name = N'NavLabel')
BEGIN
	ALTER TABLE ProductCategories ADD NavLabel NVARCHAR(250) NULL
	ALTER TABLE ProductCategories ADD NavIdVisible INT CONSTRAINT DV_NavIdVisible DEFAULT 1 WITH VALUES	
	
	ALTER TABLE ProductCategories DROP CONSTRAINT DV_NavIdVisible
	ALTER TABLE ProductCategories ALTER COLUMN NavIdVisible INT NULL	
END

GO