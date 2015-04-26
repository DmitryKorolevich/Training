IF OBJECT_ID(N'dbo.ProductCategories', N'U') IS NULL
BEGIN

CREATE TABLE [dbo].[ProductCategories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ContentItemId] [int] NOT NULL,
	[MasterContentItemId] [int] NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[FileImageSmallUrl] [nvarchar](250) NULL,
	[FileImageLargeUrl] [nvarchar](250) NULL,
	[ParentId] [int] NULL,
	[StatusCode] [int] NOT NULL DEFAULT ((1)),
	[Url] [nvarchar](250) NOT NULL,
	[Order] [int] NOT NULL,
	[LongDescription] [nvarchar](max) NULL,
	[LongDescriptionBottom] [nvarchar](max) NULL,
	[Assigned][int] NOT NULL DEFAULT ((1)),
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[ProductCategories]  WITH CHECK ADD  CONSTRAINT [FK_ProductCategories_ToContentCategory] FOREIGN KEY([ParentId])
REFERENCES [dbo].[ProductCategories] ([Id])

ALTER TABLE [dbo].[ProductCategories] CHECK CONSTRAINT [FK_ProductCategories_ToContentCategory]

ALTER TABLE [dbo].[ProductCategories]  WITH CHECK ADD  CONSTRAINT [FK_ProductCategories_ToContentItem] FOREIGN KEY([ContentItemId])
REFERENCES [dbo].[ContentItems] ([Id])

ALTER TABLE [dbo].[ProductCategories] CHECK CONSTRAINT [FK_ProductCategories_ToContentItem]

ALTER TABLE [dbo].[ProductCategories]  WITH CHECK ADD  CONSTRAINT [FK_ProductCategories_ToMasterContentItem] FOREIGN KEY([MasterContentItemId])
REFERENCES [dbo].[MasterContentItems] ([Id])

ALTER TABLE [dbo].[ProductCategories] CHECK CONSTRAINT [FK_ProductCategories_ToMasterContentItem]

ALTER TABLE [dbo].[ProductCategories]  WITH CHECK ADD  CONSTRAINT [FK_ProductCategories_ToRecordStatusCode] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])

ALTER TABLE [dbo].[ProductCategories] CHECK CONSTRAINT [FK_ProductCategories_ToRecordStatusCode]

CREATE NONCLUSTERED INDEX [IX_ProductCategories_MasterContentItemId] ON [dbo].[ProductCategories]
(
	[MasterContentItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80)

CREATE NONCLUSTERED INDEX [IX_ProductCategories_ParentId] ON [dbo].[ProductCategories]
(
	[ParentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80)

CREATE NONCLUSTERED INDEX [IX_ProductCategories_Url] ON [dbo].[ProductCategories]
(
	[Url] ASC
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
(ContentItemId,MasterContentItemId,Name,ParentId,StatusCode,Url, [Order])
VALUES
(@contentItemId,@masterRootId,'Products',NULL,2,'root',0)

END