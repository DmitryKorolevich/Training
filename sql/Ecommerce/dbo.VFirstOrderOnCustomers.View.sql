/****** Object:  View [dbo].[VFirstOrderOnCustomers]    Script Date: 6/25/2016 2:13:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[VFirstOrderOnCustomers]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[VFirstOrderOnCustomers]
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

' 
GO
