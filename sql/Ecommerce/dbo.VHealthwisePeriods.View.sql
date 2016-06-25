/****** Object:  View [dbo].[VHealthwisePeriods]    Script Date: 6/25/2016 2:13:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[VHealthwisePeriods]'))
EXEC dbo.sp_executesql @statement = N'
CREATE VIEW [dbo].[VHealthwisePeriods]
AS
SELECT hp.Id,hp.StartDate,hp.EndDate, hp.PaidAmount, hp.PaidDate, hp.IdCustomer, 
	   options.FirstName AS CustomerFirstName, options.LastName AS CustomerLastName, c.Email As CustomerEmail,
	   ISNULL(ho.Count,0) As OrdersCount, ISNULL(ho.DiscountedSubtotal,0) As DiscountedSubtotal, ho.LastOrderDate LastOrderDate
FROM
	dbo.HealthwisePeriods hp
LEFT JOIN (SELECT ho.IdHealthwisePeriod, COUNT(ho.IdHealthwisePeriod) As Count, SUM(o.ProductsSubtotal - o.DiscountTotal) DiscountedSubtotal, MAX(o.DateCreated) As LastOrderDate
	FROM dbo.HealthwiseOrders ho
	INNER JOIN dbo.Orders o ON ho.Id=o.Id
	WHERE (o.OrderStatus IN (2,3,5) OR o.POrderStatus IN (2,3,5) OR o.NPOrderStatus IN (2,3,5)) AND o.StatusCode!=3
	GROUP BY ho.IdHealthwisePeriod) ho ON ho.IdHealthwisePeriod=hp.Id
INNER JOIN dbo.Customers c ON hp.IdCustomer=c.Id
INNER JOIN [dbo].[Addresses] AS ad ON c.IdProfileAddress = ad.Id
INNER JOIN (SELECT
	[IdAddress],
	[FirstName],
	[LastName]
FROM (SELECT [IdAddress],[Name],[Value] FROM [dbo].[AddressOptionTypes] AS adt
INNER JOIN [dbo].[AddressOptionValues] AS adv ON adt.Id = adv.IdOptionType) AS source
PIVOT (
MIN([Value]) FOR [Name] IN ([FirstName], [LastName])
) AS piv) AS options ON ad.Id = options.IdAddress
WHERE c.StatusCode!=3

' 
GO
