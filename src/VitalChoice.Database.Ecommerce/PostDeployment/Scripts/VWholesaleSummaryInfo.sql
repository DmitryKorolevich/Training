IF OBJECT_ID(N'[dbo].[VWholesaleSummaryInfo]', N'V') IS NOT NULL
	DROP VIEW [dbo].[VWholesaleSummaryInfo]
GO
CREATE VIEW [dbo].[VWholesaleSummaryInfo]
AS 
 
	SELECT 
		c.Id, c.DateCreated, CAST(CASE WHEN c.DateCreated>=DATEADD(mm,-12,GETDATE()) THEN 1 ELSE 0 END as BIT) as NewCustomer,
		CAST(val.Value as INT) as TradeClass, 
		CAST((CASE WHEN EXISTS(
		SELECT TOP 1 Id 
			FROM Orders o  WITH(NOLOCK) WHERE o.IdCustomer=c.Id AND
				o.DateCreated>=DATEADD(mm,-12,GETDATE()) AND
				o.StatusCode!=3 AND o.IdObjectType!=5 AND o.IdObjectType!=6 AND o.IdObjectType!=2 AND 
				((o.OrderStatus IS NOT NULL AND o.OrderStatus!=1 AND o.OrderStatus!=4) OR 
				(o.OrderStatus IS NULL AND o.POrderStatus!=1 AND o.POrderStatus!=4 AND
				o.NPOrderStatus!=1 AND o.NPOrderStatus!=4))
		) THEN 1 ELSE 0 END) as bit) as OrdersExist
	FROM Customers c WITH(NOLOCK)
	LEFT JOIN CustomerOptionTypes AS opt WITH(NOLOCK) ON opt.Name = N'TradeClass' AND opt.IdObjectType = 2
	LEFT JOIN CustomerOptionValues AS val WITH(NOLOCK) ON val.IdCustomer = c.Id AND val.IdOptionType = opt.Id 
	WHERE c.IdObjectType=2 AND c.StatusCode NOT IN (3,5)

GO