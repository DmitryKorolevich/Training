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

IF OBJECT_ID(N'[dbo].[ProductOptionTypes]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[ProductOptionTypes]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[Name] NVARCHAR(50) NOT NULL, 
		[IdFieldType] INT NOT NULL, 
		[IdLookup] INT NULL, 
		[IdProductType] INT NULL, 
		[DefaultValue] NVARCHAR(250) NULL, 
		CONSTRAINT [FK_ProductOptionTypes_ToLookup] FOREIGN KEY ([IdLookup]) REFERENCES [Lookups]([Id]), 
		CONSTRAINT [FK_ProductOptionTypes_ToFieldType] FOREIGN KEY ([IdFieldType]) REFERENCES [FieldTypes]([Id]), 
		CONSTRAINT [FK_ProductOptionTypes_ToProductType] FOREIGN KEY ([IdProductType]) REFERENCES [ProductTypes]([Id])
	);

	CREATE INDEX [IX_ProductOptionTypes_Name] ON [dbo].[ProductOptionTypes] ([Name]) INCLUDE (Id, IdFieldType, IdProductType)
END

GO

IF OBJECT_ID(N'[dbo].[Skus]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[Skus]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[IdProduct] INT NOT NULL, 
		[DateCreated] DATETIME2 NOT NULL DEFAULT SYSDATETIME(), 
		[DateEdited] DATETIME2 NOT NULL DEFAULT SYSDATETIME(), 
		[StatusCode] INT NOT NULL DEFAULT 1, 
		[Price] MONEY NOT NULL DEFAULT 0, 
		[WholesalePrice] MONEY NOT NULL DEFAULT 0, 
		[Code] NVARCHAR(20) NOT NULL, 
		CONSTRAINT [FK_Skus_ToProduct] FOREIGN KEY ([IdProduct]) REFERENCES [Products]([Id]),
		CONSTRAINT [FK_Skus_ToRecordStatusCode] FOREIGN KEY ([StatusCode]) REFERENCES [RecordStatusCodes] ([StatusCode])
	);

	CREATE INDEX [IX_Skus_Code] ON [dbo].[Skus] ([Code], [StatusCode]) INCLUDE (Id, IdProduct, DateCreated, DateEdited, Price, WholesalePrice)
END

GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'Hidden' AND [Object_ID] = OBJECT_ID(N'[dbo].Skus', N'U'))
	ALTER TABLE dbo.Skus
	ADD Hidden BIT NOT NULL DEFAULT 0

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'Order' AND [Object_ID] = OBJECT_ID(N'[dbo].Skus', N'U'))
	ALTER TABLE Skus ADD [Order] INT NOT NULL DEFAULT(0)

GO

IF OBJECT_ID(N'[dbo].[ProductOptionValues]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[ProductOptionValues]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[IdProduct] INT NULL, 
		[IdSku] INT NULL, 
		[IdOptionType] INT NOT NULL, 
		[Value] NVARCHAR(250) NULL, 
		CONSTRAINT [FK_ProductOptionValue_ToProductOptionType] FOREIGN KEY ([IdOptionType]) REFERENCES [ProductOptionTypes]([Id]), 
		CONSTRAINT [FK_ProductOptionValues_ToSku] FOREIGN KEY ([IdSku]) REFERENCES [Skus]([Id]), 
		CONSTRAINT [FK_ProductOptionValues_ToProduct] FOREIGN KEY ([IdProduct]) REFERENCES [Products]([Id])
	);

	CREATE INDEX [IX_ProductOptionValues_Value] ON [dbo].[ProductOptionValues] ([Value]) INCLUDE (Id, IdProduct, IdSku, IdOptionType)
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ProductOptionValues]') AND name = N'IdBigString')
	ALTER TABLE [ProductOptionValues]
	ADD [IdBigString] BIGINT NULL,
	CONSTRAINT [FK_ProductOptionValue_ToBigStringValue] FOREIGN KEY ([IdBigString]) REFERENCES [BigStringValues]([IdBigString]) ON DELETE CASCADE

IF EXISTS(SELECT * FROM sys.columns WHERE [object_id] = OBJECT_ID(N'[dbo].[Products]', N'U') AND Name = 'IdProductType')
BEGIN
	EXEC sp_rename 'dbo.Products.IdProductType', 'IdObjectType', 'COLUMN';
	EXEC sp_rename 'dbo.ProductOptionTypes.IdProductType', 'IdObjectType', 'COLUMN';
END