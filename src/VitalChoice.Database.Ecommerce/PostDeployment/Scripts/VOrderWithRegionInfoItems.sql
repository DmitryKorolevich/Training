GO

IF OBJECT_ID(N'[dbo].[VOrderWithRegionInfoItems]', N'V') IS NOT NULL
	DROP VIEW [dbo].[VOrderWithRegionInfoItems]
GO
CREATE VIEW [dbo].[VOrderWithRegionInfoItems]
AS 

	SELECT o.Id, o.DateCreated, o.IdCustomer, aval.Value As Zip,
		(CASE WHEN pad.IdState IS NOT NULL THEN s.StateCode ELSE pad.County END) As Region, acval.Value As City, o.Total,
		c.IdObjectType As IdCustomerType, oval.Value As OrderType
		FROM Orders o 
		LEFT JOIN OrderOptionTypes AS oopt ON oopt.Name = N'POrderType' AND (oopt.IdObjectType = o.IdObjectType OR oopt.IdObjectType IS NULL)
		LEFT JOIN OrderOptionValues AS oval ON oval.IdOrder = o.Id AND oval.IdOptionType = oopt.Id
		INNER JOIN Customers c ON o.IdCustomer=c.Id
		INNER JOIN OrderPaymentMethods opm ON o.IdPaymentMethod =opm.Id
		INNER JOIN OrderAddresses pad ON opm.IdAddress=pad.Id
		LEFT JOIN AddressOptionTypes AS aopt ON aopt.Name = N'Zip'
		LEFT JOIN OrderAddressOptionValues AS aval ON aval.IdOrderAddress = pad.Id AND aval.IdOptionType = aopt.Id
		LEFT JOIN AddressOptionTypes AS acopt ON acopt.Name = N'City'
		LEFT JOIN OrderAddressOptionValues AS acval ON acval.IdOrderAddress = pad.Id AND acval.IdOptionType = acopt.Id
		LEFT JOIN States s ON pad.IdState=s.Id
		WHERE o.StatusCode!=3 AND o.OrderStatus IN (2,3,5) AND (pad.IdState IS NOT NULL OR pad.County IS NOT NULL)
		AND aval.Value IS NOT NULL

GO


