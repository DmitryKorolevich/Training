/****** Object:  View [dbo].[VAffiliateNotPaidCommissions]    Script Date: 6/25/2016 2:13:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[VAffiliateNotPaidCommissions]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[VAffiliateNotPaidCommissions]
WITH SCHEMABINDING
AS 
SELECT op.IdAffiliate As Id, SUM(op.Amount) As Amount, COUNT_BIG(*) As Count FROM [dbo].AffiliateOrderPayments op
INNER JOIN [dbo].Orders o ON o.Id=op.Id
INNER JOIN [dbo].AppOptions ao ON ao.OptionName=''AffiliateOrderPaymentsCountToDate''
WHERE op.Status=1 AND o.DateCreated<CONVERT(datetime2,ao.OptionValue, 21)
GROUP BY op.IdAffiliate

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
/****** Object:  Index [IX_IdAffiliate]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[VAffiliateNotPaidCommissions]') AND name = N'IX_IdAffiliate')
CREATE UNIQUE CLUSTERED INDEX [IX_IdAffiliate] ON [dbo].[VAffiliateNotPaidCommissions]
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
