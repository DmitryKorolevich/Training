IF NOT EXISTS(SELECT * FROM OrderOptionTypes WHERE IdObjectType=3 AND Name='ShippingOverride')
BEGIN

	INSERT INTO [dbo].[OrderOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'ShippingUpgradeP', 3, (SELECT TOP 1 Id FROM Lookups WHERE Name='ShippingUpgrade'), 3, NULL),
	(N'ShippingUpgradeNP', 3, (SELECT TOP 1 Id FROM Lookups WHERE Name='ShippingUpgrade'), 3, NULL),
	(N'ShippingOverride', 1, NULL, 3, NULL),
	(N'GiftOrder', 5, NULL, 3, NULL)

END
GO

IF NOT EXISTS(SELECT * FROM OrderOptionTypes WHERE IdObjectType=5 AND Name='GiftMessage')
BEGIN

	INSERT INTO [dbo].[OrderOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'GiftMessage', 4, NULL, 5, NULL),
	(N'GiftOrder', 5, NULL, 5, NULL)

END
GO

IF NOT EXISTS(SELECT * FROM OrderOptionTypes WHERE IdObjectType=5 AND Name='ServiceCodeNotes')
BEGIN

	INSERT INTO [dbo].[OrderOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'ServiceCodeNotes', 4, NULL, 5, NULL),
	(N'ServiceCodeNotes', 4, NULL, 6, NULL)

	INSERT OrderOptionValues
	(
		IdOptionType,
		IdOrder,
		Value
	)
	(
		SELECT 
			sc.Id,
			o.Id,
			v.Value
		FROM Orders o 
		JOIN OrderOptionTypes t ON t.Name='OrderNotes' AND t.IdObjectType=o.IdObjectType
		JOIN OrderOptionValues v ON v.IdOptionType=t.Id AND v.IdOrder=o.Id
		JOIN OrderOptionTypes sc ON sc.Name='ServiceCodeNotes' AND sc.IdObjectType=o.IdObjectType
		WHERE o.IdObjectType IN (5,6)
	)

END
GO