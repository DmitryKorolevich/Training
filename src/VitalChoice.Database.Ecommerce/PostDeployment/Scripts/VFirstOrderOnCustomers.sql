IF OBJECT_ID(N'dbo.VFirstOrderOnCustomers') IS NOT NULL
BEGIN
	DROP VIEW [dbo].[VFirstOrderOnCustomers]
END
GO
CREATE VIEW [dbo].[VFirstOrderOnCustomers]
WITH SCHEMABINDING
AS 
	SELECT o.IdCustomer, MIN(o.DateCreated) DateCreated
	FROM dbo.Orders AS o
	INNER JOIN dbo.Customers AS c ON o.IdCustomer = c.Id
	WHERE o.StatusCode!=3 AND o.IdObjectType NOT IN (2,5,6) AND 
	((o.OrderStatus IS NOT NULL AND o.OrderStatus !=1 AND o.OrderStatus !=4 AND o.OrderStatus !=6 ) OR 
	(o.OrderStatus IS NULL AND o.POrderStatus !=1 AND o.POrderStatus !=4 AND o.POrderStatus !=6 AND 
	o.NPOrderStatus !=1 AND o.NPOrderStatus !=4 AND o.NPOrderStatus !=6))
	GROUP BY o.IdCustomer

GO