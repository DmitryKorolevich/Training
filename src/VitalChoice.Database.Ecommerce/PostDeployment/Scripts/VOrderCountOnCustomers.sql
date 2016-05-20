IF OBJECT_ID(N'dbo.VOrderCountOnCustomers') IS NOT NULL
BEGIN
	DROP VIEW [dbo].[VOrderCountOnCustomers]
END
GO
CREATE VIEW [dbo].[VOrderCountOnCustomers]
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

GO

CREATE UNIQUE CLUSTERED INDEX [IX_VOrderCountOnCustomers_IdCustomer] ON [dbo].[VOrderCountOnCustomers]
(
	[IdCustomer] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO