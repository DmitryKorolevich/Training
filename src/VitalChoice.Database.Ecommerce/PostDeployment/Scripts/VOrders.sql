GO

IF OBJECT_ID(N'[dbo].[VOrders]', N'V') IS NOT NULL
	DROP VIEW [dbo].[VOrders]
GO
CREATE VIEW [dbo].[VOrders]
AS 
SELECT 
	o.Id,
	CAST(o.Id as nvarchar(20)) As IdString,
	o.OrderStatus,
	CAST(orderOptions.OrderType as int) As IdOrderSource,
	orderOptions.OrderType As SIdOrderSource,
	orderOptions.OrderNotes As OrderNotes,
	opm.IdObjectType As IdPaymentMethod,
	o.DateCreated,
	o.Total,
	o.IdEditedBy,
	o.DateEdited,
	CAST(orderOptions.POrderType as int) As POrderType,
	orderOptions.POrderType As SPOrderType,
	c.IdObjectType AS IdCustomerType,
	CASE WHEN orderOptions.ShippingUpgradeP IS NULL AND orderOptions.ShippingUpgradeNP IS NULL THEN 2 ELSE 1 END As IdShippingMethod,
	c.Id AS IdCustomer,
	options.Company,
	options.FirstName+' '+options.LastName As Customer,
	st.StateCode,
	shipOptions.FirstName+' '+shipOptions.LastName As ShipTo,
	CONVERT(bit,CASE WHEN ho.Id IS NULL THEN 0 ELSE 1 END) As Healthwise,
	CAST(orderOptions.[ShipDelayDate] as datetime2) As DateShipped,
	CAST(orderOptions.[ShipDelayDateP] as datetime2) As PDateShipped,
	CAST(orderOptions.[ShipDelayDateNP] as datetime2) As NPDateShipped
	FROM Orders AS o
	LEFT JOIN HealthwiseOrders AS ho ON ho.Id = o.Id
	LEFT JOIN OrderPaymentMethods AS opm ON opm.Id = o.IdPaymentMethod

	LEFT JOIN (SELECT [IdOrder], [OrderType], [OrderNotes], [POrderType], [ShippingUpgradeP], [ShippingUpgradeNP], [ShipDelayDate], [ShipDelayDateP], [ShipDelayDateNP]
	FROM (SELECT [IdOrder], [Name], [Value] FROM [dbo].[OrderOptionTypes] AS adt
	INNER JOIN [dbo].[OrderOptionValues] AS adv on adt.Id = adv.IdOptionType) As source
	PIVOT(
	MIN([Value]) FOR [Name] in ([OrderType], [OrderNotes], [POrderType], [ShippingUpgradeP], [ShippingUpgradeNP], [ShipDelayDate], [ShipDelayDateP], [ShipDelayDateNP])
	) AS piv) AS orderOptions ON o.Id = orderOptions.IdOrder

	JOIN Customers AS c ON c.Id = o.[IdCustomer]
	JOIN Addresses AS ad ON ad.Id = c.IdProfileAddress
	LEFT JOIN (SELECT [IdAddress], [FirstName], [LastName], [Company]
	FROM (SELECT [IdAddress], [Name], [Value] FROM [dbo].[AddressOptionTypes] AS adt
	INNER JOIN [dbo].[AddressOptionValues] AS adv on adt.Id = adv.IdOptionType ) AS source
	PIVOT(
	MIN([Value]) FOR [Name] in ([FirstName], [LastName], [City], [Company], [Address1], [Address2], [Phone], [Zip])
	) AS piv) AS options ON ad.Id = options.IdAddress
	LEFT JOIN OrderAddresses AS shAd ON shAd.Id = o.IdShippingAddress AND shAd.IdObjectType = 3--Shipping
	LEFT JOIN (SELECT [IdOrderAddress], [FirstName], [LastName], [Company]
	FROM (SELECT [IdOrderAddress], [Name], [Value] FROM [dbo].[AddressOptionTypes] AS adt
	INNER JOIN [dbo].[OrderAddressOptionValues] AS adv on adt.Id = adv.IdOptionType ) AS source
	PIVOT(
	MIN([Value]) FOR [Name] in ([FirstName], [LastName], [City], [Company], [Address1], [Address2], [Phone], [Zip])
	) AS piv) AS shipOptions ON shAd.Id = shipOptions.IdOrderAddress
	LEFT OUTER JOIN [dbo].[States] AS st ON shAd.IdState = st.Id
	WHERE o.StatusCode!=3

GO


