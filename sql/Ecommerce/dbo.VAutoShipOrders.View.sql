/****** Object:  View [dbo].[VAutoShipOrders]    Script Date: 6/25/2016 2:13:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[VAutoShipOrders]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[VAutoShipOrders]
AS 
SELECT        o.[Id], CAST([AutoShipId] AS int) AS [AutoShipId]
FROM            dbo.Orders AS o LEFT JOIN
                             (SELECT        [IdOrder], [AutoShipId]
                               FROM            (SELECT        [IdOrder], [Name], [Value]
                                                         FROM            [dbo].[OrderOptionTypes] AS adt INNER JOIN
                                                                                   [dbo].[OrderOptionValues] AS adv ON adt.Id = adv.IdOptionType) AS source PIVOT (MIN([Value]) FOR [Name] IN ([AutoShipId])) AS piv) AS orderOptions ON 
                         o.Id = orderOptions.IdOrder
WHERE        o.IdObjectType = 7 AND o.StatusCode = 2 AND (o.OrderStatus IS NOT NULL AND o.OrderStatus <> 1 OR o.POrderStatus IS NOT NULL AND o.POrderStatus <> 1 OR o.NPOrderStatus IS NOT NULL AND o.NPOrderStatus <> 1)
' 
GO
