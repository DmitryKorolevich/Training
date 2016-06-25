/****** Object:  View [dbo].[VAffiliates]    Script Date: 6/25/2016 2:13:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[VAffiliates]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[VAffiliates]
AS 
SELECT 
	a.Id,
	a.StatusCode,
	a.Name,
	cval.Value as Company,
	wval.Value as WebSite,
	a.CommissionFirst,
	a.CommissionAll,
	tval.Value as Tier,
	ISNULL(cust.Count,0) as CustomersCount,
	a.DateEdited,
	a.IdEditedBy
	FROM Affiliates AS a
	LEFT JOIN AffiliateOptionTypes AS copt ON copt.Name = N''Company''
	LEFT JOIN AffiliateOptionValues AS cval ON cval.IdAffiliate = a.Id AND cval.IdOptionType = copt.Id
	LEFT JOIN AffiliateOptionTypes AS wopt ON wopt.Name = N''WebSite''
	LEFT JOIN AffiliateOptionValues AS wval ON wval.IdAffiliate = a.Id AND wval.IdOptionType = wopt.Id
	LEFT JOIN AffiliateOptionTypes AS topt ON topt.Name = N''Tier''
	LEFT JOIN AffiliateOptionValues AS tval ON tval.IdAffiliate = a.Id AND tval.IdOptionType = topt.Id
	LEFT JOIN (SELECT ca.Id, COUNT(*) AS Count
	FROM dbo.Affiliates AS ca
	INNER JOIN dbo.Customers AS c ON ca.Id = c.IdAffiliate
	GROUP BY ca.Id) cust ON cust.Id = a.Id

' 
GO
