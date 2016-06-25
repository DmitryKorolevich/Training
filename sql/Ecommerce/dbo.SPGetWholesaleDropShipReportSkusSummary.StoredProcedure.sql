/****** Object:  StoredProcedure [dbo].[SPGetWholesaleDropShipReportSkusSummary]    Script Date: 6/25/2016 2:13:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SPGetWholesaleDropShipReportSkusSummary]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SPGetWholesaleDropShipReportSkusSummary] AS' 
END
GO
ALTER PROCEDURE [dbo].[SPGetWholesaleDropShipReportSkusSummary]
	@from datetime2,
	@to datetime2,
	@shipfrom datetime2 = NULL,
	@shipto datetime2 = NULL, 
	@idcustomertype int = NULL,
	@idtradeclass int = NULL,
	@customerfirstname nvarchar(250) = NULL,
	@customerlastname nvarchar(250) = NULL,
	@shipfirstname nvarchar(250) = NULL,
	@shiplastname nvarchar(250) = NULL,
	@shipidconfirm nvarchar(250) = NULL,
	@idorder int = NULL,
	@ponumber nvarchar(250) = NULL
AS
BEGIN

	SET NOCOUNT ON
		
	DECLARE @orderIds AS TABLE
    (
        Id int NOT NULL
    );

	INSERT INTO @orderIds	
	EXEC [dbo].[SPGetOrderIdsForWholesaleDropShipReport]
		@from = @from,
		@to = @to,
		@shipfrom = @shipfrom,
		@shipto = @shipto,
		@idcustomertype = @idcustomertype,
		@idtradeclass = @idtradeclass,
		@customerfirstname = @customerfirstname,
		@customerlastname = @customerlastname,
		@shipfirstname = @shipfirstname,
		@shiplastname = @shiplastname,
		@shipidconfirm = @shipidconfirm,
		@idorder = @idorder,
		@ponumber = @ponumber,
		@pageindex = NULL,
		@pagesize = NULL,
		@getcount = NULL

		SELECT temp.IdSku as Id, s.Code, temp.Quantity, temp.Amount, orders.ProductsSubtotal, orders.DiscountTotal,
			orders.ShippingTotal, orders.Total FROM 
			(SELECT temp.IdSku, SUM(temp.Quantity) as Quantity, SUM(temp.Amount) as Amount FROM
				(SELECT ors.IdSku, ors.Quantity, ors.Amount*ors.Quantity as Amount
				FROM @orderIds ids
				JOIN OrderToSkus ors WITH(NOLOCK) ON ids.Id=ors.IdOrder
				UNION ALL
				SELECT orp.IdSku, orp.Quantity, orp.Amount*orp.Quantity as Amount
				FROM @orderIds ids
				JOIN OrderToPromos orp WITH(NOLOCK) ON ids.Id=orp.IdOrder AND orp.Disabled=0) temp
			GROUP BY temp.IdSku) temp
		JOIN Skus s WITH(NOLOCK) ON temp.IdSku=s.Id
		CROSS JOIN 
			(SELECT SUM(o.ProductsSubtotal) as ProductsSubtotal, SUM(o.DiscountTotal) as DiscountTotal,
			SUM(o.ShippingTotal) as ShippingTotal, SUM(o.Total) as Total
			FROM @orderIds ids
			JOIN Orders o WITH(NOLOCK) ON ids.Id=o.Id) AS orders

END


GO
