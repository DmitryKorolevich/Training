IF OBJECT_ID(N'[dbo].[Products]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[Products]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[StatusCode] INT NOT NULL DEFAULT 1,
		[DateCreated] DATETIME2 NOT NULL DEFAULT SYSDATETIME(), 
		[DateEdited] DATETIME2 NOT NULL DEFAULT SYSDATETIME(), 
		[IdProductType] INT NOT NULL, 
		[IdExternal] INT NULL, 
		[Name] NVARCHAR(250) NOT NULL, 
		[Url] NVARCHAR(250) NOT NULL, 
		CONSTRAINT [FK_Products_ToProductType] FOREIGN KEY ([IdProductType]) REFERENCES [ProductTypes]([Id]), 
		CONSTRAINT [FK_Products_ToRecordStatusCode] FOREIGN KEY ([StatusCode]) REFERENCES [RecordStatusCodes] ([StatusCode])
	);

	CREATE INDEX [IX_Products_Name] ON [dbo].[Products] ([Name], StatusCode) INCLUDE(Id, DateCreated, DateEdited, IdProductType, IdExternal, Url);
	CREATE INDEX [IX_Products_StatusCode] ON [dbo].[Products] (StatusCode) INCLUDE(Id, Name, DateCreated, DateEdited, IdProductType, IdExternal, Url)
END

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'Hidden' AND [Object_ID] = OBJECT_ID(N'[dbo].Products', N'U'))
	ALTER TABLE dbo.Products
	ADD Hidden BIT NOT NULL DEFAULT 0

GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'EditedById' AND [Object_ID] = OBJECT_ID(N'[dbo].Products', N'U'))
	EXEC SP_RENAME 'Products.IdExternal' , 'EditedById', 'COLUMN'

GO