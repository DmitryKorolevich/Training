IF OBJECT_ID(N'[dbo].[PromotionTypes]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[PromotionTypes]
	(
		[Id] INT NOT NULL PRIMARY KEY, 
		[Name] NVARCHAR(50) NOT NULL
	);

	INSERT INTO PromotionTypes
	(Id, Name)
	SELECT 1, 'Buy X Get Y'
END

GO

IF OBJECT_ID(N'[dbo].[Promotions]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[Promotions]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[StatusCode] INT NOT NULL DEFAULT 1,
		[DateCreated] DATETIME2 NOT NULL DEFAULT SYSDATETIME(), 
		[DateEdited] DATETIME2 NOT NULL DEFAULT SYSDATETIME(), 
		[IdObjectType] INT NOT NULL, 
		[IdEditedBy] INT NULL, 
		[IdAddedBy] INT NULL, 
		[Description] NVARCHAR(250) NOT NULL, 
		[Assigned] INT NULL,
		[StartDate] DATETIME2 NULL,
		[ExpirationDate] DATETIME2 NULL,
		CONSTRAINT [FK_Promotions_ToPromotionType] FOREIGN KEY ([IdObjectType]) REFERENCES [PromotionTypes]([Id]), 
		CONSTRAINT [FK_Promotions_ToRecordStatusCode] FOREIGN KEY ([StatusCode]) REFERENCES [RecordStatusCodes] ([StatusCode])
	);

	CREATE INDEX [IX_Promotions_StatusCode] ON [dbo].[Promotions] (StatusCode)

	ALTER TABLE dbo.Promotions ADD CONSTRAINT
	FK_Promotions_CustomerTypes FOREIGN KEY
	(
	Assigned
	) REFERENCES dbo.CustomerTypes
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
END

GO

IF OBJECT_ID(N'[dbo].[PromotionOptionTypes]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[PromotionOptionTypes]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[Name] NVARCHAR(50) NOT NULL, 
		[IdFieldType] INT NOT NULL, 
		[IdLookup] INT NULL, 
		[IdObjectType] INT NULL, 
		[DefaultValue] NVARCHAR(250) NULL, 
		CONSTRAINT [FK_PromotionOptionTypes_ToLookup] FOREIGN KEY ([IdLookup]) REFERENCES [Lookups]([Id]), 
		CONSTRAINT [FK_PromotionOptionTypes_ToFieldType] FOREIGN KEY ([IdFieldType]) REFERENCES [FieldTypes]([Id]), 
		CONSTRAINT [FK_PromotionOptionTypes_ToPromotionType] FOREIGN KEY ([IdObjectType]) REFERENCES [PromotionTypes]([Id])
	);

	CREATE INDEX [IX_PromotionOptionTypes_Name] ON [dbo].[PromotionOptionTypes] ([Name]) INCLUDE (Id, IdFieldType, IdObjectType)
END

GO

IF OBJECT_ID(N'[dbo].[PromotionOptionValues]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[PromotionOptionValues]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[IdPromotion] INT NOT NULL,
		[IdOptionType] INT NOT NULL, 
		[Value] NVARCHAR(250) NULL, 
		CONSTRAINT [FK_PromotionOptionValue_ToPromotionOptionType] FOREIGN KEY ([IdOptionType]) REFERENCES [PromotionOptionTypes]([Id]), 
		CONSTRAINT [FK_PromotionOptionValues_ToPromotion] FOREIGN KEY ([IdPromotion]) REFERENCES [Promotions]([Id])
	);
	
	CREATE INDEX [IX_PromotionOptionValues_Value] ON [dbo].[PromotionOptionValues] ([Value]) INCLUDE (Id, IdPromotion, IdOptionType)
END

GO

IF OBJECT_ID(N'[dbo].[PromotionsToBuySkus]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[PromotionsToBuySkus]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[IdPromotion] INT NOT NULL, 
		[IdSku] INT NOT NULL, 		
		[Quantity] INT NOT NULL, 
		CONSTRAINT [FK_PromotionsToBuySkus_ToPromotion] FOREIGN KEY ([IdPromotion]) REFERENCES [Promotions]([Id]), 
		CONSTRAINT [FK_PromotionsToBuySkus_ToSku] FOREIGN KEY ([IdSku]) REFERENCES [Skus]([Id])
	)
END

GO

IF OBJECT_ID(N'[dbo].[PromotionsToGetSkus]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[PromotionsToGetSkus]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[IdPromotion] INT NOT NULL, 
		[IdSku] INT NOT NULL, 		
		[Quantity] INT NOT NULL, 
		[Percent] DECIMAL(5,2) NOT NULL, 
		CONSTRAINT [FK_PromotionsToGetSkus_ToPromotion] FOREIGN KEY ([IdPromotion]) REFERENCES [Promotions]([Id]), 
		CONSTRAINT [FK_PromotionsToGetSkus_ToSku] FOREIGN KEY ([IdSku]) REFERENCES [Skus]([Id])
	)
END

GO