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

IF NOT EXISTS(SELECT * FROM ProductOptionTypes)
BEGIN
--non-perishable product type

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdProductType, Name)
	SELECT NULL, 4, 1, N'Description'
	UNION
	SELECT NULL, 4, 1, N'Serving'
	UNION
	SELECT NULL, 4, 1, N'Recepies'
	UNION
	SELECT NULL, 4, 1, N'Ingredients'
	UNION
	SELECT NULL, 4, 1, N'ShortDescription'
	UNION
	SELECT NULL, 4, 1, N'ProductNotes'
	UNION
	SELECT NULL, 4, 1, N'MetaTitle'
	UNION
	SELECT NULL, 4, 1, N'MetaDescription'
	UNION
	SELECT NULL, 4, 1, N'GoogleCategory'
	UNION
	SELECT NULL, 4, 1, N'TaxCode'
	UNION
	SELECT NULL, 4, 1, N'SpecialIcon'
	UNION
	SELECT NULL, 4, 1, N'Thumbnail'
	UNION
	SELECT NULL, 4, 1, N'NutritionalTitle'
	UNION
	SELECT NULL, 4, 1, N'MainProductImage'
	UNION
	SELECT N'0', 3, 1, N'Stock'
	UNION
	SELECT N'1', 5, 1, N'DisregardStock'
	UNION
	SELECT N'0', 5, 1, N'DisallowSingle'
	UNION
	SELECT N'0', 5, 1, N'NonDiscountable'
	UNION
	SELECT N'0', 5, 1, N'OrphanType'
	UNION
	SELECT N'0', 5, 1, N'AutoShipProduct'
	UNION
	SELECT N'0', 2, 1, N'OffPercent'
	UNION
	SELECT N'0', 5, 1, N'HideFromDataFeed'
	UNION
	SELECT N'0', 5, 1, N'AutoShipFrequency1'
	UNION
	SELECT N'0', 5, 1, N'AutoShipFrequency2'
	UNION
	SELECT N'0', 5, 1, N'AutoShipFrequency3'
	UNION
	SELECT N'0', 5, 1, N'AutoShipFrequency6'
	
	DECLARE @IdLookup INT
	
	INSERT INTO Lookups
	(LookupValueType)
	VALUES
	(N'string')

	SET @IdLookup = SCOPE_IDENTITY()

	INSERT INTO LookupVariants
	(Id, IdLookup, ValueVariant)
	SELECT 1, @IdLookup, 'ModSeller'
	UNION
	SELECT 2, @IdLookup, 'LowSeller'
	UNION
	SELECT 3, @IdLookup, 'BestSeller'

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdProductType, Name, IdLookup)
	SELECT '1', 2, 1, 'Seller', @IdLookup

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdProductType, Name, IdLookup)
	SELECT '1', 2, 2, 'Seller', @IdLookup

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdProductType, Name, IdLookup)
	SELECT '1', 2, 3, 'Seller', @IdLookup

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdProductType, Name, IdLookup)
	SELECT '1', 2, 4, 'Seller', @IdLookup

END