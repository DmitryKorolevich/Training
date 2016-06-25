/****** Object:  View [dbo].[VCustomers]    Script Date: 6/25/2016 2:13:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[VCustomers]'))
EXEC dbo.sp_executesql @statement = N'
CREATE VIEW [dbo].[VCustomers]
AS
SELECT
	cu.[Id],
	cu.[IdAffiliate],
	cu.[Email],
	options.[FirstName],
	options.[LastName],
	cu.[DateEdited],
	cu.[StatusCode],
	cu.[IdObjectType],
	cu.[IdEditedBy],
	options.[Company],
	co.[CountryCode],
	co.[CountryName],
	st.[StateCode],
	st.[StateName],
	ad.[County],
	options.[City],
	options.[Address1],
	options.[Address2],
	options.[Phone],
	options.[Zip],
	ISNULL(st.[StateCode], ad.[County]) AS StateOrCounty,
	(SELECT count(*) FROM Orders o WHERE cu.Id=o.IdCustomer AND o.StatusCode!=3 AND o.OrderStatus IN (2,3,5) AND o.IdObjectType!=6) AS TotalOrders,
	(SELECT TOP 1 o.DateCreated FROM Orders o WHERE cu.Id=o.IdCustomer AND o.StatusCode!=3 AND o.OrderStatus IN (2,3,5) AND o.IdObjectType!=6 ORDER BY o.DateCreated DESC) AS LastOrderPlaced,
	(SELECT TOP 1 o.DateCreated FROM Orders o WHERE cu.Id=o.IdCustomer AND o.StatusCode!=3 AND o.OrderStatus IN (2,3,5) AND o.IdObjectType!=6 ORDER BY o.DateCreated ASC) AS FirstOrderPlaced
FROM [dbo].[Customers] AS cu
LEFT OUTER JOIN [dbo].[Addresses] AS ad ON cu.IdProfileAddress = ad.Id
LEFT OUTER JOIN (SELECT
	[IdAddress],
	[FirstName],
	[LastName],
	[City],
	[Company],
	[Address1],
	[Address2],
	[Phone],
	[Zip]
FROM (SELECT [IdAddress],[Name],[Value] FROM [dbo].[AddressOptionTypes] AS adt
INNER JOIN [dbo].[AddressOptionValues] AS adv ON adt.Id = adv.IdOptionType) AS source
PIVOT (
MIN([Value]) FOR [Name] IN ([FirstName], [LastName], [City], [Company], [Address1], [Address2], [Phone], [Zip])
) AS piv) AS options ON ad.Id = options.IdAddress
LEFT OUTER JOIN [dbo].[Countries] AS co ON ad.IdCountry = co.Id
LEFT OUTER JOIN [dbo].[States] AS st ON ad.IdState = st.Id

' 
GO
