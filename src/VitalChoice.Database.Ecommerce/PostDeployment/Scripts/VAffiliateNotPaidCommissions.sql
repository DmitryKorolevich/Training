IF OBJECT_ID(N'[dbo].[VAffiliateNotPaidCommissions]', N'V') IS NOT NULL
	DROP VIEW [dbo].[VAffiliateNotPaidCommissions]
GO
CREATE VIEW [dbo].[VAffiliateNotPaidCommissions]
WITH SCHEMABINDING
AS 
SELECT op.IdAffiliate As Id, SUM(op.Amount) As Amount, COUNT_BIG(*) As Count FROM [dbo].AffiliateOrderPayments op
INNER JOIN [dbo].Orders o ON o.Id=op.Id
INNER JOIN [dbo].AppOptions ao ON ao.OptionName='AffiliateOrderPaymentsCountToDate'
WHERE op.Status=1 AND o.DateCreated<CONVERT(datetime2,ao.OptionValue, 21)
GROUP BY op.IdAffiliate

GO
CREATE UNIQUE CLUSTERED INDEX IX_IdAffiliate ON [dbo].[VAffiliateNotPaidCommissions] ([Id])
GO