IF NOT EXISTS(SELECT * FROM SkuOptionTypes)
BEGIN
	INSERT INTO SkuOptionTypes
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	SELECT [Name], [IdFieldType], [IdLookup], NULL, [DefaultValue] FROM ProductOptionTypes AS t
	WHERE t.Name IN ('OffPercent',
	'Stock',
	'AutoShipFrequency1',
	'AutoShipFrequency2',
	'AutoShipFrequency3',
	'AutoShipFrequency6',
	'AutoShipProduct',
	'DisallowSingle',
	'HideFromDataFeed',
	'NonDiscountable',
	'OrphanType',
	'DisregardStock',
	'Seller',
	'QTY',
	'QTYThreshold',
	'SalesText',
	'InventorySkuChannel',
	'Assemble',
	'BornDate')
	GROUP BY t.Name, [IdFieldType], [IdLookup], [DefaultValue]
	HAVING COUNT(t.Name) = 4

	INSERT INTO SkuOptionTypes
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	SELECT [Name], [IdFieldType], [IdLookup], t.IdObjectType, [DefaultValue] FROM ProductOptionTypes AS t
	WHERE t.Name IN ('OffPercent',
	'Stock',
	'AutoShipFrequency1',
	'AutoShipFrequency2',
	'AutoShipFrequency3',
	'AutoShipFrequency6',
	'AutoShipProduct',
	'DisallowSingle',
	'OrphanType',
	'DisregardStock',
	'QTYThreshold',
	'SalesText',
	'InventorySkuChannel',
	'Assemble',
	'BornDate')

	DELETE v
	FROM SkuOptionValues AS v
	INNER JOIN Skus AS s ON s.Id = v.IdSku
	INNER JOIN Products AS p ON p.Id = s.IdProduct
	INNER JOIN ProductOptionTypes AS t ON t.Name NOT IN (SELECT Name FROM SkuOptionTypes) AND t.Id = v.IdOptionType

	UPDATE SkuOptionValues
	SET IdOptionType = st.Id
	FROM SkuOptionValues AS v
	INNER JOIN Skus AS s ON s.Id = v.IdSku
	INNER JOIN Products AS p ON p.Id = s.IdProduct
	INNER JOIN ProductOptionTypes AS t ON t.Id = v.IdOptionType
	INNER JOIN SkuOptionTypes AS st ON st.Name = t.Name AND (st.IdObjectType = p.IdObjectType OR st.IdObjectType IS NULL)

	ALTER TABLE [dbo].[SkuOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_SkuOptionValue_ToSkuOptionType] FOREIGN KEY([IdOptionType])
	REFERENCES [dbo].[SkuOptionTypes] ([Id])

	ALTER TABLE [dbo].[SkuOptionValues] CHECK CONSTRAINT [FK_SkuOptionValue_ToSkuOptionType]

	DELETE v
	FROM ProductOptionValues AS v
	INNER JOIN ProductOptionTypes AS t ON t.Id = v.IdOptionType
	WHERE t.Name IN (SELECT Name FROM SkuOptionTypes)

	DELETE FROM ProductOptionTypes
	WHERE Name IN (SELECT Name FROM SkuOptionTypes)

	INSERT INTO SkuOptionValues
	(IdOptionType, IdSku, Value)
	SELECT t.Id, s.Id, t.DefaultValue FROM Skus AS s
	INNER JOIN Products AS p ON p.Id = s.IdProduct
	INNER JOIN SkuOptionTypes AS t ON t.IdObjectType = p.IdObjectType OR t.IdObjectType IS NULL
	WHERE t.DefaultValue IS NOT NULL AND NOT EXISTS(SELECT * FROM SkuOptionValues AS v WHERE v.IdOptionType = t.Id AND v.IdSku = s.Id)
END