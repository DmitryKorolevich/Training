IF OBJECT_ID(N'dbo.InventorySkuCategories', N'U') IS NULL
BEGIN

CREATE TABLE [dbo].[InventorySkuCategories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[ParentId] [int] NULL,
	[StatusCode] [int] NOT NULL DEFAULT ((1)),
	[Order] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[InventorySkuCategories]  WITH CHECK ADD  CONSTRAINT [FK_InventorySkuCategories_RecordStatusCodes] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])

ALTER TABLE [dbo].[InventorySkuCategories] CHECK CONSTRAINT [FK_InventorySkuCategories_RecordStatusCodes]

CREATE NONCLUSTERED INDEX [IX_InventorySkuCategories_ParentId] ON [dbo].[InventoryCategories]
(
	[ParentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80)

END

GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE [object_id] = OBJECT_ID(N'[dbo].[LookupVariants]', N'U') AND Name = 'Order')
BEGIN
	ALTER TABLE [LookupVariants]
	ADD [Order] INT NOT NULL DEFAULT(1)

	ALTER TABLE [Lookups]
	ADD [Description] NVARCHAR(250) NULL
END

GO
IF OBJECT_ID(N'dbo.InventorySkuCategories', N'U') IS NULL
BEGIN

CREATE TABLE [dbo].[InventorySkuCategories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[ParentId] [int] NULL,
	[StatusCode] [int] NOT NULL DEFAULT ((1)),
	[Order] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[InventorySkuCategories]  WITH CHECK ADD  CONSTRAINT [FK_InventorySkuCategories_RecordStatusCodes] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])

ALTER TABLE [dbo].[InventorySkuCategories] CHECK CONSTRAINT [FK_InventorySkuCategories_RecordStatusCodes]

CREATE NONCLUSTERED INDEX [IX_InventorySkuCategories_ParentId] ON [dbo].[InventoryCategories]
(
	[ParentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80)

END

GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE [object_id] = OBJECT_ID(N'[dbo].[LookupVariants]', N'U') AND Name = 'Order')
BEGIN
	ALTER TABLE [LookupVariants]
	ADD [Order] INT NOT NULL DEFAULT(1)

	ALTER TABLE [Lookups]
	ADD [Description] NVARCHAR(250) NULL
END

GO

IF OBJECT_ID(N'[dbo].[InventorySkus]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[InventorySkus]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[DateCreated] DATETIME2 NOT NULL DEFAULT SYSDATETIME(), 
		[DateEdited] DATETIME2 NOT NULL DEFAULT SYSDATETIME(), 
		[IdEditedBy] [int] NULL
			CONSTRAINT FK_InventorySkusToUser FOREIGN KEY (IdEditedBy) REFERENCES dbo.Users (Id),
		[StatusCode] INT NOT NULL DEFAULT 1,  
		[Code] NVARCHAR(20) NOT NULL, 
		[Description] NVARCHAR(250) NOT NULL, 
		[IdInventorySkuCategory] INT NULL, 
		CONSTRAINT [FK_InventorySku_ToInventorySkuCategory] FOREIGN KEY ([IdInventorySkuCategory]) REFERENCES [InventorySkuCategories]([Id]), 
		CONSTRAINT [FK_InventorySkus_ToRecordStatusCode] FOREIGN KEY ([StatusCode]) REFERENCES [RecordStatusCodes] ([StatusCode])
	);

	CREATE TABLE [dbo].[InventorySkuOptionTypes]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[Name] NVARCHAR(50) NOT NULL, 
		[IdFieldType] INT NOT NULL, 
		[IdLookup] INT NULL, 
		[IdObjectType] INT NULL, 
		[DefaultValue] NVARCHAR(250) NULL,
		CONSTRAINT [FK_InventorySkuOptionType_ToLookup] FOREIGN KEY ([IdLookup]) REFERENCES [Lookups]([Id]), 
		CONSTRAINT [FK_InventorySkuOptionType_ToFieldType] FOREIGN KEY ([IdFieldType]) REFERENCES [FieldTypes]([Id])
	);

	CREATE TABLE [dbo].[InventorySkuOptionValues]
	(
		[IdInventorySku] INT NOT NULL, 
		[IdOptionType] INT NOT NULL, 
		[Value] NVARCHAR(250) NULL,
		CONSTRAINT [PK_InventorySkuOptionValues] PRIMARY KEY CLUSTERED 
		(
			[IdInventorySku] ASC,
			[IdOptionType] ASC
		),
		CONSTRAINT [FK_InventorySkuOptionValue_InventorySkuOptionType] FOREIGN KEY ([IdOptionType]) REFERENCES [InventorySkuOptionTypes]([Id]), 
		CONSTRAINT [FK_InventorySkuOptionValue_ToInventorySku] FOREIGN KEY ([IdInventorySku]) REFERENCES [InventorySkus]([Id]), 
	);

	CREATE INDEX [IX_InventorySkuOptionValue_Value] ON [dbo].[InventorySkuOptionValues] ([Value])

	CREATE INDEX [IX_InventorySkuOptionValue_ValuesSearch] ON [dbo].[InventorySkuOptionValues] ([Value], [IdOptionType]) 
	INCLUDE (IdInventorySku)

	CREATE TABLE [dbo].[SkusToInventorySkus]
	(
		[IdSku] INT NOT NULL, 
		[IdInventorySku] INT NOT NULL, 
		CONSTRAINT [PK_SkusToInventorySkus] PRIMARY KEY CLUSTERED 
		(
			[IdSku] ASC,
			[IdInventorySku] ASC
		),
		CONSTRAINT [FK_SkusToInventorySku_ToSku] FOREIGN KEY ([IdSku]) REFERENCES [Skus]([Id]), 
		CONSTRAINT [FK_SkusToInventorySku_ToInventorySku] FOREIGN KEY ([IdInventorySku]) REFERENCES [InventorySkus]([Id]), 
	);

	CREATE TABLE [dbo].[OrderToSkusToInventorySkus]
	(
		[IdOrder] INT NOT NULL, 
		[IdSku] INT NOT NULL, 
		[IdInventorySku] INT NOT NULL, 
		CONSTRAINT [PK_OrderToSkusToInventorySkus] PRIMARY KEY CLUSTERED 
		(
			[IdOrder] ASC,
			[IdSku] ASC,
			[IdInventorySku] ASC
		),
		CONSTRAINT [FK_OrderToSkuToInventorySku_ToSku] FOREIGN KEY ([IdOrder],[IdSku]) REFERENCES [OrderToSkus]([IdOrder],[IdSku]) ON DELETE CASCADE, 
		CONSTRAINT [FK_OrderToSkuToInventorySku_ToInventorySku] FOREIGN KEY ([IdInventorySku]) REFERENCES [InventorySkus]([Id]), 
	);

	CREATE TABLE [dbo].[OrderToPromosToInventorySkus]
	(
		[IdOrder] INT NOT NULL, 
		[IdSku] INT NOT NULL, 
		[IdInventorySku] INT NOT NULL, 
		CONSTRAINT [PK_OrderToPromosToInventorySkus] PRIMARY KEY CLUSTERED 
		(
			[IdOrder] ASC,
			[IdSku] ASC,
			[IdInventorySku] ASC
		),
		CONSTRAINT [FK_OrderToPromoToInventorySku_ToSku] FOREIGN KEY ([IdOrder],[IdSku]) REFERENCES [OrderToPromos]([IdOrder],[IdSku]) ON DELETE CASCADE, 
		CONSTRAINT [FK_OrderToPromoToInventorySku_ToInventorySku] FOREIGN KEY ([IdInventorySku]) REFERENCES [InventorySkus]([Id]), 
	);

END

GO