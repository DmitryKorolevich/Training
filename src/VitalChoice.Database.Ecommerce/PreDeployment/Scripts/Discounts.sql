﻿IF OBJECT_ID(N'[dbo].[DiscountTypes]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[DiscountTypes]
	(
		[Id] INT NOT NULL PRIMARY KEY, 
		[Name] NVARCHAR(50) NOT NULL
	);

	INSERT INTO DiscountTypes
	(Id, Name)
	SELECT 1, 'Price Discount'
	UNION
	SELECT 2, 'Percent Discount'
	UNION
	SELECT 3, 'Free Shipping'
	UNION
	SELECT 4, 'Threshold'
	UNION
	SELECT 5, 'Tiered'
END

GO

IF OBJECT_ID(N'[dbo].[Discounts]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[Discounts]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[StatusCode] INT NOT NULL DEFAULT 1,
		[DateCreated] DATETIME2 NOT NULL DEFAULT SYSDATETIME(), 
		[DateEdited] DATETIME2 NOT NULL DEFAULT SYSDATETIME(), 
		[IdDiscountType] INT NOT NULL, 
		[IdExternal] INT NULL, 
		[Description] NVARCHAR(250) NOT NULL, 
		[Code] NVARCHAR(250) NOT NULL,
		[Assigned] INT NOT NULL,
		[StartDate] DATETIME2 NULL,
		[ExpirationDate] DATETIME2 NULL,
		CONSTRAINT [FK_Discounts_ToProductType] FOREIGN KEY ([IdDiscountType]) REFERENCES [DiscountTypes]([Id]), 
		CONSTRAINT [FK_Discounts_ToRecordStatusCode] FOREIGN KEY ([StatusCode]) REFERENCES [RecordStatusCodes] ([StatusCode])
	);

	CREATE INDEX [IX_Discounts_Code] ON [dbo].[Discounts] ([Code], StatusCode)
	CREATE INDEX [IX_Discounts_StatusCode] ON [dbo].[Discounts] (StatusCode)
END

GO

IF (( SELECT Count(*) AS existFlag FROM sys.columns 
WHERE [name] = N'ExcludeSkus' AND [object_id] = OBJECT_ID(N'Discounts')) = 0
AND (SELECT Count(*) AS existFlag FROM sys.columns 
WHERE [object_id] = OBJECT_ID(N'Discounts')) > 0)
BEGIN

ALTER TABLE Discounts ADD ExcludeSkus BIT NOT NULL DEFAULT(0)
ALTER TABLE Discounts ADD ExcludeCategories BIT NOT NULL DEFAULT(0)

END

GO

ALTER TABLE [dbo].[Discounts] ALTER COLUMN [StartDate] DATETIME2 NULL

GO

IF OBJECT_ID(N'[dbo].[DiscountsToCategories]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[DiscountsToCategories]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[IdCategory] INT NOT NULL, 
		[IdDiscount] INT NOT NULL, 		
		[Include] BIT NOT NULL, 
		CONSTRAINT [FK_DiscountsToCategories_ToDiscount] FOREIGN KEY ([IdDiscount]) REFERENCES [Discounts]([Id]), 
		CONSTRAINT [FK_DiscountsToCategories_ToProductCategory] FOREIGN KEY ([IdCategory]) REFERENCES [ProductCategories]([Id])
	)
END

GO

IF OBJECT_ID(N'[dbo].[DiscountsToProducts]', N'U') IS NOT NULL
BEGIN
	DROP TABLE [dbo].[DiscountsToProducts]
END

GO

IF OBJECT_ID(N'[dbo].[DiscountsToSelectedProducts]', N'U') IS NOT NULL
BEGIN
	DROP TABLE [dbo].[DiscountsToSelectedProducts]
END

GO

IF OBJECT_ID(N'[dbo].[DiscountsToSkus]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[DiscountsToSkus]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[IdDiscount] INT NOT NULL, 
		[IdSku] INT NOT NULL, 
		CONSTRAINT [FK_DiscountsToSkus_ToDiscount] FOREIGN KEY ([IdDiscount]) REFERENCES [Discounts]([Id]), 
		CONSTRAINT [FK_DiscountsToSkus_ToSku] FOREIGN KEY ([IdSku]) REFERENCES [Skus]([Id])
	)
END

GO

IF OBJECT_ID(N'[dbo].[DiscountsToSelectedSkus]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].DiscountsToSelectedSkus
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[IdDiscount] INT NOT NULL, 
		[IdSku] INT NOT NULL, 
		CONSTRAINT [FK_DiscountsToSelectedSkus_ToDiscount] FOREIGN KEY ([IdDiscount]) REFERENCES [Discounts]([Id]), 
		CONSTRAINT [FK_DiscountsToSelectedSkus_ToSku] FOREIGN KEY ([IdSku]) REFERENCES [Skus]([Id])
	)
END

GO

IF OBJECT_ID(N'[dbo].[DiscountTiers]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[DiscountTiers]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[IdDiscount] INT NOT NULL, 
		[From] MONEY NOT NULL, 
		[To] MONEY NOT NULL, 
		[IdDiscountType] INT NOT NULL, 
		[Percent] DECIMAL(3,2) NOT NULL, 
		[Amount] MONEY NOT NULL, 
		[Order] INT NOT NULL, 
		CONSTRAINT [FK_DiscountTiers_ToDiscount] FOREIGN KEY ([IdDiscount]) REFERENCES [Discounts]([Id]),
	)
END

ALTER TABLE [dbo].[DiscountTiers] ALTER COLUMN [Percent] DECIMAL(5,2) NULL
ALTER TABLE [dbo].[DiscountTiers] ALTER COLUMN [Amount] MONEY NULL 

GO

IF OBJECT_ID(N'[dbo].[DiscountOptionTypes]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[DiscountOptionTypes]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[Name] NVARCHAR(50) NOT NULL, 
		[IdFieldType] INT NOT NULL, 
		[IdLookup] INT NULL, 
		[IdDiscountType] INT NULL, 
		[DefaultValue] NVARCHAR(250) NULL, 
		CONSTRAINT [FK_DiscountOptionTypes_ToLookup] FOREIGN KEY ([IdLookup]) REFERENCES [Lookups]([Id]), 
		CONSTRAINT [FK_DiscountOptionTypes_ToFieldType] FOREIGN KEY ([IdFieldType]) REFERENCES [FieldTypes]([Id]), 
		CONSTRAINT [FK_DiscountOptionTypes_ToDiscountType] FOREIGN KEY ([IdDiscountType]) REFERENCES [DiscountTypes]([Id])
	);

	CREATE INDEX [IX_DiscountOptionTypes_Name] ON [dbo].[DiscountOptionTypes] ([Name]) INCLUDE (Id, IdFieldType, IdDiscountType)
END

GO

IF OBJECT_ID(N'[dbo].[DiscountOptionValues]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[DiscountOptionValues]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[IdDiscount] INT NOT NULL,
		[IdOptionType] INT NOT NULL, 
		[Value] NVARCHAR(250) NULL, 
		CONSTRAINT [FK_DiscountOptionValue_ToDiscountOptionType] FOREIGN KEY ([IdOptionType]) REFERENCES [DiscountOptionTypes]([Id]), 
		CONSTRAINT [FK_DiscountOptionValues_ToDiscount] FOREIGN KEY ([IdDiscount]) REFERENCES [Discounts]([Id])
	);
	
	CREATE INDEX [IX_DiscountOptionValues_Value] ON [dbo].[DiscountOptionValues] ([Value]) INCLUDE (Id, IdDiscount, IdOptionType)
END

GO

IF EXISTS(SELECT * FROM sys.columns WHERE [object_id] = OBJECT_ID(N'[dbo].[Discounts]', N'U') AND Name = 'IdDiscountType')
BEGIN
	EXEC sp_rename 'dbo.Discounts.IdDiscountType', 'IdObjectType', 'COLUMN';
	EXEC sp_rename 'dbo.DiscountOptionTypes.IdDiscountType', 'IdObjectType', 'COLUMN';
END

GO

IF EXISTS(SELECT * FROM sys.columns WHERE [object_id] = OBJECT_ID(N'[dbo].[DiscountsToCategories]', N'U') AND Name = 'Include')
	ALTER TABLE DiscountsToCategories
	DROP COLUMN [Include]

GO

IF ('NO' =(select TOP 1 IS_NULLABLE from INFORMATION_SCHEMA.COLUMNS 
where TABLE_NAME ='Discounts' and COLUMN_NAME ='Assigned'))
BEGIN
ALTER TABLE Discounts ALTER COLUMN Assigned INT NULL

UPDATE Discounts
SET Assigned=NULL
WHERE Assigned=1

UPDATE Discounts
SET Assigned=1
WHERE Assigned=3

ALTER TABLE dbo.Discounts ADD CONSTRAINT
	FK_Discounts_CustomerTypes FOREIGN KEY
	(
	Assigned
	) REFERENCES dbo.CustomerTypes
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

END

GO

IF OBJECT_ID(N'[dbo].[DiscountToSelectedCategories]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[DiscountToSelectedCategories]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[IdCategory] INT NOT NULL, 
		[IdDiscount] INT NOT NULL, 		
		CONSTRAINT [FK_DiscountToSelectedCategories_ToDiscount] FOREIGN KEY ([IdDiscount]) REFERENCES [Discounts]([Id]), 
		CONSTRAINT [FK_DDiscountToSelectedCategories_ToProductCategory] FOREIGN KEY ([IdCategory]) REFERENCES [ProductCategories]([Id])
	)
END

GO