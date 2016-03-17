﻿IF OBJECT_ID(N'dbo.VTopProducts') IS NOT NULL
BEGIN
	DROP VIEW [dbo].[VTopProducts]
END
GO
CREATE VIEW [dbo].[VTopProducts]
WITH SCHEMABINDING
AS 
	SELECT s.IdSku, SUM(s.Quantity) AS [Count], o.IdCustomer, COUNT_BIG(*) TotalCount
	FROM dbo.OrderToSkus AS s
	INNER JOIN dbo.Orders AS o ON o.Id = s.IdOrder
	GROUP BY o.IdCustomer, s.IdSku

GO
CREATE UNIQUE CLUSTERED INDEX [IX_TopProductsCustomerToSku] ON [dbo].[VTopProducts]
(
	[IdSku] ASC,
	[IdCustomer] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

CREATE NONCLUSTERED INDEX [IX_TopProductsIdCustomer] ON [dbo].[VTopProducts]
(
	[IdCustomer] DESC,
	[Count] DESC
) INCLUDE 
(
	[IdSku]
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO