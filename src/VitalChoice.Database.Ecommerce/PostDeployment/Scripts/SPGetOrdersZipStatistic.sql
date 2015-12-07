IF OBJECT_ID(N'[dbo].[SPGetOrdersZipStatistic]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetOrdersZipStatistic]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE SPGetOrdersZipStatistic
	@from datetime2, @to datetime2,
	@IdCustomerType int = null, @IdOrderType int = null
AS
BEGIN
	
	SELECT temp.Zip, SUM(temp.Total) As Amount, COUNT(*) As Count FROM 
		(SELECT aval.Value As Zip, o.Total
		FROM Orders o 
		LEFT JOIN OrderOptionTypes AS oopt ON oopt.Name = N'POrderType' AND (oopt.IdObjectType = o.IdObjectType OR oopt.IdObjectType IS NULL)
		LEFT JOIN OrderOptionValues AS oval ON oval.IdOrder = o.Id AND oval.IdOptionType = oopt.Id
		INNER JOIN Customers c ON o.IdCustomer=c.Id
		INNER JOIN OrderPaymentMethods opm ON o.IdPaymentMethod =opm.Id
		INNER JOIN OrderAddresses pad ON opm.IdAddress=pad.Id
		LEFT JOIN AddressOptionTypes AS aopt ON aopt.Name = N'Zip'
		LEFT JOIN OrderAddressOptionValues AS aval ON aval.IdOrderAddress = pad.Id AND aval.IdOptionType = aopt.Id
		WHERE o.StatusCode!=3 AND o.OrderStatus IN (2,3,5) AND 
		o.DateCreated>=@from AND o.DateCreated<@to AND
		(@IdCustomerType IS NULL OR (c.IdObjectType = @IdCustomerType)) AND	
		(@IdOrderType IS NULL OR (oval.Value = @IdOrderType))) temp
	WHERE temp.Zip IS NOT NULL
	GROUP BY temp.Zip
	ORDER BY SUM(temp.Total) DESC

END

GO