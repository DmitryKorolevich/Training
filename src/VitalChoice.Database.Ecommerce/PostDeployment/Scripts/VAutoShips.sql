GO

IF OBJECT_ID(N'[dbo].[VAutoShips]', N'V') IS NOT NULL
	DROP VIEW [dbo].[VAutoShips]
GO
CREATE VIEW [dbo].[VAutoShips]
AS 
SELECT        o.[Id], CAST([AutoShipFrequency] AS int) AS [AutoShipFrequency], CAST( [LastAutoShipDate] AS datetime2) AS [LastAutoShipDate]
FROM            dbo.Orders AS o 
	LEFT JOIN (
		SELECT [IdOrder], [AutoShipFrequency], [LastAutoShipDate]
		FROM (
			SELECT [IdOrder], [Name], [Value]
			FROM [dbo].[OrderOptionTypes] AS adt 
			INNER JOIN [dbo].[OrderOptionValues] AS adv ON adt.Id = adv.IdOptionType
		) AS source 
		PIVOT (MIN([Value]) FOR [Name] IN ([AutoShipFrequency], [LastAutoShipDate])) AS piv
	) AS orderOptions ON o.Id = orderOptions.IdOrder
WHERE o.IdObjectType = 2 AND o.StatusCode = 2 AND 
(
	o.OrderStatus IS NOT NULL AND o.OrderStatus NOT IN (1, 6) OR 
	o.POrderStatus IS NOT NULL AND o.POrderStatus NOT IN (1, 6) OR
	o.NPOrderStatus IS NOT NULL AND o.NPOrderStatus NOT IN (1, 6)
)
GO


IF OBJECT_ID(N'[dbo].[VAutoShipOrders]', N'V') IS NOT NULL
	DROP VIEW [dbo].[VAutoShipOrders]
GO
CREATE VIEW [dbo].[VAutoShipOrders]
AS 
SELECT o.[Id], CAST(adv.Value AS int) as [AutoShipId]
FROM dbo.Orders AS o 
LEFT JOIN  [dbo].[OrderOptionValues] AS adv ON adv.IdOrder = o.Id
INNER JOIN [dbo].[OrderOptionTypes] AS adt ON adt.Id = adv.IdOptionType AND adt.Name = 'AutoShipId'
WHERE  o.IdObjectType = 7 AND o.StatusCode = 2 AND (o.OrderStatus IS NOT NULL AND o.OrderStatus <> 1 OR
                         o.POrderStatus IS NOT NULL AND o.POrderStatus <> 1 OR
                         o.NPOrderStatus IS NOT NULL AND o.NPOrderStatus <> 1)
GO


