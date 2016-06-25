/****** Object:  StoredProcedure [dbo].[SPGetOrderStatisticByTypeForOrdersSummarySalesReport]    Script Date: 6/25/2016 2:13:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SPGetOrderStatisticByTypeForOrdersSummarySalesReport]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SPGetOrderStatisticByTypeForOrdersSummarySalesReport] AS' 
END
GO
ALTER PROCEDURE [dbo].[SPGetOrderStatisticByTypeForOrdersSummarySalesReport]
	@from datetime2,
	@to datetime2,
	@shipfrom datetime2 = NULL,
	@shipto datetime2 = NULL, 
	@idcustomersource int = NULL,
	@customersourcedetails nvarchar(250) = NULL,
	@fromcount int = NULL,
	@tocount int = null,
	@keycode nvarchar(250) = NULL,
	@idcustomer int = NULL,
	@firstorderfrom datetime2 =NULL,
	@firstorderto datetime2 =NULL,
	@idcustomertype int = NULL,
	@discountcode nvarchar(250) = NULL,
	@isaffiliate bit =NULL
AS
BEGIN

	SET NOCOUNT ON


		;WITH tempOrders(
			OrderType, Id, Total
		) As
		(
			SELECT 
				otval.Value as OrderType , temp.Id, temp.Total
				FROM
				(SELECT 
					o.Id, o.IdObjectType, o.IdCustomer, c.IdObjectType As CustomerIdObjectType, o.IdDiscount, c.DateCreated AS CustomerDateCreated,
					o.Total
				FROM Orders o WITH(NOLOCK)
				JOIN Customers c WITH(NOLOCK) ON o.IdCustomer=c.Id
				WHERE
					o.DateCreated>=@from AND o.DateCreated<=@to AND
					o.StatusCode!=3 AND o.IdObjectType NOT IN (2,5,6) AND 
					((o.OrderStatus IS NOT NULL AND o.OrderStatus !=1 AND o.OrderStatus !=4 AND o.OrderStatus !=6 ) OR 
					(o.OrderStatus IS NULL AND o.POrderStatus !=1 AND o.POrderStatus !=4 AND o.POrderStatus !=6 AND 
					o.NPOrderStatus !=1 AND o.NPOrderStatus !=4 AND o.NPOrderStatus !=6)) AND
					(@idcustomer IS NULL OR o.IdCustomer = @idcustomer) AND	
					(@idcustomertype IS NULL OR c.IdObjectType = @idcustomertype)) temp	
			LEFT JOIN OrderOptionTypes AS kcopt WITH(NOLOCK) ON kcopt.Name = N'KeyCode' AND kcopt.IdObjectType = temp.IdObjectType
			LEFT JOIN OrderOptionValues AS kcval WITH(NOLOCK) ON kcval.IdOrder = temp.Id AND kcval.IdOptionType = kcopt.Id
			JOIN OrderOptionTypes AS otopt WITH(NOLOCK) ON otopt.Name = N'OrderType'
			JOIN OrderOptionValues AS otval WITH(NOLOCK) ON otval.IdOrder = temp.Id AND otval.IdOptionType = otopt.Id
			LEFT JOIN CustomerOptionTypes AS sopt WITH(NOLOCK) ON sopt.Name = N'Source'
			LEFT JOIN CustomerOptionValues AS sval WITH(NOLOCK) ON sval.IdCustomer = temp.IdCustomer AND sval.IdOptionType = sopt.Id
			LEFT JOIN CustomerOptionTypes AS sdopt WITH(NOLOCK) ON sdopt.Name = N'SourceDetails'
			LEFT JOIN CustomerOptionValues AS sdval WITH(NOLOCK) ON sdval.IdCustomer = temp.IdCustomer AND sdval.IdOptionType = sdopt.Id
			LEFT JOIN VOrderCountOnCustomers As occ WITH(NOLOCK) ON occ.IdCustomer = temp.IdCustomer
			LEFT JOIN VFirstOrderOnCustomers As foc WITH(NOLOCK) ON foc.IdCustomer = temp.IdCustomer
			LEFT JOIN Discounts As d WITH(NOLOCK) ON d.Id = temp.IdDiscount
			LEFT JOIN AffiliateOrderPayments As aop WITH(NOLOCK) ON aop.Id = temp.Id
			WHERE 
				(@keycode IS NULL OR kcval.Value = @keycode) AND 
				(@idcustomersource IS NULL OR sval.Value = @idcustomersource) AND
				(@customersourcedetails IS NULL OR sdval.Value = @customersourcedetails) AND 		
				(@fromcount IS NULL OR @fromcount<=occ.Count) AND		
				(@tocount IS NULL OR @tocount>=occ.Count) AND	
				(@firstorderfrom IS NULL OR (foc.DateCreated IS NOT NULL AND @firstorderfrom<=foc.DateCreated) 
				OR (foc.DateCreated IS NULL AND @firstorderfrom<=temp.CustomerDateCreated)) AND		
				(@firstorderto IS NULL OR (foc.DateCreated IS NOT NULL AND @firstorderto>=foc.DateCreated) 
				OR (foc.DateCreated IS NULL AND @firstorderto>=temp.CustomerDateCreated)) AND		
				(@discountcode IS NULL OR @discountcode=d.Code) AND
				(@isaffiliate IS NULL OR @isaffiliate=0 OR (@isaffiliate=1 AND aop.IdAffiliate IS NOT NULL))
		)

		SELECT CAST(OrderType as int) as Id, COUNT(*) Count, SUM(Total) Total FROM tempOrders
		GROUP BY OrderType

END


GO
