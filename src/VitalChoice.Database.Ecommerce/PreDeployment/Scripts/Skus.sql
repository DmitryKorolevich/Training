IF OBJECT_ID('[dbo].[SkuOptionTypes]') IS NULL
BEGIN
	CREATE TABLE [dbo].[SkuOptionTypes](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Name] [nvarchar](50) NOT NULL,
		[IdFieldType] [int] NOT NULL,
		[IdLookup] [int] NULL,
		[IdObjectType] [int] NULL,
		[DefaultValue] [nvarchar](250) NULL,
	CONSTRAINT PK_SkuOptionTypes PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[SkuOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_SkuOptionTypes_ToFieldType] FOREIGN KEY([IdFieldType])
	REFERENCES [dbo].[FieldTypes] ([Id])

	ALTER TABLE [dbo].[SkuOptionTypes] CHECK CONSTRAINT [FK_SkuOptionTypes_ToFieldType]

	ALTER TABLE [dbo].[SkuOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_SkuOptionTypes_ToLookup] FOREIGN KEY([IdLookup])
	REFERENCES [dbo].[Lookups] ([Id])

	ALTER TABLE [dbo].[SkuOptionTypes] CHECK CONSTRAINT [FK_SkuOptionTypes_ToLookup]

	ALTER TABLE [dbo].[SkuOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_SkuOptionTypes_ToProductType] FOREIGN KEY([IdObjectType])
	REFERENCES [dbo].[ProductTypes] ([Id])

	ALTER TABLE [dbo].[SkuOptionTypes] CHECK CONSTRAINT [FK_SkuOptionTypes_ToProductType]

	ALTER TABLE [dbo].[SkuOptionValues]
	DROP CONSTRAINT [FK_SkuOptionValue_ToProductOptionType]

	CREATE NONCLUSTERED INDEX [IX_SkuOptionTypes_Name] ON [dbo].[SkuOptionTypes]
	(
		[Name] ASC
	)
	INCLUDE ( 	[Id],
		[IdFieldType],
		[IdObjectType],
		[IdLookup],
		[DefaultValue]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

	CREATE UNIQUE NONCLUSTERED INDEX [IX_UQ_NameTypeSkuOption] ON [dbo].[SkuOptionTypes]
	(
		[Name] ASC,
		[IdObjectType] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
END