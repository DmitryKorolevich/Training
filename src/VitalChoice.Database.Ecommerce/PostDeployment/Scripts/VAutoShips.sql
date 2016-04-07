GO

IF OBJECT_ID(N'[dbo].[VAutoShips]', N'V') IS NOT NULL
	DROP VIEW [dbo].[VAutoShips]
GO
CREATE VIEW [dbo].[VAutoShips]
AS 
SELECT        o.[Id], CAST([AutoShipFrequency] AS int) AS [AutoShipFrequency], CAST( [LastAutoShipDate] AS datetime) AS [LastAutoShipDate]
FROM            dbo.Orders AS o LEFT JOIN
                             (SELECT        [IdOrder], [AutoShipFrequency], [LastAutoShipDate]
                               FROM            (SELECT        [IdOrder], [Name], [Value]
                                                         FROM            [dbo].[OrderOptionTypes] AS adt INNER JOIN
                                                                                   [dbo].[OrderOptionValues] AS adv ON adt.Id = adv.IdOptionType) AS source PIVOT (MIN([Value]) FOR [Name] IN ([AutoShipFrequency], [LastAutoShipDate])) AS piv) AS orderOptions ON 
                         o.Id = orderOptions.IdOrder
WHERE        o.IdObjectType = 2 AND o.StatusCode = 2 AND (NOT (o.OrderStatus = 1 OR
                         o.POrderStatus = 1 OR
                         o.NPOrderStatus = 1) OR
                         (o.POrderStatus IS NULL OR
                         o.NPOrderStatus IS NULL))
GO


IF OBJECT_ID(N'[dbo].[VAutoShipOrders]', N'V') IS NOT NULL
	DROP VIEW [dbo].[VAutoShipOrders]
GO
CREATE VIEW [dbo].[VAutoShipOrders]
AS 
SELECT        o.[Id], CAST([AutoShipId] AS int) AS [AutoShipId]
FROM            dbo.Orders AS o LEFT JOIN
                             (SELECT        [IdOrder], [AutoShipId]
                               FROM            (SELECT        [IdOrder], [Name], [Value]
                                                         FROM            [dbo].[OrderOptionTypes] AS adt INNER JOIN
                                                                                   [dbo].[OrderOptionValues] AS adv ON adt.Id = adv.IdOptionType) AS source PIVOT (MIN([Value]) FOR [Name] IN ([AutoShipId])) AS piv) AS orderOptions ON 
                         o.Id = orderOptions.IdOrder
WHERE        o.IdObjectType = 7 AND o.StatusCode = 2 AND (NOT (o.OrderStatus = 1 OR
                         o.POrderStatus = 1 OR
                         o.NPOrderStatus = 1) OR
                         (o.POrderStatus IS NULL OR
                         o.NPOrderStatus IS NULL))
GO


