IF OBJECT_ID(N'[dbo].[VAffiliateNotPaidCommissions]', N'V') IS NOT NULL
	DROP VIEW [dbo].[VAffiliateNotPaidCommissions]
GO
CREATE VIEW [dbo].[VAffiliateNotPaidCommissions]
WITH SCHEMABINDING
AS 
	SELECT 
		op.IdAffiliate As Id,
		SUM(op.Amount) As Amount,
		COUNT_BIG(*) As Count 
	FROM [dbo].AffiliateOrderPayments op
	INNER JOIN [dbo].Orders o ON o.Id=op.Id
	INNER JOIN [dbo].AppOptions ao ON ao.OptionName='AffiliateOrderPaymentsCountToDate'
	WHERE 
		op.Status=1 AND o.DateCreated<CONVERT(datetime2,ao.OptionValue, 21) AND o.DateCreated>=DATEADD(mm,-6,CONVERT(datetime2,OptionValue, 21)) AND
		o.StatusCode!=3 AND 
		(
			(o.OrderStatus IS NOT NULL AND o.OrderStatus IN (2,3,5)) OR
			(o.OrderStatus IS NULL AND (o.POrderStatus IN (2,3,5) OR o.NPOrderStatus IN (2,3,5)))
		)
	GROUP BY op.IdAffiliate

GO
CREATE UNIQUE CLUSTERED INDEX IX_IdAffiliate ON [dbo].[VAffiliateNotPaidCommissions] ([Id])
GO