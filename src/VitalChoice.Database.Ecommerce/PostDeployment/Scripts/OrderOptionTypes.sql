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