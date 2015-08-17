GO

IF OBJECT_ID(N'[dbo].[VOrders]', N'V') IS NOT NULL
	DROP VIEW [dbo].[VOrders]
GO
CREATE VIEW [dbo].[VOrders]
AS 
SELECT 
	o.Id,
	o.OrderStatus,
	oval.Value As IdOrderSource,
	oval.Value As OrderNotes,
	o.IdPaymentMethod,
	o.DateCreated,
	NULL As DateShipped,
	o.Total,
	o.IdEditedBy,
	o.DateEdited,
	NULL As  POrderType,
	c.IdObjectType AS IdCustomerType,
	NULL As IdShippingMethod,
	c.Id AS IdCustomer,
	options.Company,
	options.FirstName+' '+options.LastName As Customer,
	st.StateCode
	FROM Orders AS o
	LEFT JOIN OrderOptionTypes AS oopt ON oopt.Name = N'OrderSource' AND oopt.IdObjectType = o.IdObjectType
	LEFT JOIN OrderOptionValues AS oval ON oval.IdOrder = o.Id AND oval.IdOptionType = oopt.Id
	LEFT JOIN OrderOptionTypes AS onopt ON onopt.Name = N'OrderNotes' AND onopt.IdObjectType = o.IdObjectType
	LEFT JOIN OrderOptionValues AS onval ON onval.IdOrder = o.Id AND onval.IdOptionType = onopt.Id
	JOIN Customers AS c ON c.Id = o.[IdCustomer]
	JOIN Addresses AS ad ON ad.IdCustomer = c.Id
	LEFT JOIN (SELECT [IdAddress], [FirstName], [LastName], [Company]
	FROM (SELECT [IdAddress], [Name], [Value] FROM [dbo].[AddressOptionTypes] AS adt
	INNER JOIN [dbo].[AddressOptionValues] AS adv on adt.Id = adv.IdOptionType ) AS source
	PIVOT(
	MIN([Value]) FOR [Name] in ([FirstName], [LastName], [City], [Company], [Address1], [Address2], [Phone], [Zip])
	) AS piv) AS options ON ad.Id = options.IdAddress
	LEFT OUTER JOIN [dbo].[States] AS st ON ad.IdState = st.Id
	WHERE o.StatusCode!=3


GO


