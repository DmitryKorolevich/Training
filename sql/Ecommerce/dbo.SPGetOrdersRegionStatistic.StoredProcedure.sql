/****** Object:  StoredProcedure [dbo].[SPGetOrdersRegionStatistic]    Script Date: 6/25/2016 2:13:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SPGetOrdersRegionStatistic]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SPGetOrdersRegionStatistic] AS' 
END
GO
ALTER PROCEDURE [dbo].[SPGetOrdersRegionStatistic]
	@from datetime2, @to datetime2,
	@IdCustomerType int = null, @IdOrderType int = null
AS
BEGIN
	
	SELECT temp.Region, SUM(temp.Total) As Amount, COUNT(*) As Count FROM 
		(SELECT (CASE WHEN pad.IdState IS NOT NULL THEN s.StateCode ELSE pad.County END) As Region, o.Total
		FROM Orders o WITH(NOLOCK)
		LEFT JOIN OrderOptionTypes AS oopt WITH(NOLOCK) ON oopt.Name = N'POrderType' AND (oopt.IdObjectType = o.IdObjectType OR oopt.IdObjectType IS NULL)
		LEFT JOIN OrderOptionValues AS oval WITH(NOLOCK) ON oval.IdOrder = o.Id AND oval.IdOptionType = oopt.Id
		INNER JOIN Customers c WITH(NOLOCK) ON o.IdCustomer=c.Id
		INNER JOIN OrderPaymentMethods opm WITH(NOLOCK) ON o.IdPaymentMethod =opm.Id
		INNER JOIN OrderAddresses pad WITH(NOLOCK) ON opm.IdAddress=pad.Id
		LEFT JOIN States s WITH(NOLOCK) ON pad.IdState=s.Id
		WHERE o.StatusCode!=3 AND o.OrderStatus IN (2,3,5) AND 
		o.DateCreated>=@from AND o.DateCreated<@to AND
		(@IdCustomerType IS NULL OR (c.IdObjectType = @IdCustomerType)) AND	
		(@IdOrderType IS NULL OR (oval.Value = @IdOrderType))) temp		
	WHERE temp.Region IS NOT NULL
	GROUP BY temp.Region
	ORDER BY SUM(temp.Total) DESC

END


GO
