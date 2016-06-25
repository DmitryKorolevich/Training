/****** Object:  View [dbo].[VOrderWithRegionInfoItems]    Script Date: 6/25/2016 2:13:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[VOrderWithRegionInfoItems]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[VOrderWithRegionInfoItems]
AS 

	SELECT o.Id, o.DateCreated, o.IdCustomer, aval.Value As Zip,
		(CASE WHEN pad.IdState IS NOT NULL THEN s.StateCode ELSE pad.County END) As Region, acval.Value As City, o.Total,
		c.IdObjectType As IdCustomerType, oval.Value As OrderType
		FROM Orders o 
		LEFT JOIN OrderOptionTypes AS oopt ON oopt.Name = N''POrderType'' AND (oopt.IdObjectType = o.IdObjectType OR oopt.IdObjectType IS NULL)
		LEFT JOIN OrderOptionValues AS oval ON oval.IdOrder = o.Id AND oval.IdOptionType = oopt.Id
		INNER JOIN Customers c ON o.IdCustomer=c.Id
		INNER JOIN OrderPaymentMethods opm ON o.IdPaymentMethod =opm.Id
		INNER JOIN OrderAddresses pad ON opm.IdAddress=pad.Id
		LEFT JOIN AddressOptionTypes AS aopt ON aopt.Name = N''Zip''
		LEFT JOIN OrderAddressOptionValues AS aval ON aval.IdOrderAddress = pad.Id AND aval.IdOptionType = aopt.Id
		LEFT JOIN AddressOptionTypes AS acopt ON acopt.Name = N''City''
		LEFT JOIN OrderAddressOptionValues AS acval ON acval.IdOrderAddress = pad.Id AND acval.IdOptionType = acopt.Id
		LEFT JOIN States s ON pad.IdState=s.Id
		WHERE o.StatusCode!=3 AND o.OrderStatus IN (2,3,5) AND (pad.IdState IS NOT NULL OR pad.County IS NOT NULL)

' 
GO
