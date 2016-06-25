/****** Object:  View [dbo].[VTopProducts]    Script Date: 6/25/2016 2:13:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[VTopProducts]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[VTopProducts]
WITH SCHEMABINDING
AS 
	SELECT s.IdSku, SUM(s.Quantity) AS [Count], o.IdCustomer, COUNT_BIG(*) TotalCount
	FROM dbo.OrderToSkus AS s
	INNER JOIN dbo.Orders AS o ON o.Id = s.IdOrder
	GROUP BY o.IdCustomer, s.IdSku

' 
GO
SET ARITHABORT ON
SET CONCAT_NULL_YIELDS_NULL ON
SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
SET NUMERIC_ROUNDABORT OFF

GO
/****** Object:  Index [IX_TopProductsCustomerToSku]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[VTopProducts]') AND name = N'IX_TopProductsCustomerToSku')
CREATE UNIQUE CLUSTERED INDEX [IX_TopProductsCustomerToSku] ON [dbo].[VTopProducts]
(
	[IdSku] ASC,
	[IdCustomer] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ARITHABORT ON
SET CONCAT_NULL_YIELDS_NULL ON
SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
SET NUMERIC_ROUNDABORT OFF

GO
/****** Object:  Index [IX_TopProductsIdCustomer]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[VTopProducts]') AND name = N'IX_TopProductsIdCustomer')
CREATE NONCLUSTERED INDEX [IX_TopProductsIdCustomer] ON [dbo].[VTopProducts]
(
	[IdCustomer] DESC,
	[Count] DESC
)
INCLUDE ( 	[IdSku]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
