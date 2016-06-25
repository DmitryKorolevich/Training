/****** Object:  View [dbo].[VOrderCountOnCustomers]    Script Date: 6/25/2016 2:13:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[VOrderCountOnCustomers]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[VOrderCountOnCustomers]
WITH SCHEMABINDING
AS 
	SELECT o.IdCustomer, COUNT_BIG(*) AS [Count]
	FROM dbo.Orders AS o
	INNER JOIN dbo.Customers AS c ON o.IdCustomer = c.Id
	WHERE o.StatusCode!=3 AND o.IdObjectType NOT IN (2,5,6) AND 
	((o.OrderStatus IS NOT NULL AND o.OrderStatus !=1 AND o.OrderStatus !=4 AND o.OrderStatus !=6 ) OR 
	(o.OrderStatus IS NULL AND o.POrderStatus !=1 AND o.POrderStatus !=4 AND o.POrderStatus !=6 AND 
	o.NPOrderStatus !=1 AND o.NPOrderStatus !=4 AND o.NPOrderStatus !=6))
	GROUP BY o.IdCustomer

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
/****** Object:  Index [IX_VOrderCountOnCustomers_IdCustomer]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[VOrderCountOnCustomers]') AND name = N'IX_VOrderCountOnCustomers_IdCustomer')
CREATE UNIQUE CLUSTERED INDEX [IX_VOrderCountOnCustomers_IdCustomer] ON [dbo].[VOrderCountOnCustomers]
(
	[IdCustomer] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
