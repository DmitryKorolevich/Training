IF OBJECT_ID(N'[dbo].[DiscountTypes]', N'U') IS NULL
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
		[StartDate] DATETIME2 NOT NULL,
		[ExpirationDate] DATETIME2 NULL,
		CONSTRAINT [FK_Discounts_ToProductType] FOREIGN KEY ([IdDiscountType]) REFERENCES [DiscountTypes]([Id]), 
		CONSTRAINT [FK_Discounts_ToRecordStatusCode] FOREIGN KEY ([StatusCode]) REFERENCES [RecordStatusCodes] ([StatusCode])
	);

	CREATE INDEX [IX_Discounts_Code] ON [dbo].[Discounts] ([Code], StatusCode)
	CREATE INDEX [IX_Discounts_StatusCode] ON [dbo].[Discounts] (StatusCode)
END

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

IF OBJECT_ID(N'[dbo].[DiscountsToProducts]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[DiscountsToProducts]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[IdProduct] INT NOT NULL, 
		[IdDiscount] INT NOT NULL, 
		[Include] BIT NOT NULL, 
		CONSTRAINT [FK_DiscountsToProducts_ToDiscount] FOREIGN KEY ([IdDiscount]) REFERENCES [Discounts]([Id]), 
		CONSTRAINT [FK_DiscountsToProducts_ToProduct] FOREIGN KEY ([IdProduct]) REFERENCES [Products]([Id])
	)
END

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