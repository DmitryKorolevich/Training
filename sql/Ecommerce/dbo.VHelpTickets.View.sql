/****** Object:  View [dbo].[VHelpTickets]    Script Date: 6/25/2016 2:13:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[VHelpTickets]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[VHelpTickets]
AS 
SELECT 
	h.Id,
	h.IdOrder,
	h.[Priority],
	h.Summary,
	h.DateCreated,
	h.DateEdited,
	h.StatusCode,
	c.Id AS IdCustomer,
	options.FirstName+'' ''+options.LastName As Customer,
	c.Email As CustomerEmail
	FROM HelpTickets AS h
	JOIN Orders AS o ON h.IdOrder=o.Id
	JOIN Customers AS c ON c.Id = o.[IdCustomer]
	JOIN Addresses AS ad ON ad.Id = c.IdProfileAddress
	LEFT JOIN (SELECT [IdAddress], [FirstName], [LastName]
	FROM (SELECT [IdAddress], [Name], [Value] FROM [dbo].[AddressOptionTypes] AS adt
	INNER JOIN [dbo].[AddressOptionValues] AS adv on adt.Id = adv.IdOptionType ) AS source
	PIVOT(
	MIN([Value]) FOR [Name] in ([FirstName], [LastName])
	) AS piv) AS options ON ad.Id = options.IdAddress
	LEFT OUTER JOIN [dbo].[States] AS st ON ad.IdState = st.Id
	WHERE h.StatusCode!=3


' 
GO
