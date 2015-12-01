GO

IF OBJECT_ID(N'[dbo].[VOrders]', N'V') IS NOT NULL
	DROP VIEW [dbo].[VOrders]
GO
CREATE VIEW [dbo].[VOrders]
AS 
SELECT 
	o.Id,
	o.OrderStatus,
	CAST(oval.Value as int) As IdOrderSource,
	oval.Value As SIdOrderSource,
	onval.Value As OrderNotes,
	opm.IdObjectType As IdPaymentMethod,
	o.DateCreated,
	CAST(NULL as datetime2) As DateShipped,
	o.Total,
	o.IdEditedBy,
	o.DateEdited,
	CAST(opval.Value as int) As POrderType,
	opval.Value As SPOrderType,
	c.IdObjectType AS IdCustomerType,
	CASE WHEN sppval.Value IS NULL AND snpval.Value IS NULL THEN 2 ELSE 1 END As IdShippingMethod,
	c.Id AS IdCustomer,
	options.Company,
	options.FirstName+' '+options.LastName As Customer,
	st.StateCode,
	shipOptions.FirstName+' '+shipOptions.LastName As ShipTo,
	CONVERT(bit,CASE WHEN ho.Id IS NULL THEN 0 ELSE 1 END) As Healthwise
	FROM Orders AS o
	LEFT JOIN HealthwiseOrders AS ho ON ho.Id = o.Id
	LEFT JOIN OrderPaymentMethods AS opm ON opm.Id = o.IdPaymentMethod
	LEFT JOIN OrderOptionTypes AS oopt ON oopt.Name = N'OrderType' AND (oopt.IdObjectType = o.IdObjectType OR oopt.IdObjectType IS NULL)
	LEFT JOIN OrderOptionValues AS oval ON oval.IdOrder = o.Id AND oval.IdOptionType = oopt.Id
	LEFT JOIN OrderOptionTypes AS onopt ON onopt.Name = N'OrderNotes' AND (onopt.IdObjectType = o.IdObjectType OR onopt.IdObjectType IS NULL)
	LEFT JOIN OrderOptionValues AS onval ON onval.IdOrder = o.Id AND onval.IdOptionType = onopt.Id
	LEFT JOIN OrderOptionTypes AS opopt ON opopt.Name = N'POrderType' AND (opopt.IdObjectType = o.IdObjectType OR opopt.IdObjectType IS NULL)
	LEFT JOIN OrderOptionValues AS opval ON opval.IdOrder = o.Id AND opval.IdOptionType = opopt.Id
	LEFT JOIN OrderOptionTypes AS sppopt ON sppopt.Name = N'ShippingUpgradeP' AND (sppopt.IdObjectType = o.IdObjectType OR sppopt.IdObjectType IS NULL)
	LEFT JOIN OrderOptionValues AS sppval ON sppval.IdOrder = o.Id AND sppval.IdOptionType = sppopt.Id
	LEFT JOIN OrderOptionTypes AS snpopt ON snpopt.Name = N'ShippingUpgradeNP' AND (snpopt.IdObjectType = o.IdObjectType OR snpopt.IdObjectType IS NULL)
	LEFT JOIN OrderOptionValues AS snpval ON snpval.IdOrder = o.Id AND snpval.IdOptionType = snpopt.Id
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


