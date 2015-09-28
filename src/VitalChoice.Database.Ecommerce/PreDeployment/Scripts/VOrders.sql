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
	onval.Value As OrderNotes,
	opm.IdObjectType As IdPaymentMethod,
	o.DateCreated,
	NULL As DateShipped,
	o.Total,
	o.IdEditedBy,
	o.DateEdited,
	CAST(opval.Value as int) As POrderType,
	c.IdObjectType AS IdCustomerType,
	NULL As IdShippingMethod,
	c.Id AS IdCustomer,
	options.Company,
	options.FirstName+' '+options.LastName As Customer,
	st.StateCode,
	shipOptions.FirstName+' '+shipOptions.LastName As ShipTo
	FROM Orders AS o
	LEFT JOIN OrderPaymentMethods AS opm ON opm.Id = o.IdPaymentMethod
	LEFT JOIN OrderOptionTypes AS oopt ON oopt.Name = N'OrderType' AND (oopt.IdObjectType = o.IdObjectType OR oopt.IdObjectType IS NULL)
	LEFT JOIN OrderOptionValues AS oval ON oval.IdOrder = o.Id AND oval.IdOptionType = oopt.Id
	LEFT JOIN OrderOptionTypes AS onopt ON onopt.Name = N'OrderNotes' AND (onopt.IdObjectType = o.IdObjectType OR onopt.IdObjectType IS NULL)
	LEFT JOIN OrderOptionValues AS onval ON onval.IdOrder = o.Id AND onval.IdOptionType = onopt.Id
	LEFT JOIN OrderOptionTypes AS opopt ON opopt.Name = N'POrderType' AND (opopt.IdObjectType = o.IdObjectType OR opopt.IdObjectType IS NULL)
	LEFT JOIN OrderOptionValues AS opval ON opval.IdOrder = o.Id AND opval.IdOptionType = opopt.Id
	JOIN Customers AS c ON c.Id = o.[IdCustomer]
	JOIN Addresses AS ad ON ad.IdCustomer = c.Id AND ad.IdObjectType = 1--Profile
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


