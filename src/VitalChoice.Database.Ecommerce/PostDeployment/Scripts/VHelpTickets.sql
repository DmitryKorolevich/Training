GO

IF OBJECT_ID(N'[dbo].[VHelpTickets]', N'V') IS NOT NULL
	DROP VIEW [dbo].[VHelpTickets]
GO
CREATE VIEW [dbo].[VHelpTickets]
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
	options.FirstName+' '+options.LastName As Customer,
	c.Email As CustomerEmail
	FROM HelpTickets AS h
	JOIN Orders AS o ON h.IdOrder=o.Id
	JOIN Customers AS c ON c.Id = o.[IdCustomer]
	JOIN Addresses AS ad ON ad.IdCustomer = c.Id AND ad.IdObjectType = (SELECT [Id] FROM [dbo].[AddressTypes] WHERE [Name] = N'Profile')
	LEFT JOIN (SELECT [IdAddress], [FirstName], [LastName]
	FROM (SELECT [IdAddress], [Name], [Value] FROM [dbo].[AddressOptionTypes] AS adt
	INNER JOIN [dbo].[AddressOptionValues] AS adv on adt.Id = adv.IdOptionType ) AS source
	PIVOT(
	MIN([Value]) FOR [Name] in ([FirstName], [LastName])
	) AS piv) AS options ON ad.Id = options.IdAddress
	LEFT OUTER JOIN [dbo].[States] AS st ON ad.IdState = st.Id
	WHERE h.StatusCode!=3


GO
