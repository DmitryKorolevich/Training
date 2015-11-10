IF OBJECT_ID(N'dbo.Products', N'U') IS NULL
BEGIN

CREATE TABLE [dbo].[Products](
	[Id] [int] NOT NULL,
	[Url] NVARCHAR(250) NOT NULL,
	[StatusCode] INT NOT NULL
			CONSTRAINT FK_ProductsToStatus FOREIGN KEY (StatusCode) REFERENCES dbo.RecordStatusCodes (StatusCode),
	[ContentItemId] [int] NOT NULL,
	[MasterContentItemId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_ToContentItem] FOREIGN KEY([ContentItemId])
REFERENCES [dbo].[ContentItems] ([Id])

ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_ToContentItem]

ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_ToMasterContentItem] FOREIGN KEY([MasterContentItemId])
REFERENCES [dbo].[MasterContentItems] ([Id])

ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_ToMasterContentItem]

CREATE NONCLUSTERED INDEX [IX_Products_MasterContentItemId] ON [dbo].[Products]
(
	[MasterContentItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80)

DECLARE @productId int, @contentItemId int, @masterRootId int

INSERT INTO ContentTypes
(Id,Name,DefaultMasterContentItemId)
VALUES
(10, 'Product',NULL)

INSERT INTO MasterContentItems
(Name,Template,TypeId,StatusCode)
VALUES
('Product page','',10,2)

SELECT @masterRootId=@@IDENTITY

UPDATE ContentTypes
SET DefaultMasterContentItemId=@masterRootId
WHERE Id=10

END

GO

--DECLARE @id int, @url nvarchar(250), @code int, @idContent int, @idMaster int

--SET @idMaster = (SELECT TOP 1 Id FROM MasterContentItems WHERE TypeId=10)

--DECLARE temp CURSOR FOR 
--SELECT Id, Url, StatusCode
--FROM dbo.ProductsEcom
--ORDER BY Id;

--OPEN temp

--FETCH NEXT FROM temp 
--INTO @id, @url, @code

--WHILE @@FETCH_STATUS = 0
--BEGIN

--INSERT INTO [dbo].[ContentItems]
--           ([Template]
--           ,[Description]
--           ,[Title]
--           ,[MetaKeywords]
--           ,[MetaDescription])
--     VALUES
--           (''
--           ,''
--           ,NULL
--           ,NULL
--           ,NULL)

--SET @idContent = @@identity

--INSERT INTO [dbo].[Products]
--           ([Id]
--           ,[Url]
--           ,[StatusCode]
--           ,[ContentItemId]
--           ,[MasterContentItemId])
--     VALUES
--           (@id
--           ,@url
--           ,@code
--           ,@idContent
--           ,@idMaster)

--FETCH NEXT FROM temp 
--INTO @id, @url, @code
--END 
--CLOSE temp;
--DEALLOCATE temp;

--GO

--DROP INDEX [IX_Products_Name] ON [VitalChoice.Ecommerce].[dbo].[Products]
--GO

--DROP INDEX [IX_Products_StatusCode] ON [VitalChoice.Ecommerce].[dbo].[Products]
--GO

--ALTER TABLE [VitalChoice.Ecommerce].[dbo].[Products]
--	DROP COLUMN Url
--GO
